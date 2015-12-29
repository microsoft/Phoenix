using System;
using System.Collections.Generic;
using CmpInterfaceModel;
using CmpInterfaceModel.Models;
using VmDeploymentRequest = CmpServiceLib.Models.VmDeploymentRequest;

namespace CmpServiceLib.Stages
{
    public class CreateVmStage : Stage
    {
        public int CreateVmMinutesTtl { get; set; }

        public int DeleteDwelltimeMinutes { get; set; }

        public Action<VmDeploymentRequest, int, bool, bool> DeleteVm { get; set; }

        public Func<VmDeploymentRequest, int, bool> HasTimedOut { get; set; }

        public Func<int?, int> NonNull { get; set; } 

        public Action<VmDeploymentRequest, string, CmpDb, bool, bool> ResubmitRequest { get; set; }

        public override object Execute()
        {
            if (AllQueuesBlocked)
                return 0;

            var appIdList = new HashSet<string>();

            var cdb = new CmpDb(CmpDbConnectionString);
            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.ReadyForCreatingVM.ToString(), true);
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
                    var vmCfg = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                    //*** Which Azure API are we looking at here? ***
                    if (null == vmCfg.AzureApiConfig)
                        vmCfg.AzureApiConfig = null == vmCfg.AzureArmConfig
                            ? new AzureApiSpec() { Platform = Constants.AzureApiType.RDFE.ToString() }
                            : new AzureApiSpec() { Platform = Constants.AzureApiType.ARM.ToString() };

                    //*** Don't try ops on the same service twice in the same run ***
                    if (vmCfg.AzureApiConfig.Platform.Equals(Constants.AzureApiType.RDFE.ToString()))
                    {
                        if (appIdList.Contains(vmReq.ParentAppID))
                            continue;

                        appIdList.Add(vmReq.ParentAppID);
                    }

                    //*** Set this now to create a timestamp for subsequent timeout test ***
                    if (!vmReq.StatusMessage.Equals("Submitting VM Request to Azure") &
                        !vmReq.StatusMessage.Contains("Retry") &
                        !vmReq.StatusMessage.Contains("Certificates"))
                    {
                        vmReq.StatusMessage = "Submitting VM Request to Azure";
                        vmReq.CurrentStateStartTime = DateTime.UtcNow;
                        cdb.SetVmDepRequestStatus(vmReq, null);
                    }

                    if (null == vmReq.OverwriteExisting)
                        vmReq.OverwriteExisting = false;

                    var haltSequence = false;
                    var retry = false;

                    var vmDepReq = new CmpInterfaceModel.Models.VmDeploymentRequest
                    {
                        AftsID = Convert.ToInt32(vmReq.AftsID),
                        ID = vmReq.ID,
                        Active = Convert.ToBoolean(vmReq.Active),
                        Config = vmReq.Config,
                        ExceptionMessage = vmReq.ExceptionMessage,
                        LastStatusUpdate = Convert.ToDateTime(vmReq.LastStatusUpdate),
                        ParentAppID = vmReq.ParentAppID,
                        ParentAppName = vmReq.ParentAppName,
                        RequestDescription = vmReq.RequestDescription,
                        RequestName = vmReq.RequestName,
                        RequestType = vmReq.RequestType,
                        SourceServerName = vmReq.SourceServerName,
                        SourceVhdFilesCSV = vmReq.SourceVhdFilesCSV,
                        Status = vmReq.StatusCode,
                        StatusMessage = vmReq.StatusMessage,
                        TagData = vmReq.TagData,
                        TargetAccount = vmReq.TargetAccount,
                        TargetAccountCreds = vmReq.TargetAccountCreds,
                        TargetAccountType = vmReq.TargetAccountType,
                        TargetLocation = vmReq.TargetLocation,
                        TargetLocationType = vmReq.TargetLocationType,
                        TargetServiceName = vmReq.TargetServicename,
                        TargetServiceProviderType = vmReq.TargetServiceProviderType,
                        TargetVmName = vmReq.TargetVmName,
                        VmSize = vmReq.VmSize,
                        WhenRequested = Convert.ToDateTime(vmReq.WhenRequested),
                        WhoRequested = vmReq.WhoRequested,
                        ExceptionTypeCode = vmReq.ExceptionTypeCode,
                        SourceServerRegion = vmReq.SourceServerRegion,
                        TargetServiceProviderAccountID = NonNull(vmReq.ServiceProviderAccountID),
                        TagID = NonNull(vmReq.TagID),
                        ValidationResults = vmReq.ValidationResults
                    };

                    if (null == vmReq.ServiceProviderAccountID)
                        throw new Exception("ServiceProviderAccountID == NULL");

                    var connection = ServProvAccount.GetAzureServiceAccountConnection(
                        (int)vmReq.ServiceProviderAccountID, CmpDbConnectionString);

                    if (null == connection)
                        throw new Exception("Unable to locate account for given ServiceProviderAccountID");

                    //*** Send request to the service ***

                    var vmo = new AzureAdminClientLib.VmOps(connection);
                    var resp = vmo.CreateVm(vmDepReq);

                    if (resp.HadError)
                    {
                        if (HasTimedOut(vmReq, CreateVmMinutesTtl))
                        {
                            ResubmitRequest(vmReq, "Resubmit on CreateVM timeout : " + resp.Body, cdb, false, false);
                            appIdList.Remove(vmReq.ParentAppID);
                            continue;
                        }

                        if (resp.Body.ToLower().Contains("(503)"))
                        {
                            //*** Back off, let it time out
                            continue;
                        }

                        if (resp.Body.ToLower().Contains("(500)"))
                        {
                            vmReq.StatusMessage = "Retry on '(500)' : " + resp.Body;
                            cdb.SetVmDepRequestStatus(vmReq, null);
                            appIdList.Remove(vmReq.ParentAppID);
                            continue;
                        }

                        if (resp.Body.ToLower().Contains("retry"))
                        {
                            vmReq.StatusMessage = "Retry on 'Please retry the request' : " + resp.Body;
                            cdb.SetVmDepRequestStatus(vmReq, null);
                            continue;
                        }

                        if (resp.Body.Contains("requires exclusive access"))
                        {
                            vmReq.StatusMessage = "Retry on 'requires exclusive access' : " + resp.Body;
                            cdb.SetVmDepRequestStatus(vmReq, null);
                            continue;
                        }

                        if (resp.Body.Contains("another operation is pending"))
                        {
                            vmReq.StatusMessage = "Retry on 'another operation is pending' : " + resp.Body;
                            cdb.SetVmDepRequestStatus(vmReq, null);
                            continue;
                        }

                        if (resp.Body.Contains("already exists") & (bool)vmReq.OverwriteExisting)
                        {
                            DeleteVm(vmReq, DeleteDwelltimeMinutes, true, true);
                            ResubmitRequest(vmReq, "Resubmit on 'VM already exists in the deployment'", cdb, false, false);
                        }

                        if (resp.Body.Contains("is occupied"))
                        {
                            DeleteVm(vmReq, DeleteDwelltimeMinutes, true, true);
                            ResubmitRequest(vmReq, "Resubmit on 'The specified deployment slot Production is occupied'", cdb, false, false);
                        }

                        vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                        vmReq.StatusMessage = "CreateVM Exception : " + resp.Body;
                        Utilities.SetVmReqExceptionType(vmReq,
                            CmpInterfaceModel.Constants.RequestExceptionTypeCodeEnum.Admin);

                        vmReq.ExceptionMessage = "CreateVM Exception : " + resp.Body;
                        haltSequence = true;
                    }
                    else
                    {
                        vmReq.StatusCode = Constants.StatusEnum.CreatingVM.ToString();
                        vmReq.StatusMessage = "CreateVM Request Accepted : " + resp.Body;
                        vmReq.ExceptionMessage = "";
                        vmReq.ServiceProviderStatusCheckTag = resp.StatusCheckUrl;
                    }

                    cdb.SetVmDepRequestStatus(vmReq, null);

                    if (!haltSequence)
                        vmReq.CurrentStateStartTime = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("(503)"))
                        continue;

                    if (null != vmReq)
                    {
                        vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                        vmReq.StatusMessage = "Exception in ProcessCreateVm() : " +
                            Utilities.UnwindExceptionMessages(ex);
                        vmReq.ExceptionMessage = vmReq.StatusMessage;
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