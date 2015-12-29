using System;
using System.Collections.Generic;
using CmpInterfaceModel;

namespace CmpServiceLib.Stages
{
    public class UploadServiceCertificateStage : Stage
    {
        public int HostedServiceCreationDwellTime { get; set; }

        public override object Execute()
        {
            if (AllQueuesBlocked)
                return 0;

            var noCerts = true;
            var HaltSequence = false;
            var cdb = new CmpDb(CmpDbConnectionString);

            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.ReadyForUploadingServiceCert.ToString(), true);
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
                    var vmc =
                        CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                    noCerts = true;

                    if (null != vmc.ServiceCertList)
                        foreach (var cf in vmc.ServiceCertList)
                        {
                            if (null == cf.Data)
                                continue;

                            if (1 > cf.Data.Length)
                                continue;

                            noCerts = false;

                            if (null == vmReq.ServiceProviderAccountID)
                                throw new Exception("ServiceProviderAccountID == NULL");

                            var connection =
                                ServProvAccount.GetAzureServiceAccountConnection((int)vmReq.ServiceProviderAccountID, CmpDbConnectionString);

                            AzureAdminClientLib.HttpResponse resp = null;

                            var co = new AzureAdminClientLib.CertOps(connection);
                            resp = co.UploadServiceCert(vmReq.TargetServicename, cf.Data, cf.Password);

                            if (resp.HadError)
                            {
                                vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                                vmReq.StatusMessage = "Exception";
                                vmReq.ExceptionMessage = resp.Body;
                                Utilities.SetVmReqExceptionType(vmReq,
                                    CmpInterfaceModel.Constants.RequestExceptionTypeCodeEnum.Admin);
                            }
                            else
                            {
                                vmReq.StatusCode = Constants.StatusEnum.ReadyForCreatingVM.ToString();
                                vmReq.ExceptionMessage = "";
                                vmReq.StatusMessage = resp.Body;
                                vmReq.ServiceProviderStatusCheckTag = resp.StatusCheckUrl;
                                System.Threading.Thread.Sleep(HostedServiceCreationDwellTime);
                            }

                            cdb.SetVmDepRequestStatus(vmReq, null);
                        }

                    if (noCerts)
                    {
                        vmReq.StatusCode = Constants.StatusEnum.ReadyForCreatingVM.ToString();
                        vmReq.ExceptionMessage = "";
                        vmReq.StatusMessage = "No Certificates Specified";
                        vmReq.ServiceProviderStatusCheckTag = "";
                        cdb.SetVmDepRequestStatus(vmReq, null);
                    }

                    if (!HaltSequence)
                        vmReq.CurrentStateStartTime = DateTime.UtcNow;

                }
                catch (Exception ex)
                {
                    if (null != vmReq)
                    {
                        vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                        vmReq.ExceptionMessage = Utilities.UnwindExceptionMessages(ex);

                        if (vmReq.ExceptionMessage.Contains("Unable to locate VM request record"))
                        {
                        }
                        else
                        {
                            vmReq.StatusMessage = "Exception";
                            Utilities.SetVmReqExceptionType(vmReq,
                                CmpInterfaceModel.Constants.RequestExceptionTypeCodeEnum.Admin);
                            cdb.SetVmDepRequestStatus(vmReq, null);
                        }
                    }
                }
            }

            return 0;
        }
    }
}