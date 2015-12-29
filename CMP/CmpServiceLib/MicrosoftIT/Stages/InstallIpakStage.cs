using System;
using System.Collections.Generic;
using CmpInterfaceModel;
using CmpServiceLib.Stages;

namespace CmpServiceLib.MicrosoftIT.Stages
{
    public class InstallIpakStage : Stage
    {
        public override object Execute()
        {
            if (AllQueuesBlocked)
                return 0;

            var cdb = new CmpDb(CmpDbConnectionString);
            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.InstallingIpak.ToString(), true);
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
                    //AssertIfTimedOut(vmReq, 0, Constants.StatusEnum.InstallingIpak.ToString());
                    var domainJoin = Utilities.GetXmlInnerText(vmReq.Config, "DomainJoin");
                    /*
                    //*** change OU and IPAK only if a domain join is specified in config ***
                    if (null != domainJoin)
                    {
                        //*** Change OU ***
                        //ChangeOu(vmReq, true);

                        //*** Install IPak ***

                        //*** Set Status ***
                        vmReq.CurrentStateStartTime = DateTime.UtcNow;
                        vmReq.StatusCode = Constants.StatusEnum.StartingSequences.ToString();
                        vmReq.StatusMessage = "Starting final stage sequences";
                    }*/
                    // move to next sequence for domain and non-domain joined vms
                    vmReq.CurrentStateStartTime = DateTime.UtcNow;
                    vmReq.StatusCode = Constants.StatusEnum.StartingSequences.ToString();
                    vmReq.StatusMessage = "Starting final stage sequences";

                    try
                    {
                        cdb.SetVmDepRequestStatus(vmReq, null);
                    }
                    catch (Exception ex)
                    {
                        LogThis(ex, System.Diagnostics.EventLogEntryType.Error,
                            "Exception while setting req status in ProcessInstallIpak()", 100, 100);
                    }
                }
                catch (Exception ex)
                {
                    LogThis(ex, System.Diagnostics.EventLogEntryType.Error,
                        "Exception in ProcessInstallIpak()", 100, 100);
                }
            }

            return 0;
        }
    }
}