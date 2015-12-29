using System;
using System.Collections.Generic;
using CmpInterfaceModel;
using CmpServiceLib.Models;

namespace CmpServiceLib.Stages
{
    public class StartSequencesStage : Stage
    {
        public Func<VmDeploymentRequest, ProcessorVm.SequenceExecutionResults> ExecuteSequences { get; set; }

        public bool ProcessSequences { get; set; }

        public override object Execute()
        {
            if (AllQueuesBlocked)
                return 0;

            var cdb = new CmpDb(CmpDbConnectionString);
            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.StartingSequences.ToString(), true);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessTransferedSubmissions() " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }

            foreach (var vmReq in vmReqList)
            {
                try
                {
                    //AssertIfTimedOut(vmReq, 0, Constants.StatusEnum.PostProcessing1.ToString());

                    var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                    //_processSequences

                    if (!ProcessSequences)
                    {
                        vmReq.StatusMessage = "Sequence processing disabled";
                    }
                    else if ((null == vmc.ScriptList) || (0 == vmc.ScriptList.Count))
                    {
                        vmReq.StatusMessage = "No sequences specified";
                    }
                    else
                    {
                        //*** Temporary **********
                        foreach (var scriptSpec in vmc.ScriptList)
                            scriptSpec.ExecuteInState = Constants.StatusEnum.StartingSequences.ToString();

                        vmReq.Config = vmc.Serialize();
                        ExecuteSequences(vmReq);
                        vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);
                        vmReq.StatusMessage = "Sequences Initiated";

                        foreach (var sequence in vmc.ScriptList)
                            vmReq.StatusMessage += string.Format(" - {0} : {1}", sequence.Name, sequence.ResultCode);
                    }

                    vmReq.CurrentStateStartTime = DateTime.UtcNow;
                    vmReq.ExceptionMessage = null;
                    vmReq.StatusCode = Constants.StatusEnum.RunningSequences.ToString();
                    cdb.SetVmDepRequestStatus(vmReq, null);
                }
                catch (Exception ex)
                {
                    if (null == vmReq)
                        LogThis(ex, System.Diagnostics.EventLogEntryType.Error,
                            "Exception in StartingSequences() : " +
                            CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex), 100, 100);
                    else
                    {
                        vmReq.CurrentStateStartTime = DateTime.UtcNow;
                        vmReq.ExceptionMessage = "Exception in StartingSequences() : " +
                                                 CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex);
                        vmReq.StatusMessage = vmReq.ExceptionMessage;
                        vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                        cdb.SetVmDepRequestStatus(vmReq, null);

                        LogThis(ex, System.Diagnostics.EventLogEntryType.Warning,
                            vmReq.ExceptionMessage, 100, 100);
                    }
                }
            }

            return 0;
        }
    }
}