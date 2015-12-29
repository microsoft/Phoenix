using System;
using System.Collections.Generic;
using CmpInterfaceModel;

namespace CmpServiceLib.Stages
{
    public class ReadyForTransferSubmissionsStage : Stage
    {
        public string AftsDbConnectionString { get; set; }

        public override object Execute()
        {
            if (AllQueuesBlocked)
                return 0;

            var cdb = new CmpDb(CmpDbConnectionString);
            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.ReadyForTransfer.ToString(), true);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessReadyForTransferSubmissions() " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }

            foreach (var vmReq in vmReqList)
            {
                try
                {
                    //*** Check status of AFTS job ***

                    string message;
                    var adb = new AftsDb(AftsDbConnectionString);
                    var transferStatus = adb.FetchTransferStatus(vmReq.ID, out message);

                    switch (transferStatus)
                    {
                        case Constants.StatusEnum.Submitted:
                            continue;
                        //break;
                        case Constants.StatusEnum.Transferring:
                            vmReq.StatusCode = Constants.StatusEnum.Transferring.ToString();
                            vmReq.StatusMessage = Constants.StatusEnum.Transferring.ToString();
                            break;
                        case Constants.StatusEnum.Complete:
                            vmReq.StatusCode = Constants.StatusEnum.Transferred.ToString();
                            vmReq.StatusMessage = Constants.StatusEnum.Transferred.ToString();
                            break;
                        case Constants.StatusEnum.Exception:
                            vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                            vmReq.StatusMessage = "Transfer Exception";
                            vmReq.ExceptionMessage = message;
                            break;
                    }

                    vmReq.CurrentStateStartTime = DateTime.UtcNow;
                    cdb.SetVmDepRequestStatus(vmReq, null);
                }
                catch (Exception ex)
                {
                    vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                    vmReq.ExceptionMessage = "Exception in ProcessReadyForTransferSubmissions() " +
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