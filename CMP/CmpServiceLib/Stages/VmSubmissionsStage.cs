using System;
using System.Collections.Generic;
using CmpInterfaceModel;

namespace CmpServiceLib.Stages
{
    public class VmSubmissionsStage : Stage
    {
        public Action<CmpDb> ReadConfigValues { get; set; }

        public bool SubmittedQueueBlocked { get; set; }

        public override object Execute()
        {
            var cdb = new CmpDb(CmpDbConnectionString);
            ReadConfigValues(cdb);

            if (SubmittedQueueBlocked)
                return 0;

            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.Submitted.ToString(), true);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessVmSubmissions() " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }

            foreach (var vmReq in vmReqList)
            {
                try
                {
                    vmReq.CurrentStateStartTime = DateTime.UtcNow;

                    if (vmReq.RequestType.Equals(Constants.RequestTypeEnum.NewVM.ToString()))
                    {
                        if (null == vmReq.CurrentStateTryCount)
                            vmReq.CurrentStateTryCount = 0;

                        /*if (0 == vmReq.CurrentStateTryCount)
                            if(AzureAdminClientLib.VmOps.DoesNameResolve(vmReq.TargetVmName))
                                throw new Exception(string.Format(
                                    "A DNS record for a server with name '{0}' already exists. Duplicates not allowed.", vmReq.TargetVmName));*/

                        vmReq.StatusCode = Constants.StatusEnum.QcVmRequest.ToString();
                        cdb.SetVmDepRequestStatus(vmReq, null);
                    }
                }
                catch (Exception ex)
                {
                    vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                    vmReq.ExceptionMessage = "Exception in ProcessVmSubmissions() " +
                        Utilities.UnwindExceptionMessages(ex);
                    Utilities.SetVmReqExceptionType(vmReq,
                        Constants.RequestExceptionTypeCodeEnum.Admin);
                    cdb.SetVmDepRequestStatus(vmReq, null);
                }
            }

            return 0;
        }
    }
}