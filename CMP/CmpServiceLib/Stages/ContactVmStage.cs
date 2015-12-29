using System;
using System.Collections.Generic;
using CmpInterfaceModel;
using CmpServiceLib.Models;

namespace CmpServiceLib.Stages
{
    public class ContactVmStage : Stage
    {
        public string AftsDbConnectionString { get; set; }

        public int ContactingVmMinutesTTL { get; set; }

        public int DeleteDwelltimeMinutes { get; set; }

        public Action<VmDeploymentRequest, int, bool, bool> DeleteVm { get; set; }

        public bool DisableSmartCardAuth { get; set; }

        public Func<VmDeploymentRequest, int, bool> HasTimedOut { get; set; }

        public Action<VmDeploymentRequest, string, CmpDb, bool, bool> ResubmitRequest { get; set; }

        #region TestPsConnection

        public delegate Constants.PostProvInintDisksResultEnum TestPsConnectionDelegate(
            VmDeploymentRequest vmReq, out string remotePsUrl,
            out PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility rpcPortVisibility);

        public TestPsConnectionDelegate TestPsConnection { get; set; }

        #endregion

        public override object Execute()
        {
            if (AllQueuesBlocked)
                return 0;

            var cdb = new CmpDb(CmpDbConnectionString);
            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.ContactingVM.ToString(), true);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessContactVm() " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }

            foreach (var vmReq in vmReqList)
            {
                try
                {
                    string remotePsUrl;
                    PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility rpcPortVisibility;

                    //*** Ignore for now
                    try
                    {
                        if (DisableSmartCardAuth)
                            AzureAdminClientLib.VmOps.DisableSmartCardAuth(vmReq.TargetVmName);
                    }
                    catch (Exception)
                    {
                    }

                    //AssertIfTimedOut(vmReq, _ContactingVmMinutesTTL, Constants.StatusEnum.ContactingVM.ToString());
                    var ppidr = TestPsConnection(vmReq, out remotePsUrl, out rpcPortVisibility);

                    if (Constants.PostProvInintDisksResultEnum.Success != ppidr)
                    {
                        if (HasTimedOut(vmReq, ContactingVmMinutesTTL))
                        {
                            DeleteVm(vmReq, DeleteDwelltimeMinutes, true, false);
                            ResubmitRequest(vmReq, "Timeout " + Constants.StatusEnum.ContactingVM, cdb, false, false);
                        }

                        continue;
                    }

                    if (null != remotePsUrl)
                    {
                        try
                        {
                            var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);
                            var uri = new System.Uri(remotePsUrl);

                            if (null == vmc.InfoFromVM)
                                vmc.InfoFromVM = new CmpInterfaceModel.Models.InfoFromVmSpec();

                            if (null == vmc.PostInfoFromVM)
                                vmc.PostInfoFromVM = new CmpInterfaceModel.Models.PostInfoFromVmSpec();

                            vmc.InfoFromVM.VmAddress = uri.Host;
                            vmc.PostInfoFromVM.VmAddress = uri.Host;

                            /*string POSH_DOMAIN = "(Get-WmiObject Win32_ComputerSystem).Domain";

                            //get data by querying the WMI as we cann't depend on the Active directory in this context 
                            vmc.InfoFromVM.MachineDomain = GetPowerShellDataGathering(vmReq, POSH_DOMAIN);*/

                            //UnReserveStaticIpAddress(uri.Host);

                            if (rpcPortVisibility ==
                                PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.PublicHttps)
                            {
                                var cmpS = new CmpService(EventLog, CmpDbConnectionString, AftsDbConnectionString);
                                var rdpPort = cmpS.FetchServicePort(vmReq.ID, "RDP");

                                if (0 < rdpPort)
                                {
                                    vmc.InfoFromVM.VmAddress += ":" + rdpPort;
                                    vmc.PostInfoFromVM.VmAddress += ":" + rdpPort;
                                }
                            }

                            vmReq.Config = vmc.Serialize();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Unable to acquire VM address : " +
                                Utilities.UnwindExceptionMessages(ex));
                        }
                    }

                    vmReq.CurrentStateStartTime = DateTime.UtcNow;
                    vmReq.ExceptionMessage = "";
                    vmReq.StatusMessage = "Contacted VM";
                    vmReq.StatusCode = Constants.StatusEnum.MovingPagefile.ToString();
                    cdb.SetVmDepRequestStatus(vmReq, null);
                }
                catch (Exception ex)
                {
                    if (null != vmReq)
                    {
                        vmReq.CurrentStateStartTime = DateTime.UtcNow;
                        vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                        vmReq.ExceptionMessage = Utilities.UnwindExceptionMessages(ex);
                        Utilities.SetVmReqExceptionType(vmReq,
                            CmpInterfaceModel.Constants.RequestExceptionTypeCodeEnum.Admin);
                        cdb.SetVmDepRequestStatus(vmReq, null);
                    }
                }
            }

            return 0;
        }
    }
}