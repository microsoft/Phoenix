using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using AzureAdminClientLib;

//*****************************************************************************
//
//
//*****************************************************************************
using CmpInterfaceModel;
using CmpServiceLib.Models;

namespace CmpServiceLib
{
    public class CmpDb
    {
        private string _ConnectionString = null;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// 
        //*********************************************************************

        private void LogThis(string message, Exception ex)
        {

        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// 
        //*********************************************************************

        public CmpDb(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public string FetchConfigValue(string name, bool isActive)
        {
            var vmrList = new List<Models.VmDeploymentRequest>();

            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var val =
                        db.Configs.Where(rb => (rb.Name == name & rb.IsActive == isActive));

                    if (!val.Any())
                        return null;

                    return val.First().Value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpDb.FetchConfigValue() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// 
        //*********************************************************************

        public void SetConfigValue(string name, string value)
        {
            var vmrList = new List<Models.VmDeploymentRequest>();

            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var settings =
                        db.Configs.Where(rb => (rb.Name == name));

                    if (!settings.Any())
                        return;

                    var setting = settings.First();

                    setting.Value = value;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpDb.SetConfigValue() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        //*** FluRequest Region ***********************************************
        //*********************************************************************

        #region FluRequest Region

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.FluRequest> FetchFluRequests()
        {
            List<Models.FluRequest> fluReqList = new List<Models.FluRequest>();

            using (var db = new Models.CMPContext())
            {
                db.Database.Connection.ConnectionString = _ConnectionString;

                var fluReqQ = db.FluRequests.OrderBy(rb => rb.ID);

                fluReqList.AddRange(fluReqQ);

                return fluReqList;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.FluRequest> FetchFluRequests(string Status, bool active)
        {
            List<Models.FluRequest> FluReqList = new List<Models.FluRequest>();

            using (var db = new Models.CMPContext())
            {
                db.Database.Connection.ConnectionString = _ConnectionString;

                var fluReqQ =
                    db.FluRequests.Where(rb => (rb.Status == Status & rb.Active == active)).OrderBy(rb => rb.ID);

                FluReqList.AddRange(fluReqQ);

                return FluReqList;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private int FetchMaxFluRequestID(Models.CMPContext db)
        {
            int MaxID = 0;

            try
            {
                MaxID = (from RB in db.FluRequests
                    select RB.ID).Max();
            }
            catch (InvalidOperationException)
            {
                MaxID = 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchMaxFluRequestID() : " + ex.Message);
            }

            return MaxID;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fluRequest"></param>
        /// 
        //*********************************************************************

        public void SetFluRequestStatus(Models.FluRequest fluRequest)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var foundReqList = db.FluRequests.Where(vmr => vmr.ID == fluRequest.ID);

                    if (!foundReqList.Any())
                    {
                        LogThis("SetFluRequestStatus() : Unable to locate FLU request record: ID: "
                                + fluRequest.ID, null);
                        throw new Exception("Unable to locate FLU request record: ID: "
                                            + fluRequest.ID);
                    }
                    else
                    {
                        var foundReq = foundReqList.First();

                        //FoundReq.AftsID = fluRequest.AftsID;
                        foundReq.Status = fluRequest.Status;
                        foundReq.ExceptionMessage = fluRequest.ExceptionMessage;
                        foundReq.SourceVhdFilesCSV = fluRequest.SourceVhdFilesCSV;
                        foundReq.LastStatusUpdate = DateTime.UtcNow;

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in SetFluRequestStatus() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fluRequest"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public Models.FluRequest InsertFluRequest(Models.FluRequest fluRequest)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    fluRequest.ID = FetchMaxFluRequestID(db) + 1;

                    db.FluRequests.Add(fluRequest);
                    db.SaveChanges();

                    return fluRequest;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in InsertFluRequest() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        //*********************************************************************
        //*** VmDeploymentRequest Region **************************************
        //*********************************************************************

        #region VmDeploymentRequest Region

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.VmDeploymentRequest> FetchVmDepRequests(string state, 
            DateTime startDate, DateTime endDate)
        {
            var vmrList = new List<Models.VmDeploymentRequest>();

            try
            {
                using (var db = new Models.CMPContext())
                {
                    startDate = startDate.AddDays(-1);
                    endDate = endDate.AddDays(1);

                    db.Database.Connection.ConnectionString = _ConnectionString;

                    IOrderedQueryable<VmDeploymentRequest> vmrQ;

                    if (null == state)
                        vmrQ = db.VmDeploymentRequests.Where(rb => 
                            (rb.WhenRequested > startDate & rb.WhenRequested < endDate))
                                .OrderBy(RB => RB.ID);
                    else if (state.Equals("In Process", StringComparison.InvariantCulture))
                        vmrQ = db.VmDeploymentRequests.Where(rb =>
                            (rb.StatusCode != "Complete" & rb.StatusCode != "Exception" & rb.StatusCode != "Rejected" &
                            rb.WhenRequested > startDate & rb.WhenRequested < endDate))
                                .OrderBy(RB => RB.ID);
                    else if (state.Equals("Not Complete", StringComparison.InvariantCulture))
                        vmrQ = db.VmDeploymentRequests.Where(rb =>
                            (rb.StatusCode != "Complete" &
                            rb.WhenRequested > startDate & rb.WhenRequested < endDate))
                                .OrderBy(RB => RB.ID);
                    else
                        vmrQ = db.VmDeploymentRequests.Where(rb => 
                            (rb.StatusCode == state & rb.WhenRequested > startDate & rb.WhenRequested < endDate))
                                .OrderBy(RB => RB.ID);

                    vmrList.AddRange(vmrQ);

                    return vmrList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVmDepRequests() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.VmDeploymentRequest> FetchVmDepRequests(string status, bool active)
        {
            var vmrList = new List<Models.VmDeploymentRequest>();

            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Configuration.LazyLoadingEnabled = false;                   
                    db.Database.Connection.ConnectionString = _ConnectionString;                    

                    IOrderedQueryable<VmDeploymentRequest> vmrQ ;

                    if (null != status)
                        if (0 == status.Length) 
                            status = null;

                    if (null == status)
                        vmrQ = db.VmDeploymentRequests.Where(rb => (rb.Active == active))
                                .OrderBy(rb => rb.ID);
                    else
                        vmrQ = db.VmDeploymentRequests.Where(rb => (rb.StatusCode == status & rb.Active == active))
                                .OrderBy(rb => rb.ID);

                    vmrQ.Load();                    
                    vmrList.AddRange(vmrQ);
                }
                    return vmrList;
                }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVmDepRequests() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.VmDeploymentRequest> FetchVmDepRequestsByVmName(string vmName)
        {
            var vmrList = new List<Models.VmDeploymentRequest>();

            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var vmrQ = db.VmDeploymentRequests.Where(rb => (rb.TargetVmName == vmName))
                        .OrderBy(rb => rb.ID);

                    vmrList.AddRange(vmrQ);

                    return vmrList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVmDepRequestsByVmName() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deploymentRequestId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************
        public bool CheckVmDepRequest(int deploymentRequestId)
        {
            Models.CMPContext db = null;
            try
            {
                db = new Models.CMPContext();
                if(db.VmDeploymentRequests.Any(rb => rb.ID == deploymentRequestId))
                    return true;
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CheckVmDepRequest() : "
                                    + Utilities.UnwindExceptionMessages(ex));                
            }
            finally
            {
                if (null != db)
                    db.Dispose();
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deploymentRequestId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************
        public Models.VmDeploymentRequest FetchVmDepRequest(int deploymentRequestId)
        {
            var vmrList = new List<Models.VmDeploymentRequest>();
            Models.CMPContext db = null;

            try
            {
                db = new Models.CMPContext();

                //using (var db = new Models.CMPContext())
                //{
                db.Database.Connection.ConnectionString = _ConnectionString;

                var vmrQ = db.VmDeploymentRequests.Where(rb => rb.ID == deploymentRequestId);

                if (!vmrQ.Any())
                    throw new Exception("No record found with deploymentRequestID:" +
                                        deploymentRequestId);

                return vmrQ.First();
                //}
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVmDepRequest() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
            finally
            {
                if(null != db)
                    db.Dispose();
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deploymentRequestId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.ChangeLog> FetchVmDepRequestChangeRecords(int deploymentRequestId)
        {
            var vmrList = new List<Models.ChangeLog>();

            Models.CMPContext db = null;

            try
            {
                db = new Models.CMPContext();

                //using (var db = new Models.CMPContext())
                //{
                db.Database.Connection.ConnectionString = _ConnectionString;

                var vmrQ = db.ChangeLogs.Where(rb => rb.RequestID == deploymentRequestId);

                //if (!vmrQ.Any())
                //    throw new Exception("No record found with deploymentRequestID:" +
                //                        deploymentRequestId);

                vmrList.AddRange(vmrQ);

                return vmrList;
                //}
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVmDepRequestChangeRecords() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
            finally
            {
                if (null != db)
                    db.Dispose();
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        /// <param name="vmRequest"></param>
        /// <param name="warningList"></param>
        ///  
        //*********************************************************************

        public void SetVmDepRequestStatus(Models.VmDeploymentRequest vmRequest,
            List<string> warningList)
        {
            try
            {
                ReplaceVmDepRequest(vmRequest, warningList, RequestReplacementTypeEnum.StatusUpdate);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in SetVmRequestStatus() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmRequest"></param>
        /// 
        //*********************************************************************

        public void ResubmitVmDepRequest(Models.VmDeploymentRequest vmRequest )
        {
            try
            {
                ReplaceVmDepRequest(vmRequest, null, RequestReplacementTypeEnum.Resubmit);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ResubmitVmDepRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// 
        //*********************************************************************

        public bool IsVmDepRequestInProcess(string serverName)
        {
            var vmrList = new List<Models.VmDeploymentRequest>();

            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    IQueryable<VmDeploymentRequest> vmrQ = db.VmDeploymentRequests.Where(rb => (
                        rb.TargetVmName == serverName & 
                        rb.StatusCode != Constants.StatusEnum.Complete.ToString() & 
                        rb.StatusCode != Constants.StatusEnum.Exception.ToString() &
                        rb.StatusCode != Constants.StatusEnum.Rejected.ToString() ));

                    if(vmrQ.Any())
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in IsVmDepRequestInProcess() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        public enum RequestReplacementTypeEnum { Resubmit, StatusUpdate };

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        /// <param name="vmRequest"></param>
        /// <param name="warningList"></param>
        /// <param name="replacementType"></param>
        ///  
        //*********************************************************************

        private void ReplaceVmDepRequest(Models.VmDeploymentRequest vmRequest,
            List<string> warningList, RequestReplacementTypeEnum replacementType)
        {
            try
            {
                var now = DateTime.UtcNow;

                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var foundReqList = db.VmDeploymentRequests.Where(vmr => vmr.ID == vmRequest.ID);

                    if (!foundReqList.Any())
                    {
                        LogThis("SetVmRequestStatus() : Unable to locate VM request record: ID: "
                            + vmRequest.ID, null);
                        throw new Exception("Unable to locate VM request record: ID: "
                            + vmRequest.ID);
                    }
                    else
                    {
                        var foundReq = foundReqList.First();

                        //*** common ***

                        foundReq.AftsID = vmRequest.AftsID;
                        foundReq.StatusCode = vmRequest.StatusCode;
                        foundReq.CurrentStateStartTime = vmRequest.CurrentStateStartTime;
                        foundReq.LastStatusUpdate = now;
                        foundReq.TargetAccountType = vmRequest.TargetAccountType;
                        foundReq.TargetServicename = vmRequest.TargetServicename;
                        foundReq.ServiceProviderAccountID = vmRequest.ServiceProviderAccountID;
                        foundReq.TargetAccount = vmRequest.TargetAccount;
                        foundReq.CurrentStateTryCount = vmRequest.CurrentStateTryCount;
                         
                        if (null != vmRequest.Config)
                            foundReq.Config = string.Copy(vmRequest.Config);
                       
                        if (null != vmRequest.SourceVhdFilesCSV)
                            foundReq.SourceVhdFilesCSV = vmRequest.SourceVhdFilesCSV;

                        switch (replacementType)
                        {
                            case RequestReplacementTypeEnum.Resubmit:

                                foundReq.ParentAppName = vmRequest.ParentAppName;
                                foundReq.RequestDescription = vmRequest.RequestDescription;
                                foundReq.RequestName = vmRequest.RequestName;
                                foundReq.SourceServerName = vmRequest.SourceServerName;
                                foundReq.SourceServerRegion = vmRequest.SourceServerRegion;
                                foundReq.TagData = vmRequest.TagData;
                                foundReq.TargetLocation = vmRequest.TargetLocation;
                                foundReq.TargetLocationType = vmRequest.TargetLocationType;
                                foundReq.TargetVmName = vmRequest.TargetVmName;
                                foundReq.VmSize = vmRequest.VmSize;
                                foundReq.WhenRequested = now;
                                foundReq.WhoRequested = vmRequest.WhoRequested;
                                foundReq.Active = vmRequest.Active;
                                foundReq.AftsID = vmRequest.AftsID;
                                foundReq.ParentAppID = vmRequest.ParentAppID;
                                foundReq.RequestType = vmRequest.RequestType;
                                foundReq.StatusMessage = "Resubmitted";
                                foundReq.TargetAccountCreds = vmRequest.TargetAccountCreds;
                                foundReq.TargetServiceProviderType = vmRequest.TargetServiceProviderType;
                                foundReq.TagID = vmRequest.TagID;
                                foundReq.ServiceProviderStatusCheckTag = null;
                                foundReq.OverwriteExisting = false;
                                foundReq.ValidationResults = null;
                                foundReq.ExceptionTypeCode = null;
                                foundReq.LastState = null;
                                foundReq.ExceptionMessage = null;
                                foundReq.ServiceProviderStatusCheckTag = null;
                                foundReq.Warnings = null;
                                foundReq.ExceptionTypeCode = null;
                                foundReq.ValidationResults = null;

                                break;

                            case RequestReplacementTypeEnum.StatusUpdate:

                                foundReq.LastState = foundReq.StatusCode;
                                foundReq.StatusMessage = vmRequest.StatusMessage;
                                foundReq.ExceptionMessage = vmRequest.ExceptionMessage;
                                foundReq.ExceptionTypeCode = vmRequest.ExceptionTypeCode;
                                foundReq.ValidationResults = vmRequest.ValidationResults;
                                                                
                                if (null != vmRequest.ServiceProviderStatusCheckTag)
                                    foundReq.ServiceProviderStatusCheckTag = vmRequest.ServiceProviderStatusCheckTag;

                                if (null != warningList)
                                {
                                    foreach (string warning in warningList)
                                    {
                                        if (null == foundReq.Warnings)
                                            foundReq.Warnings = warning;
                                        else
                                            foundReq.Warnings = foundReq.Warnings + "; " + warning;
                                    }

                                    vmRequest.Warnings = foundReq.Warnings;
                                }

                                break;
                        }

                        //*** Blank out passwords before updating DB ***

                        /*string pwd = Utilities.GetXmlInnerText(foundReq.Config, "Password");
                        if (null != pwd) if (0 < pwd.Length)
                                foundReq.Config = foundReq.Config.Replace(pwd, "XXX");*/

                        //*** Save change record ***

                        var cl = new Models.ChangeLog
                        {
                            StatusCode = vmRequest.StatusCode,
                            RequestID = vmRequest.ID,
                            When = now,
                            Message = vmRequest.ExceptionMessage ?? vmRequest.StatusMessage
                        };

                        if (null == cl.Message)
                            cl.Message = foundReq.StatusMessage;

                        SaveChangeRecord(db, cl);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ReplaceVmDepRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private int FetchMaxChangeLogID(Models.CMPContext db)
        {
            int maxId = 0;

            try
            {
                maxId = (from rb in db.ChangeLogs
                         select rb.ID).Max();
            }
            catch (InvalidOperationException)
            {
                maxId = 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchMaxChangeLogID() : " + ex.Message);
            }

            return maxId;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="changeRecord"></param>
        /// 
        //*********************************************************************

        public void SaveChangeRecord(Models.CMPContext db, Models.ChangeLog changeRecord)
        {
            try
            {
                changeRecord.ID = FetchMaxChangeLogID(db) + 1;
                db.ChangeLogs.Add(changeRecord);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in SaveChangeRecord() "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private int FetchMaxVmDepID(Models.CMPContext db)
        {
            int maxId = 0;

            try
            {
                maxId = (db.VmDeploymentRequests.Select(rb => rb.ID)).Max();
            }
            catch (InvalidOperationException)
            {
                maxId = 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchMaxVmDepID() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }

            return maxId;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmRequest"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public Models.VmDeploymentRequest InsertVmDepRequest(Models.VmDeploymentRequest vmRequest)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    vmRequest.ID = FetchMaxVmDepID(db) + 1;

                    db.VmDeploymentRequests.Add(vmRequest);
                    db.SaveChanges();

                    var now = DateTime.UtcNow;
                    var cl = new Models.ChangeLog
                    {
                        StatusCode = vmRequest.StatusCode,
                        RequestID = vmRequest.ID,
                        When = now,
                        Message = vmRequest.ExceptionMessage ?? vmRequest.StatusMessage
                    };

                     SaveChangeRecord(db, cl);

                    return vmRequest;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in InsertVmDepRequest() "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="regionName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public Models.ServiceProviderAccount FetchOsVhdStoreAccount(string regionName)
        {
            try
            {
                var trimChars = new char[] { ' ' };

                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var foundReqList = db.ServiceProviderAccounts.Where(spa => 
                        (spa.AzRegion == regionName & spa.AccountType == "AzureOsVhdStore"));

                    Models.ServiceProviderAccount Out = null;
                    if (!foundReqList.Any())
                    {
                        LogThis("FetchOsVhdStoreAccount() : Unable to locate OsVhdStoreAccount record: Region: "
                            + regionName, null);
                        throw new Exception("Unable to locate OsVhdStoreAccount record: Region: "
                            + regionName);
                    }
                    else
                    {
                        Out = foundReqList.First() as Models.ServiceProviderAccount;

                        Out.AzAffinityGroup = Out.AzAffinityGroup.TrimEnd(trimChars);
                        Out.AzRegion = Out.AzRegion.TrimEnd(trimChars);
                        Out.AzStorageContainerUrl = Out.AzStorageContainerUrl.TrimEnd(trimChars);
                        Out.AzSubnet = Out.AzSubnet.TrimEnd(trimChars);
                        Out.AzVNet = Out.AzVNet.TrimEnd(trimChars);
                    }

                    return Out;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchOsVhdStoreAccount() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #region Ops Region

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opsRequest"></param>
        /// <param name="warningList"></param>
        /// <param name="replacementType"></param>
        /// 
        //*********************************************************************

        private void ReplaceOpsRequest(Models.OpRequest opsRequest,
            List<string> warningList, RequestReplacementTypeEnum replacementType)
        {
            try
            {
                var now = DateTime.UtcNow;

                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var foundReqList = db.OpRequests.Where(vmr => vmr.Id == opsRequest.Id);

                    if (!foundReqList.Any())
                    {
                        LogThis("SetOpsRequestStatus() : Unable to locate Ops request record: ID: "
                            + opsRequest.Id, null);
                        throw new Exception("Unable to locate Ops request record: ID: "
                            + opsRequest.Id);
                    }
                    else
                    {
                        var foundReq = foundReqList.First();

                        //*** common ***

                        foundReq.Active = opsRequest.Active;
                        foundReq.Config = opsRequest.Config;
                        foundReq.CurrentStateStartTime = opsRequest.CurrentStateStartTime;
                        foundReq.CurrentStateTryCount = opsRequest.CurrentStateTryCount;
                        foundReq.ExceptionMessage = opsRequest.ExceptionMessage;
                        foundReq.LastStatusUpdate = now;
                        foundReq.RequestDescription = opsRequest.RequestDescription;
                        foundReq.RequestName = opsRequest.RequestName;
                        foundReq.RequestType = opsRequest.RequestType;
                        foundReq.ServiceProviderStatusCheckTag = opsRequest.ServiceProviderStatusCheckTag;
                        foundReq.StatusCode = opsRequest.StatusCode;
                        foundReq.StatusMessage = opsRequest.StatusMessage;
                        foundReq.TagData = opsRequest.TagData;
                        foundReq.TagID = opsRequest.TagID;
                        foundReq.TargetName = opsRequest.TargetName;

                        if (null != opsRequest.Config)
                            foundReq.Config = string.Copy(opsRequest.Config);

                        db.SaveChanges();

                        /*switch (replacementType)
                        {
                            case RequestReplacementTypeEnum.Resubmit:

                                foundReq.ParentAppName = vmRequest.ParentAppName;
                                foundReq.RequestDescription = vmRequest.RequestDescription;
                                foundReq.RequestName = vmRequest.RequestName;
                                foundReq.SourceServerName = vmRequest.SourceServerName;
                                foundReq.SourceServerRegion = vmRequest.SourceServerRegion;
                                foundReq.TagData = vmRequest.TagData;
                                foundReq.TargetLocation = vmRequest.TargetLocation;
                                foundReq.TargetLocationType = vmRequest.TargetLocationType;
                                foundReq.TargetVmName = vmRequest.TargetVmName;
                                foundReq.VmSize = vmRequest.VmSize;
                                foundReq.WhenRequested = now;
                                foundReq.WhoRequested = vmRequest.WhoRequested;
                                foundReq.Active = vmRequest.Active;
                                foundReq.AftsID = vmRequest.AftsID;
                                foundReq.ParentAppID = vmRequest.ParentAppID;
                                foundReq.RequestType = vmRequest.RequestType;
                                foundReq.StatusMessage = "Resubmitted";
                                foundReq.TargetAccountCreds = vmRequest.TargetAccountCreds;
                                foundReq.TargetServiceProviderType = vmRequest.TargetServiceProviderType;
                                foundReq.TagID = vmRequest.TagID;
                                foundReq.ServiceProviderStatusCheckTag = null;
                                foundReq.OverwriteExisting = false;
                                foundReq.ValidationResults = null;
                                foundReq.ExceptionTypeCode = null;
                                foundReq.LastState = null;
                                foundReq.ExceptionMessage = null;
                                foundReq.ServiceProviderStatusCheckTag = null;
                                foundReq.Warnings = null;
                                foundReq.ExceptionTypeCode = null;
                                foundReq.ValidationResults = null;

                                break;

                            case RequestReplacementTypeEnum.StatusUpdate:

                                foundReq.LastState = foundReq.StatusCode;
                                foundReq.StatusMessage = vmRequest.StatusMessage;
                                foundReq.ExceptionMessage = vmRequest.ExceptionMessage;
                                foundReq.ExceptionTypeCode = vmRequest.ExceptionTypeCode;
                                foundReq.ValidationResults = vmRequest.ValidationResults;

                                if (null != vmRequest.ServiceProviderStatusCheckTag)
                                    foundReq.ServiceProviderStatusCheckTag = vmRequest.ServiceProviderStatusCheckTag;

                                if (null != warningList)
                                {
                                    foreach (string warning in warningList)
                                    {
                                        if (null == foundReq.Warnings)
                                            foundReq.Warnings = warning;
                                        else
                                            foundReq.Warnings = foundReq.Warnings + "; " + warning;
                                    }

                                    vmRequest.Warnings = foundReq.Warnings;
                                }

                                break;
                        }

                        //*** Save change record ***

                        var cl = new Models.ChangeLog
                        {
                            StatusCode = vmRequest.StatusCode,
                            RequestID = vmRequest.ID,
                            When = now,
                            Message = vmRequest.ExceptionMessage ?? vmRequest.StatusMessage
                        };

                        if (null == cl.Message)
                            cl.Message = foundReq.StatusMessage;

                        SaveChangeRecord(db, cl);*/
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ReplaceOpsRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private int FetchMaxOpsId(Models.CMPContext db)
        {
            int maxId = 0;

            try
            {
                maxId = (db.OpRequests.Select(rb => rb.Id)).Max();
            }
            catch (InvalidOperationException)
            {
                maxId = 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchMaxOpsID() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }

            return maxId;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opsRequest"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public Models.OpRequest InsertOpsRequest(Models.OpRequest opsRequest)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    opsRequest.Id = FetchMaxOpsId(db) + 1;

                    db.OpRequests.Add(opsRequest);
                    db.SaveChanges();

                    var now = DateTime.UtcNow;

                    /*var cl = new Models.ChangeLog
                    {
                        StatusCode = opsRequest.StatusCode,
                        RequestID = vmRequest.ID,
                        When = now,
                        Message = vmRequest.ExceptionMessage ?? vmRequest.StatusMessage
                    };

                    SaveChangeRecord(db, cl);*/

                    return opsRequest;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in InsertOpsRequest() "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        /// <summary>
        ///  
        /// </summary>
        /// <param name="vmName"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************
        public Models.OpRequest FetchVMOpRequest(string vmName)
        {
            try 
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;
                    IQueryable<OpRequest> vmrQ;
                    vmrQ = db.OpRequests.Where(vm => (vm.TargetName.Contains(vmName) && vm.StatusCode == "Submitted")).OrderBy(rb => rb.WhenRequested);


                    if (vmrQ.FirstOrDefault() == null)
                    {
                        vmrQ = db.OpRequests.Where(vm => (vm.TargetName.Contains(vmName))).OrderByDescending(rb =>rb.WhenRequested);
                        return vmrQ.FirstOrDefault();
                    }

                    return vmrQ.FirstOrDefault();
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Exception in FetchOpsRequests() : "
                                    + Utilities.UnwindExceptionMessages(ex));            
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.OpRequest> FetchOpsRequests(string status, bool active)
        {
            var oprList = new List<Models.OpRequest>();

            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    IOrderedQueryable<OpRequest> vmrQ;

                    if (null == status)
                        vmrQ = db.OpRequests.Where(rb => (rb.Active == active))
                                .OrderBy(rb => rb.Id);
                    else
                        vmrQ = db.OpRequests.Where(rb => (rb.StatusCode == status & rb.Active == active))
                                .OrderBy(rb => rb.Id);

                    oprList.AddRange(vmrQ);

                    return oprList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchOpsRequests() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opsRequest"></param>
        /// <param name="warningList"></param>
        /// 
        //*********************************************************************

        public void SetOpsRequestStatus(Models.OpRequest opsRequest,
            List<string> warningList)
        {
            try
            {
                ReplaceOpsRequest(opsRequest, warningList, 
                    RequestReplacementTypeEnum.StatusUpdate);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in SetOpsRequestStatus() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        //*********************************************************************
        //*** ServiceProviderAccount Region ***********************************
        //*********************************************************************

        #region ServiceProviderAccount Region

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public Models.ServiceProviderAccount FetchServiceProviderAccount(int id)
        {
            try
            {
                var trimChars = new char[] { ' ' };

                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var foundReqList = db.ServiceProviderAccounts.Where(spa => spa.ID == id);

                    Models.ServiceProviderAccount Out = null;
                    if (!foundReqList.Any())
                    {
                        LogThis("SetVmRequestStatus() : Unable to locate ServiceProviderAccount record: ID: "
                            + id, null);
                        throw new Exception("Unable to locate VM request record: ID: "
                            + id);
                    }
                    else
                    {
                        Out = foundReqList.First() as Models.ServiceProviderAccount;
                        var cert = new KryptoLib.X509Krypto("KryptoCert");

                        if (null != Out.AzAffinityGroup)
                            Out.AzAffinityGroup = Out.AzAffinityGroup.TrimEnd(trimChars);
                        if (null != Out.AzRegion)
                            Out.AzRegion = Out.AzRegion.TrimEnd(trimChars);
                        if (null != Out.AzStorageContainerUrl)
                            Out.AzStorageContainerUrl = Out.AzStorageContainerUrl.TrimEnd(trimChars);
                        if (null != Out.AzSubnet)
                            Out.AzSubnet = Out.AzSubnet.TrimEnd(trimChars);
                        if (null != Out.AzVNet)
                            Out.AzVNet = Out.AzVNet.TrimEnd(trimChars);
                        if (null != Out.AzureADClientKey)
                            Out.AzureADClientKey = cert.Decrypt(Out.AzureADClientKey);
                    }

                    return Out;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchServiceProviderAccount() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceGroupName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.ServiceProviderAccount> FetchServiceProviderAccountList(string resourceGroupName)
        {
            try
            {
                var trimChars = new char[] { ' ' };
                var outList = new List<Models.ServiceProviderAccount>();
                //var cert = new KryptoLib.X509Krypto("KryptoCert");


                using (var db = new Models.CMPContext())
                {
                    //db.Database.Connection.ConnectionString = _ConnectionString;

                    IQueryable<ServiceProviderAccount> foundReqList = null;

                    if (string.IsNullOrEmpty(resourceGroupName))
                    {
                        foundReqList = db.ServiceProviderAccounts.Where(spa => spa.Active == true);

                        if (!foundReqList.Any())
                        {
                            LogThis(
                                "FetchServiceProviderAccountList() : Unable to locate any ServiceProviderAccount records", null);
                            throw new Exception("Unable to locate any ServiceProviderAccount records");
                        }
                    }
                    else
                    {
                        foundReqList = db.ServiceProviderAccounts.Where(spa => spa.ResourceGroup == resourceGroupName &&  spa.Active == true);

                        if (!foundReqList.Any())
                        {
                            LogThis(
                                "FetchServiceProviderAccountList() : Unable to locate ServiceProviderAccount record: ResourceGroup: "
                                + resourceGroupName, null);
                            throw new Exception("Unable to locate ServiceProviderAccount record: ResourceGroup: "
                                                + resourceGroupName);
                        }
                    }
                
                    foreach (var spa in foundReqList)
                    {
                        spa.AzAffinityGroup = null == spa.AzAffinityGroup ? null : spa.AzAffinityGroup.TrimEnd(trimChars);

                        spa.AzRegion                = null == spa.AzAffinityGroup ? null : spa.AzAffinityGroup.TrimEnd(trimChars);
                        spa.AzStorageContainerUrl   = null == spa.AzStorageContainerUrl ? null : spa.AzStorageContainerUrl.TrimEnd(trimChars);
                        spa.AzSubnet                = null == spa.AzSubnet ? null : spa.AzSubnet.TrimEnd(trimChars);
                        spa.AzVNet                  = null == spa.AzVNet ? null : spa.AzVNet.TrimEnd(trimChars);
                        //Temporarily using "using" because of the Safe handle has been closed issue. 
                        using (var cert = new KryptoLib.X509Krypto("KryptoCert"))
                        {
                            spa.AzureADClientKey = null == spa.AzureADClientKey ? null : cert.Decrypt(spa.AzureADClientKey);
                        }

                        outList.Add(spa);
                    }

                    return outList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchServiceProviderAccountList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idsToSearch"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public IEnumerable<Models.ServiceProviderAccount> FetchServiceProviderAccountList(IEnumerable<int> idsToSearch)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    IQueryable<ServiceProviderAccount> foundSpaList = null;

                    if (!idsToSearch.Any())
                    {
                        LogThis(
                            "FetchServiceProviderAccountList() : idsToSearch parameter is empty", null);
                        throw new Exception("Unable to search for any ServiceProviderAccount records. idsToSearch parameter is empty");
                    }

                    foundSpaList = db.ServiceProviderAccounts.Where(spa => idsToSearch.Contains(spa.ID) && spa.Active == true);

                    if (!foundSpaList.Any())
                    {
                        LogThis(
                            "FetchServiceProviderAccountList() : Unable to locate ServiceProviderAccount record", null);
                        throw new Exception("Unable to locate ServiceProviderAccount record");
                    }

                    return foundSpaList.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchServiceProviderAccountList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public IEnumerable<Models.ServiceProviderAccount> FetchServiceProviderAccountList()
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;
                    IQueryable<ServiceProviderAccount> foundSpaList = null;

                    foundSpaList = db.ServiceProviderAccounts.Where(spa => spa.Active == true);

                    if (!foundSpaList.Any())
                    {
                        LogThis(
                            "FetchServiceProviderAccountList() : Unable to locate ServiceProviderAccount record", null);
                        throw new Exception("Unable to locate ServiceProviderAccount record");
                    }

                    return foundSpaList.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchServiceProviderAccountList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private int ServiceProviderAccountId(Models.CMPContext db)
        {
            int maxId = 0;

            try
            {
                maxId = (db.ServiceProviderAccounts.Select(rb => rb.ID)).Max();
            }
            catch (InvalidOperationException)
            {
                maxId = 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ServiceProviderAccountId() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }

            return maxId;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPa"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public ServiceProviderAccount InsertServiceProviderAccount(ServiceProviderAccount sPa)
        {
            try
            {
                var trimChars = new char[] { ' ' };
                var outList = new List<Models.ServiceProviderAccount>();

                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    sPa.ID = ServiceProviderAccountId(db) + 1;
                    var cert = new KryptoLib.X509Krypto("KryptoCert");

                    sPa.AzureADClientKey = cert.Encrypt(sPa.AzureADClientKey);

                    db.ServiceProviderAccounts.Add(sPa);
                    db.SaveChanges();

                    var now = DateTime.UtcNow;

                    return sPa;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in InsertServiceProviderAccount() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPa"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public ServiceProviderAccount UpdateServiceProviderAccount(ServiceProviderAccount sPa)
        {
            try
            {
                var trimChars = new char[] { ' ' };

                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var foundReqList = db.ServiceProviderAccounts.Where(spa => spa.ID == sPa.ID);

                    Models.ServiceProviderAccount Out = null;
                    if (!foundReqList.Any())
                    {
                        LogThis("SetVmRequestStatus() : Unable to locate ServiceProviderAccount record: ID: "
                            + sPa.ID, null);
                        throw new Exception("Unable to locate VM request record: ID: "
                            + sPa.ID);
                    }
                    else
                    {
                        Out = foundReqList.First();

                        Out.Name = sPa.Name;
                        Out.Description = sPa.Description;
                        Out.ResourceGroup = sPa.ResourceGroup;
                        Out.AccountID = sPa.AccountID;
                        Out.AccountType = sPa.AccountType;
                        Out.CertificateThumbprint = sPa.CertificateThumbprint;

                        db.SaveChanges();
                    }

                    return Out;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in UpdateServiceProviderAccount() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPa"></param>
        /// 
        //*********************************************************************

        public void DeleteServiceProviderAccount(ServiceProviderAccount sPa)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var foundReqList = db.ServiceProviderAccounts.Where(spa => spa.ID == sPa.ID);

                    if (!foundReqList.Any())
                    {
                        LogThis("SetVmRequestStatus() : Unable to locate ServiceProviderAccount record: ID: "
                            + sPa.ID, null);
                        throw new Exception("Unable to locate VM request record: ID: "
                            + sPa.ID);
                    }
                    else
                    {
                        foreach (var row in foundReqList.ToList())
                        {
                            db.ServiceProviderAccounts.Remove(row);
                        }                      
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in DeleteServiceProviderAccount() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        //*********************************************************************
        //*** CmpServiceUserAccount Region ************************************
        //*********************************************************************

        #region CmpServiceUserAccount Region

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.CmpServiceUserAccount> FetchCmpServiceUserAccounts(string Status, bool active)
        {
            var AccountList = new List<Models.CmpServiceUserAccount>();

            try
            {
                using (KryptoLib.X509Krypto XK = new KryptoLib.X509Krypto())

                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;
                    System.Linq.IOrderedQueryable<Models.CmpServiceUserAccount> AccountListQ = null;

                    AccountListQ = null == Status ? db.CmpServiceUserAccounts.Where(RB => RB.IsActive == active).OrderBy(RB => RB.Id) :
                        db.CmpServiceUserAccounts.Where(RB => (RB.StatusCode == Status & RB.IsActive == active)).OrderBy(RB => RB.Id);

                    foreach (var account in AccountListQ)
                    {
                        account.Password = XK.DecrpytKText(account.Password);
                        AccountList.Add(account);
                    }

                    return AccountList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchCmpServiceUserAccounts() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        /// <summary>
        ///  
        /// </summary>
        /// <param name="deploymentRequestID"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************
        public Models.CmpServiceUserAccount FetchCmpServiceUserAccount(int accountID)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var userListQ = db.CmpServiceUserAccounts.Where(RB => RB.Id == accountID);

                    if (!userListQ.Any())
                        throw new Exception("No record found with accountID:" +
                            accountID);

                    return userListQ.First();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchCmpServiceUserAccount() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        //*********************************************************************
        //*** CmpServiceUserAccount Region ************************************
        //*********************************************************************

        #region MigrationRequest Region

        // CREATE CLUSTERED INDEX VmMigrationRequests_Index ON VmMigrationRequests (ID)

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private int FetchMaxVmMigId(Models.CMPContext db)
        {
            int maxId = 0;

            try
            {
                maxId = (db.VmMigrationRequests.Select(rb => rb.ID)).Max();
            }
            catch (InvalidOperationException)
            {
                maxId = 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchMaxVmMigID() : " + ex.Message);
            }

            return maxId;
        }

        public enum FetchMigrationRequestKeyTypeEnum
        {
            MigReqId,
            DepReqId
        };

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="requestId"></param>
        /// <param name="keyType"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        public Models.VmMigrationRequest FetchMigrationRequest(int requestId,
            FetchMigrationRequestKeyTypeEnum keyType)
        {
            var vmrList = new List<Models.VmMigrationRequest>();

            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    IQueryable<VmMigrationRequest> vmrQ = null;

                    switch (keyType)
                    {
                        case FetchMigrationRequestKeyTypeEnum.MigReqId:
                            vmrQ = db.VmMigrationRequests.Where(rb => rb.ID == requestId);
                            break;

                        case FetchMigrationRequestKeyTypeEnum.DepReqId:
                            vmrQ = db.VmMigrationRequests.Where(rb => rb.VmDeploymentRequestID == requestId);
                            break;
                    }

                    if (null == vmrQ)
                        throw new Exception("No record found with migrationRequestID:" +
                            requestId);

                    if (!vmrQ.Any())
                        throw new Exception("No record found with migrationRequestID:" +
                            requestId);

                    return vmrQ.First();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchMigrationRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqStatus"></param>
        /// <param name="reqAgentRegion"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.VmMigrationRequest> FetchMigrationRequests(string reqStatus, string reqAgentRegion)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    IQueryable<VmMigrationRequest> vmrQ;

                    if (null == reqStatus && null == reqAgentRegion)
                        vmrQ = db.VmMigrationRequests.Where(rb => rb.StatusCode == reqStatus);
                    else if (null == reqStatus)
                        vmrQ = db.VmMigrationRequests.Where(rb => rb.AgentRegion == reqAgentRegion);
                    else if (reqStatus == Constants.StatusEnum.ReadyForTransfer.ToString())
                        vmrQ =
                            db.VmMigrationRequests.Where(
                                rb => rb.AgentRegion == reqAgentRegion & rb.StatusCode != reqStatus); //Todo:Needs improvement by extracting the operator that we are passing
                    else
                        vmrQ = db.VmMigrationRequests.Where(
                                    rb => rb.AgentRegion == reqAgentRegion & rb.StatusCode == reqStatus);



                    return new List<Models.VmMigrationRequest>(vmrQ);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchMigrationRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqAgentRegion"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.VmMigrationRequest> FetchMigrationRequests(string reqAgentRegion)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    IEnumerable<VmMigrationRequest> vmrQ = db.Database.SqlQuery<Models.VmMigrationRequest>(
                       "select * from vw_ActiveMigrationRequests", new object[] { })
                       .Where(o => ((o.AgentRegion == null)
                                    ||
                                    (String.Equals(o.AgentRegion, reqAgentRegion.Trim(),
                                        StringComparison.CurrentCultureIgnoreCase))));

                    return new List<Models.VmMigrationRequest>(vmrQ);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchMigrationRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.VmMigrationRequest> FetchMigrationRequestsByAgent(string agentName)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    IEnumerable<VmMigrationRequest> vmrQ = db.Database.SqlQuery<Models.VmMigrationRequest>("select * from vw_ActiveMigrationRequests")
                        .Where(o => ((o.AgentRegion == null)
                                     ||
                                     (String.Equals(o.AgentRegion.Trim().ToUpper(), "CO1",
                                         StringComparison.CurrentCultureIgnoreCase)) &&
                       ((o.AgentName == null)
                                      ||
                                      (String.Equals(o.AgentName.Trim().ToLower(), agentName.Trim().ToLower(),
                                          StringComparison.CurrentCultureIgnoreCase)))));

                    return new List<Models.VmMigrationRequest>(vmrQ).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchMigrationRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmRequest"></param>
        /// <param name="warningList"></param>
        /// 
        //*********************************************************************

        public void UpdateVmMigrationRequest(Models.VmMigrationRequest vmRequest, List<string> warningList)
        {
            try
            {
                DateTime now = DateTime.UtcNow;

                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var foundReqList = db.VmMigrationRequests.Where(vmr => vmr.ID == vmRequest.ID);

                    if (!foundReqList.Any())
                    {
                        LogThis("UpdateVmMigrationRequest() : Unable to locate VM migration request record: ID: "
                            + vmRequest.ID, null);
                        throw new Exception("Unable to locate VM migration request record: ID: "
                            + vmRequest.ID);
                    }
                    else
                    {
                        var foundReq = foundReqList.First();

                        if (null != warningList)
                            foreach (var warning in warningList)
                            {
                                if (null == foundReq.Warnings)
                                    foundReq.Warnings = warning;
                                else
                                    foundReq.Warnings = foundReq.Warnings + "; " + warning;
                            }
                        if (null != vmRequest.Config)
                            foundReq.Config = vmRequest.Config;
                        foundReq.SourceVhdFilesCSV = vmRequest.SourceVhdFilesCSV;
                        if (null != vmRequest.StatusCode)
                            foundReq.StatusCode = vmRequest.StatusCode;
                        if (null != vmRequest.StatusMessage)
                            foundReq.StatusMessage = vmRequest.StatusMessage;
                        foundReq.ExceptionMessage = vmRequest.ExceptionMessage;
                        foundReq.CurrentStateStartTime = vmRequest.CurrentStateStartTime;
                        foundReq.LastStatusUpdate = now;
                        if (null != vmRequest.AgentName)
                            foundReq.AgentName = vmRequest.AgentName;
                        if (null != vmRequest.AgentRegion)
                            foundReq.AgentRegion = vmRequest.AgentRegion;
                        if (null != vmRequest.SourceVhdFilesCSV)
                            foundReq.SourceVhdFilesCSV = vmRequest.SourceVhdFilesCSV;

                        db.SaveChanges();

                        //*** TODO: Mark West : set change record type

                        //*** Save change record ***

                        /*Models.ChangeLog CL = new Models.ChangeLog
                        {
                            Message = vmRequest.ExceptionMessage,
                            StatusCode = vmRequest.StatusCode,
                            RequestID = vmRequest.ID,
                            When = Now
                        };

                        SaveChangeRecord(db, CL);*/
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in SetVmRequestStatus() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }


        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmMigRequest"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public Models.VmMigrationRequest InsertVmMigrationRequest(Models.VmMigrationRequest vmMigRequest)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    vmMigRequest.ID = FetchMaxVmMigId(db) + 1;

                    db.VmMigrationRequests.Add(vmMigRequest);
                    db.SaveChanges();
                    var now = DateTime.UtcNow;
                    var cl = new Models.ChangeLog
                    {
                        StatusCode = vmMigRequest.StatusCode,
                        RequestID = vmMigRequest.ID,
                        When = now,
                        Message = vmMigRequest.ExceptionMessage ?? vmMigRequest.StatusMessage
                    };

                    SaveChangeRecord(db, cl);
                    return vmMigRequest;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in InsertVmMigrationRequest() "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region BadAset Region

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="assetType"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public bool IsAssetBad(string assetName, 
            CmpInterfaceModel.Constants.AssetTypeCodeEnum assetType)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var foundReqList = db.BadAssets.Where(ba =>
                        (ba.AssetName == assetName & 
                        ba.AssetTypeCode == assetType.ToString() & ba.Active));

                    return foundReqList.Any();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in IsAssetBad() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private int FetchMaxBadAssetId(Models.CMPContext db)
        {
            int maxId = 0;

            try
            {
                maxId = (db.BadAssets.Select(rb => rb.Id)).Max();
            }
            catch (InvalidOperationException)
            {
                maxId = 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchMaxBadAssetId() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }

            return maxId;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        /// <param name="assetName"></param>
        /// <param name="assetType"></param>
        /// <param name="problemDescription"></param>
        /// <param name="tagData"></param>
        /// <param name="whoReported"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        public Models.BadAsset InsertBadAsset(string assetName,
            CmpInterfaceModel.Constants.AssetTypeCodeEnum assetType, 
            string problemDescription, string tagData, string whoReported)
        {
            try
            {
                if (IsAssetBad(assetName, assetType))
                    return null;

                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var badAss = new BadAsset
                    {
                        Id = FetchMaxBadAssetId(db) + 1, 
                        Active = true, 
                        AssetName = assetName, 
                        AssetTypeCode = assetType.ToString(), 
                        Config = null,
                        ProblemDescription = problemDescription,
                        TagData = tagData, 
                        WhenReported = DateTime.UtcNow,
                        WhoReported = whoReported
                    };


                    db.BadAssets.Add(badAss);
                    db.SaveChanges();

                    return badAss;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in InsertBadAsset() "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region Sync with Azure Info Region

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches VM size (Azure RoleSize) info from the CMP
        ///     DB, which is synced with Azure ARM to provide the most current
        ///     offerings of VM sizes
        /// </summary>
        /// <returns>AzureRoleSize IEnumerable</returns>
        /// 
        //*********************************************************************

        public IEnumerable<AzureRoleSize> FetchAzureRoleSizeList()
        {
            //var azureRoleSizesList = new List<AzureRoleSize>();

            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var azureRoleSizesSet = (from ars in db.AzureRoleSizes
                                             orderby ars.Name
                                             select ars);

                    return azureRoleSizesSet.ToList();
                    //return azureRoleSizesList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchAzureRoleSizeList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to insert an AzureRoleSize into the CmpDb
        /// </summary>
        /// <param name="ars"></param>
        /// <returns>AzureRoleSize object</returns>
        /// 
        //*********************************************************************

        public void InsertAzureRoleSize(AzureRoleSize ars)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    db.AzureRoleSizes.Add(ars);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in InsertAzureRoleSize() "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureRoleSize GetAzureRoleSizeFromName(string name)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var result = db.AzureRoleSizes.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetAzureRoleSizeFromName()"
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region Sync with Azure Containers Region

        private int FetchMaxAzureContainerID(Models.CMPContext db)
        {
            int maxId = 0;

            try
            {
                maxId = (from rb in db.Containers
                         select rb.ID).Max();
            }
            catch (InvalidOperationException)
            {
                maxId = 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchMaxAzureContainerID() : " + ex.Message);
            }

            return maxId;
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches VM size (Azure RoleSize) info from the CMP
        ///     DB, which is synced with Azure ARM to provide the most current
        ///     offerings of VM sizes
        /// </summary>
        /// <returns>AzureRoleSize IEnumerable</returns>
        /// 
        //*********************************************************************

        public IEnumerable<AzureResourceGroup> FetchAzureContainerList()
        {
            var azureResourceGroupList = new List<AzureResourceGroup>();

            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var qresult = (from ars in db.Containers
                                             orderby ars.Name
                                             select ars);

                    azureResourceGroupList.AddRange(qresult.Select(arg => new AzureResourceGroup()
                    {
                        Id = arg.ID.ToString(),
                        Name = arg.Name,
                        Location = arg.Region
                    }));

                    return azureResourceGroupList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchAzureContainerList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches VM size (Azure RoleSize) info from the CMP
        ///     DB, which is synced with Azure ARM to provide the most current
        ///     offerings of VM sizes
        /// </summary>
        /// <returns>AzureRoleSize IEnumerable</returns>
        /// 
        //*********************************************************************
  
        public IEnumerable<AzureResourceGroup> FetchAzureContainerList(string subscriptionId)
        {
            var azureResourceGroupList = new List<AzureResourceGroup>();

            if (null == subscriptionId)
                return FetchAzureContainerList();

            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var qresult = (from ars in db.Containers
                                             where ars.SubscriptionId.Equals(subscriptionId)
                                             orderby ars.Name
                                             select ars);

                    azureResourceGroupList.AddRange(qresult.Select(arg => new AzureResourceGroup()
                    {
                        Id = arg.ID.ToString(),
                        Name = arg.Name,
                        Location = arg.Region
                    }));

                    return azureResourceGroupList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchAzureContainerList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to insert an AzureContainer into the CmpDb
        /// </summary>
        /// <param name="cont"></param>
        /// <returns>AzureRoleSize object</returns>
        /// 
        //*********************************************************************

        public void InsertAzureContainer(CmpServiceLib.Models.Container cont)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    cont.ID = 1 + FetchMaxAzureContainerID(db);

                    db.Containers.Add(cont);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in InsertAzureContainer() "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public Container GetAzureContainerFromName(string name)
        {
            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var result = db.Containers.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetAzureContainerFromName()"
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        public List<Container> GetAzureContainers()
        {
            var azureResourceGroupList = new List<AzureResourceGroup>();

            try
            {
                using (var db = new Models.CMPContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var result = (from ars in db.Containers
                                   orderby ars.Name
                                   select ars);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetAzureContainers() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }


        #endregion

    }
}
