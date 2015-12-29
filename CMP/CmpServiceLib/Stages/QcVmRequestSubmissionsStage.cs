using System;
using System.Collections.Generic;
using CmpInterfaceModel;

namespace CmpServiceLib.Stages
{
    public class QcVmRequestSubmissionsStage : Stage
    {
        public override object Execute()
        {
            var cdb = new CmpDb(CmpDbConnectionString);

            if (AllQueuesBlocked)
                return 0;

            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.QcVmRequest.ToString(), true);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessQcVmRequestSubmissions() " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }

            foreach (var vmReq in vmReqList)
            {
                try
                {
                    if (vmReq.RequestType.Equals(Constants.RequestTypeEnum.NewVM.ToString()))
                        continue;

                    vmReq.CurrentStateStartTime = DateTime.UtcNow;

                    cdb.SetVmDepRequestStatus(vmReq, null);
                }
                catch (Exception ex)
                {
                    vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                    vmReq.ExceptionMessage = "Exception in ProcessQcVmRequestSubmissions() " +
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