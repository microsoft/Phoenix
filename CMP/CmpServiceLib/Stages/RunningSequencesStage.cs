using System;
using System.Collections.Generic;
using CmpInterfaceModel;
using CmpServiceLib.Models;

namespace CmpServiceLib.Stages
{
    public class RunningSequencesStage : Stage
    {
        public Func<VmDeploymentRequest, ProcessorVm.SequenceExecutionResults> CheckSequencesStatus { get; set; }

        public bool ProcessSequences { get; set; }

        public override object Execute()
        {
            if (AllQueuesBlocked)
                return 0;

            List<string> warninglist = null;
            PowershellLib.Remoting psRem = null;
            var allSequencesComplete = true;
            var hadException = false;

            var cdb = new CmpDb(CmpDbConnectionString);
            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.RunningSequences.ToString(), true);
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

                    if (!ProcessSequences)
                    {
                        allSequencesComplete = true;
                    }
                    else if ((null == vmc.ScriptList) || (0 == vmc.ScriptList.Count))
                    {
                        allSequencesComplete = true;
                    }
                    else
                    {
                        var exSeqResult = CheckSequencesStatus(vmReq);

                        if (!exSeqResult.HadChange)
                            continue;

                        vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                        vmReq.StatusMessage = "Sequences";

                        foreach (var sequence in vmc.ScriptList)
                        {
                            if (sequence.ResultCode != null) // not all sequences return a result code
                            {
                                vmReq.StatusMessage += string.Format(
                                    " - {0} : {1}", sequence.Name, sequence.ResultCode);

                                if (sequence.ResultCode.Equals("Exception"))
                                    hadException = true;

                                if (!sequence.ResultCode.Equals("Completed"))
                                    allSequencesComplete = false;
                            }
                        }

                        if (hadException)
                            throw new Exception(vmReq.StatusMessage);

                        vmReq.ExceptionMessage = null;
                        cdb.SetVmDepRequestStatus(vmReq, null);
                    }

                    if (allSequencesComplete)
                    {
                        vmReq.CurrentStateStartTime = DateTime.UtcNow;
                        vmReq.StatusMessage = "Ready for post processing";
                        vmReq.StatusCode = Constants.StatusEnum.PostProcessing1.ToString();
                        cdb.SetVmDepRequestStatus(vmReq, null);
                    }

                }
                catch (Exception ex)
                {
                    if (null == vmReq)
                        LogThis(ex, System.Diagnostics.EventLogEntryType.Error,
                            "Exception in RunningSequences() : " +
                            CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex), 100, 100);
                    else
                    {
                        vmReq.CurrentStateStartTime = DateTime.UtcNow;
                        vmReq.ExceptionMessage = "Exception in RunningSequences() : " +
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