using System;
using System.Collections.Generic;
using Constants = CmpInterfaceModel.Constants;

namespace CmpServiceLib.Stages
{
    public class TransferredSubmissionsStage : Stage
    {
        #region BuildMigratedVmConfigString

        public delegate Constants.BuildMigratedVmConfigStringResultEnum BuildMigratedVmConfigStringDelegate(
            Models.VmDeploymentRequest vmReq, out string vmConfigString);

        public BuildMigratedVmConfigStringDelegate BuildMigratedVmConfigString { get; set; }

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
                    Constants.StatusEnum.Transferred.ToString(), true);
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
                    //*** Experiment with dwell time ***
                    if (null != vmReq.CurrentStateStartTime)
                        if (2 > DateTime.UtcNow.Subtract(((DateTime)vmReq.CurrentStateStartTime)).TotalMinutes)
                            continue;

                    //*** verify that the migrated blobs exist ***

                    string vmReqConfig;
                    if (BuildMigratedVmConfigString(vmReq, out vmReqConfig) ==
                        Constants.BuildMigratedVmConfigStringResultEnum.MissingDisk)
                    {
                        if (null != vmReq.CurrentStateStartTime)
                            if (10 < DateTime.UtcNow.Subtract(((DateTime)vmReq.CurrentStateStartTime)).TotalMinutes)
                                throw new Exception(vmReqConfig);

                        continue;
                    }

                    vmReq.Config = vmReqConfig;

                    vmReq.StatusMessage = "Migration config created";
                    vmReq.StatusCode = Constants.StatusEnum.QcVmRequestPassed.ToString();

                    vmReq.CurrentStateStartTime = DateTime.UtcNow;
                    cdb.SetVmDepRequestStatus(vmReq, null);
                }
                catch (Exception ex)
                {
                    vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                    vmReq.ExceptionMessage = "Exception in ProcessTransferedSubmissions() " +
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