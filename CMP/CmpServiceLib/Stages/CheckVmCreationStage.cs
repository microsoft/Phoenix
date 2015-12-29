using System;
using System.Collections.Generic;
using AzureAdminClientLib;
using CmpInterfaceModel;
using CmpServiceLib.Models;

namespace CmpServiceLib.Stages
{
    public class CheckVmCreationStage : Stage
    {
        public int ContactingVmMinutesTTL { get; set; }

        public int DeleteDwelltimeMinutes { get; set; }

        public Action<VmDeploymentRequest, int, bool, bool> DeleteVm { get; set; }

        public Func<VmDeploymentRequest, Constants.OsFamily> GetOsFamily { get; set; } 

        public Func<VmDeploymentRequest, int, bool> HasTimedOut { get; set; }

        public Action<CmpDb, string, Constants.AssetTypeCodeEnum, string, string, string> MarkBadAsset { get; set; }

        public Action<VmDeploymentRequest, string, CmpDb, bool, bool> ResubmitRequest { get; set; }

        public Action<VmDeploymentRequest, List<string>, CmpDb> CompleteVmRequest { get; set; }

        private void SetVmIpAddress(VmDeploymentRequest vmReq, Connection connection)
        {
            var tempHostName = vmReq.TargetVmName;
            string ipAddress2;

            var remotePsUrl = PowershellLib.VirtualMachineRemotePowerShell.GetPowerShellUrl(
                connection.SubcriptionID, connection.Certificate, connection.AdToken,
                vmReq.TargetServicename, tempHostName,
                PowershellLib.VirtualMachineRemotePowerShell.RpcPortVisibility.PublicHttps, out ipAddress2);


            var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);
            var uri = new System.Uri(remotePsUrl);

            if (null == vmc.InfoFromVM)
                vmc.InfoFromVM = new CmpInterfaceModel.Models.InfoFromVmSpec();

            if (null == vmc.PostInfoFromVM)
                vmc.PostInfoFromVM = new CmpInterfaceModel.Models.PostInfoFromVmSpec();

            //vmc.InfoFromVM.VmAddress = uri.Host;
            //vmc.PostInfoFromVM.VmAddress = uri.Host;

            vmc.InfoFromVM.VmAddress = ipAddress2;
            vmc.PostInfoFromVM.VmAddress = ipAddress2;

            vmReq.Config = vmc.Serialize();
        }


        public override object Execute()
        {
            if (AllQueuesBlocked)
                return 0;

            var cdb = new CmpDb(CmpDbConnectionString);
            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.CreatingVM.ToString(), true);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CheckVmCreation() " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }

            foreach (var vmReq in vmReqList)
            {
                try
                {
                    //AssertIfTimedOut(vmReq, 0, Constants.StatusEnum.CreatingVM.ToString());

                   var connection =
                        ServProvAccount.GetAzureServiceAccountConnection(
                        Convert.ToInt32(vmReq.ServiceProviderAccountID), CmpDbConnectionString);

                    var vmo = new AzureAdminClientLib.VmOps(connection);

                    var resp =
                        vmo.CheckVmProvisioningStatus(vmReq.ServiceProviderStatusCheckTag, vmReq.Config);

                    if (resp.HadError)
                    {
                        if (resp.Retry)
                            continue;

                        if (resp.Body.Contains("(503)"))
                            continue;

                        if (resp.Body.Contains("retry"))
                            continue;

                        if (resp.Body.Contains("try again"))
                            continue;

                        vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                        vmReq.ExceptionMessage = "Error in CheckCheckVmCreation() : " + resp.Body;
                        Utilities.SetVmReqExceptionType(vmReq,
                            CmpInterfaceModel.Constants.RequestExceptionTypeCodeEnum.Admin);
                        cdb.SetVmDepRequestStatus(vmReq, null);
                    }
                    else
                    {
                        var stat = new AzureAdminTaskStatus();

                        switch (resp.ProviderRequestState)
                        {
                            case "Succeeded":
                                stat.Result = AzureAdminClientLib.AzureAdminTaskStatus.ResultEnum.Success;
                                stat.Status = "Success";
                                break;

                            case "Running":
                                stat.Result = AzureAdminClientLib.AzureAdminTaskStatus.ResultEnum.Success;
                                stat.Status = "InProgress";
                                break;

                            case "Failed":
                                stat = new AzureAdminClientLib.AzureAdminTaskStatus(resp.Body, vmReq.RequestType);
                                break;

                            default:
                                stat = new AzureAdminClientLib.AzureAdminTaskStatus(resp.Body, vmReq.RequestType);
                                stat.Result = AzureAdminTaskStatus.ResultEnum.Failed;
                                
                                if (null == stat.ErrorMessage)
                                    stat.ErrorMessage = "---";
                                else if( 0 == stat.ErrorMessage.Length)
                                    stat.ErrorMessage = "---";

                                if (null == stat.ErrorCode)
                                    stat.ErrorCode = "---";
                                else if (0 == stat.ErrorCode.Length)
                                    stat.ErrorCode = "---";

                                break;
                        }
                        
                        switch (stat.Result)
                        {
                            case AzureAdminClientLib.AzureAdminTaskStatus.ResultEnum.Success:
                                if (stat.Status.Equals("InProgress"))
                                {
                                    vmReq.StatusCode = Constants.StatusEnum.CreatingVM.ToString();

                                    if (HasTimedOut(vmReq, ContactingVmMinutesTTL))
                                    {
                                        DeleteVm(vmReq, DeleteDwelltimeMinutes, true, false);
                                        ResubmitRequest(vmReq, "Timeout " + Constants.StatusEnum.CreatingVM, cdb, false, false);
                                    }

                                    continue;
                                }

                                //*** Do we need to perform post processing?
                                if (Utilities.PostProcessingEnabled)
                                {
                                    //*** Is it a Windows deployment? ***
                                    vmReq.StatusCode = GetOsFamily(vmReq) == Constants.OsFamily.Windows
                                        ? Constants.StatusEnum.ContactingVM.ToString()
                                        : Constants.StatusEnum.StartingSequences.ToString();
                                }
                                else
                                {
                                    //*** get the VM IP address
                                    SetVmIpAddress(vmReq, connection);

                                    //*** and complete the request
                                    CompleteVmRequest(vmReq, null, cdb);
                                    continue;
                                }

                                vmReq.CurrentStateStartTime = DateTime.UtcNow;

                                vmReq.StatusMessage = "VM Created"; //*** TODO * Do we need more detail?
                                vmReq.ExceptionMessage = "";
                                cdb.SetVmDepRequestStatus(vmReq, null);
                                break;

                            case AzureAdminClientLib.AzureAdminTaskStatus.ResultEnum.Failed:

                                if (stat.ErrorMessage.ToLower().Contains("(503)"))
                                    continue;

                                if (stat.ErrorMessage.ToLower().Contains("retry"))
                                    continue;

                                if (stat.ErrorMessage.ToLower().Contains("unable to upgrade"))
                                {
                                    if (HasTimedOut(vmReq, ContactingVmMinutesTTL))
                                    {
                                        MarkBadAsset(cdb, vmReq.TargetServicename,
                                            CmpInterfaceModel.Constants.AssetTypeCodeEnum.Hostservice,
                                            stat.ErrorMessage, "VmRequest.ID = " + vmReq.ID, "CmpWorkerService");

                                        DeleteVm(vmReq, DeleteDwelltimeMinutes, true, false);
                                        ResubmitRequest(vmReq, "Timeout " + Constants.StatusEnum.CreatingVM, cdb, false, false);
                                    }

                                    continue;
                                }

                                if (stat.ErrorMessage.ToLower().Contains("try again"))
                                    continue;

                                if (stat.ErrorMessage.Contains("Unsupported number of roles"))
                                {
                                    ResubmitRequest(vmReq, "Service role instance limit exceeded", cdb, false, false);
                                    continue;
                                }

                                if (stat.ErrorMessage.Contains("lease conflict occurred"))
                                {
                                    ResubmitRequest(vmReq, stat.ErrorMessage, cdb, false, false);
                                    continue;
                                }

                                vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                                vmReq.ExceptionMessage = "Exception in CheckCheckVmCreation() : Code: '" +
                                    stat.ErrorCode + "', Message: " + stat.ErrorMessage;
                                Utilities.SetVmReqExceptionType(vmReq,
                                     CmpInterfaceModel.Constants.RequestExceptionTypeCodeEnum.Admin);
                                cdb.SetVmDepRequestStatus(vmReq, null);

                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("(503)"))
                        continue;

                    if (null != vmReq)
                    {
                        vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                        vmReq.ExceptionMessage = "Exception in CheckCheckVmCreation() : " +
                            Utilities.UnwindExceptionMessages(ex);
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