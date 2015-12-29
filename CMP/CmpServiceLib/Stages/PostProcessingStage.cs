using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CmpInterfaceModel;
using CmpServiceLib.Models;

namespace CmpServiceLib.Stages
{
    public class PostProcessingStage : Stage
    {
        public Action<VmDeploymentRequest> ClearMaintenanceMode { get; set; }

        public Action<VmDeploymentRequest, List<string>, CmpDb>  CompleteVmRequest { get; set; }

        public int DwellTime { get; set; }

        public Func<VmDeploymentRequest, Constants.OsFamily> GetOsFamily { get; set; } 

        public Func<VmDeploymentRequest, PowershellLib.Remoting> GetPowershellConnection { get; set; }

        public bool IsMsitDeployment { get; set; }

        public bool PerformValidation { get; set; }

        public Func<VmDeploymentRequest, PowershellLib.Remoting, VmDeploymentRequest> PostQcValidation { get; set; }

        public string REBOOT_EXIT_TRAP_MATCH { get; set; }

        public override object Execute()
        {
            if (AllQueuesBlocked)
                return 0;

            List<string> warninglist = null;
            List<string> commandList = null;
            PowershellLib.Remoting psRem = null;

            var cdb = new CmpDb(CmpDbConnectionString);
            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.PostProcessing1.ToString(), true);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessPostProcessStage1() " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }

            foreach (var vmReq in vmReqList)
            {
                var vmReq2 = vmReq;

                try
                {
                    //*** Is it a non-Windows deployment? ***
                    if (GetOsFamily(vmReq) != Constants.OsFamily.Windows)
                    {
                        CompleteVmRequest(vmReq, warninglist, cdb);
                        continue;
                    }

                    //*** Obtain Powershell connection ***

                    psRem = null;

                    try
                    {
                        psRem = GetPowershellConnection(vmReq);

                        vmReq.CurrentStateStartTime = DateTime.UtcNow;
                        vmReq.ExceptionMessage = "";
                        vmReq.StatusMessage = "Post Processing";
                        vmReq.StatusCode = Constants.StatusEnum.PostProcessing1.ToString();
                        try
                        {
                            cdb.SetVmDepRequestStatus(vmReq, warninglist);
                        }
                        catch (Exception ex)
                        {
                            LogThis(ex, System.Diagnostics.EventLogEntryType.Error,
                                "Exception while setting req status in PostProcessing1()", 100, 100);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (null == psRem)
                        continue;

                    //*************

                    /*foreach (var scriptSpec in vmc.ScriptList)
                    {
                        if (scriptSpec.ExecuteInState.Equals(Constants.StatusEnum.PostProcessing1.ToString(),
                            StringComparison.InvariantCultureIgnoreCase))
                            scriptSpec.ExecuteInState = Constants.StatusEnum.PostProvisioningQC.ToString();
                    }

                    vmReq.Config = vmc.Serialize();

                    //*************

                    var exSeqResult = ExecuteSequences(vmReq);
                    cdb.SetVmDepRequestStatus(vmReq, null);*/

                    //*** Final reboot ***

                    try
                    {
                        commandList = new List<string>(1) { @"Restart-Computer -force" };
                        psRem.Execute(null, commandList);
                        Thread.Sleep(DwellTime);
                    }
                    catch (Exception ex)
                    {
                        var message = CmpCommon.Utilities.UnwindExceptionMessages(ex);

                        if (!message.Contains(REBOOT_EXIT_TRAP_MATCH))
                        {
                            if (null == warninglist)
                                warninglist = new List<string>(1);

                            warninglist.Add("Exception during final VM restart (possibly benign) : " + message);
                        }
                    }

                    CompleteVmRequest(vmReq, warninglist, cdb);
                }
                catch (Exception ex)
                {
                    vmReq2.CurrentStateStartTime = DateTime.UtcNow;
                    vmReq2.ExceptionMessage = "Exception in ProcessPostProcessStage1() : " +
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex);
                    vmReq2.StatusMessage = Constants.StatusEnum.Exception.ToString();
                    vmReq2.StatusCode = Constants.StatusEnum.Exception.ToString();
                    cdb.SetVmDepRequestStatus(vmReq2, null);

                    LogThis(ex, System.Diagnostics.EventLogEntryType.Warning,
                        "Exception in ProcessPostProcessStage1()", 100, 100);
                }
                finally
                {
                    if (null != psRem)
                        try
                        {
                            psRem.Dispose();
                        }
                        catch (Exception)
                        {
                            //*** Ignore ***
                        }
                }
            }

            return 0;
        }
    }
}