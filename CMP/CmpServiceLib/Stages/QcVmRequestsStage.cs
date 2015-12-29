using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CmpInterfaceModel;
using CmpInterfaceModel.Models;
using VmDeploymentRequest = CmpServiceLib.Models.VmDeploymentRequest;

namespace CmpServiceLib.Stages
{
    public class QcVmRequestsStage : Stage
    {
        public string AftsDbConnectionString { get; set; }

        public int DefaultCoreSafetyStockValue { get; set; }

        public Func<VmDeploymentRequest, string> GetTargetServiceProviderAccountResourceGroup { get; set; }

        public bool PerformValidation { get; set; }

        public Func<VmDeploymentRequest, CmpInterfaceModel.Models.VmDeploymentRequest> ServToInt { get; set; }

        public override object Execute()
        {
            if (AllQueuesBlocked)
                return 0;

            var cdb = new CmpDb(CmpDbConnectionString);
            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.QcVmRequest.ToString(), true);
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
                    if (null == vmReq)
                    {
                        LogThis(new Exception("Unable to fetch body content from queue"),
                            EventLogEntryType.Error, "Exception in ProcessQcVmRequests", 1, 1);
                        continue;
                    }

                    var vmc =
                        CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                    if (null == vmc)
                    {
                        vmReq.StatusMessage = "VmDeploymentRequest(" + vmReq.ID + ").Config could not de deserialized";
                        vmReq.StatusCode = Constants.StatusEnum.Rejected.ToString();
                        cdb.SetVmDepRequestStatus(vmReq, null);

                        continue;
                    }

                    if (!PerformValidation)
                    {
                        if (null == vmc.ValidationConfig)
                            vmc.ValidationConfig = new ValidateSpec();

                        vmc.ValidationConfig.Validate = false;
                    }

                    if (null != vmc.ValidationConfig)
                        if (false == vmc.ValidationConfig.Validate)
                        {
                            if (vmReq.RequestType.Equals(Constants.RequestTypeEnum.NewVM.ToString()))
                            {
                                //*** for net new, update the status and move to the next queue ***
                                vmReq.StatusMessage = "Validation Bypassed";
                                vmReq.StatusCode = Constants.StatusEnum.QcVmRequestPassed.ToString();
                            }
                            else
                            {
                                //*** for migrations, update the status but don't move to the next queue ***
                                vmReq.StatusMessage = "Validation Bypassed, ready for migration";
                                vmReq.StatusCode = Constants.StatusEnum.ReadyForConversion.ToString();

                                //*** Update the VmMigrationRequest status ***
                                var cmpS = new CmpService(EventLog, CmpDbConnectionString, AftsDbConnectionString);
                                cmpS.UpdateVmMigrationRequest(vmReq, cdb);
                            }
                        }

                    //complete the processing of the message
                    cdb.SetVmDepRequestStatus(vmReq, null);
                }
                catch (Exception ex)
                {
                    Utilities.SetVmReqExceptionType(vmReq,
                        CmpInterfaceModel.Constants.RequestExceptionTypeCodeEnum.Admin);

                    LogThis(ex, EventLogEntryType.Error, "Exception in ProcessQcVmRequests", 1, 1);
                }
            }

            return 0;
        }
    }
}