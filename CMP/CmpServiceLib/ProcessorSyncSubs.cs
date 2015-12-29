using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AzureAdminClientLib;
using CmpServiceLib.Models;

namespace CmpServiceLib
{
    //**********************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    ///         
    //*********************************************************************

    public class ProcessorSyncSubs : IDisposable
    {
        private const int _OpsTTL = 480;
        private static string _CmpDbConnectionString = null;
        private static EventLog _EventLog = null;

        private bool _submittedQueueBlocked = false;
        private bool _allQueuesBlocked = false;

        private CmpDb _cdb = null;

        bool _disposed = false;

        #region --- Setup Methods --------------------------------------------

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        /// <param name="eLog"></param>
        ///  
        //*********************************************************************
        public ProcessorSyncSubs(EventLog eLog)
        {
            InitVals(eLog, null); ;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eLog"></param>
        /// <param name="cmpDbConnectionString"></param>
        /// 
        //*********************************************************************

        public ProcessorSyncSubs(EventLog eLog, string cmpDbConnectionString)
        {
            InitVals(eLog, cmpDbConnectionString);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eLog"></param>
        /// <param name="cmpDbConnectionString"></param>
        /// 
        //*********************************************************************

        private void InitVals(EventLog eLog, string cmpDbConnectionString)
        {
            _EventLog = eLog;
            _CmpDbConnectionString = cmpDbConnectionString;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        //*********************************************************************

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
            }

            // Free any unmanaged objects here. 
            //
            _disposed = true;
        }


        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        //*** NOTE * Network
        //*** NOTE * Refresh

        public void ProcessSyncAzureSubs(string cmpDbConnectionString)
        {
            try
            {
                _CmpDbConnectionString = cmpDbConnectionString;
                SyncWithAzureSubscriptions();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessOpsRequests() " + 
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- Async Methods --------------------------------------------

        readonly int _sleepTime = 5 * 1000;
        List<Thread> _workerThreadList = null;
        delegate object ProcessorDel();

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="proc"></param>
        /// 
        //*********************************************************************

        void AddProcessor(ProcessorDel proc)
        {
            var workerThread = new Thread(Worker) { IsBackground = true };
            workerThread.Start(new ProcessorDel(proc));
            _workerThreadList.Add(workerThread);
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="cmpDbConnectionString"></param>
        ///  
        //*********************************************************************

        public void ProcessSyncAzureSubsAsync(string cmpDbConnectionString)
        {
            try
            {
                var importAllAzureSubVms_t = 
                    Microsoft.Azure.CloudConfigurationManager.GetSetting("ImportAllAzureSubVms");

                if (null == importAllAzureSubVms_t)
                    return;

                if (!importAllAzureSubVms_t.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    return;

                _CmpDbConnectionString = cmpDbConnectionString;
                _workerThreadList = new List<Thread>(1);
                AddProcessor(SyncWithAzureSubscriptions);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessSyncAzureSubsAsync() " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public void StopProcessingAsynch()
        {
            if (null == _workerThreadList)
                return;

            foreach (var worker in _workerThreadList)
            {
                try
                {
                    if (null != worker)
                        worker.Abort();
                }
                catch (Exception)
                { }
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// 
        //*********************************************************************

        //*** NOTE * Refresh

        public void Worker(object method)
        {
            var proc = method as ProcessorDel;
            object ret;
            var methodName = "x";

            try
            {
                if (null == proc)
                    throw new Exception("proc == null");

                var target = proc.Target as System.Delegate;

                if (null == target)
                    throw new Exception("target == null");

                methodName = target.Method.ToString();

                //LogThis(null, EventLogEntryType.Information,
                //    "CmpServiceLib.ProcessorVm.Worker(" + methodName + ") successfully registered", 3, 3);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "Exception in CmpServiceLib.ProcessorSyncSubs.Worker() setup", 100, 100);
                return;
            }

            while (true)
            {
                try
                {
                    ret = proc();
                    ret = 1;
                }
                catch (Exception ex)
                {
                    LogThis(ex, EventLogEntryType.Error,
                        "Exception in CmpServiceLib.ProcessorSyncSubs.Worker(" + methodName + ")", 100, 100);
                }

                Thread.Sleep(_sleepTime);
            }
        }

        #endregion

        #region --- Helper methods --------------------------------------------

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cdb"></param>
        /// 
        //*********************************************************************

        private void ReadConfigValues(CmpDb cdb)
        {
            try
            {
                _submittedQueueBlocked = false;
                _allQueuesBlocked = false;

                var temp = cdb.FetchConfigValue("BlockAllQueues", true);

                if (null != temp)
                    if (temp.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _submittedQueueBlocked = true;
                        _allQueuesBlocked = true;
                    }

                if (!_submittedQueueBlocked)
                {
                    temp = cdb.FetchConfigValue("BlockSubmittedQueue", true);

                    if (null != temp)
                        if (temp.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                            _submittedQueueBlocked = true;
                }
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "Exception in CmpServiceLib.ProcessorOps.ReadConfigValues()", 1, 1);
            }
        }


        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="type"></param>
        /// <param name="prefix"></param>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// 
        //*********************************************************************

        private void LogThis(Exception ex, EventLogEntryType type, string prefix, 
            int id, short category)
        {
            if (null == _EventLog)
                return;

            if (null == ex)
                _EventLog.WriteEntry(prefix, type, id, category);
            else
                _EventLog.WriteEntry(prefix + " : " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex), 
                    type, id, category);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        int NonNull(int? arg)
        {
            return Utilities.GetDbInt(arg);
        }

        static int _defaultStateTtlMinutes = 0;

        /// <summary>
        /// 
        /// </summary>
        /// 
        static int DefaultStateTtlMinutes
        {
            get
            {
                if (0 == _defaultStateTtlMinutes)
                {
                    if (null == Microsoft.Azure.CloudConfigurationManager.GetSetting("DefaultStateTtlMinutes"))
                        _defaultStateTtlMinutes = 120;
                    else
                        try
                        {
                            _defaultStateTtlMinutes = 
                                Convert.ToInt32(Microsoft.Azure.CloudConfigurationManager.GetSetting("DefaultStateTtlMinutes"));
                        }
                        catch (Exception)
                        {
                            _defaultStateTtlMinutes = 120;
                        }
                }

                return _defaultStateTtlMinutes;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opsReq"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        bool HasTimedOut(Models.OpRequest opsReq, int ttl)
        {
            if (0 == ttl)
                ttl = DefaultStateTtlMinutes;

            if (null != opsReq.CurrentStateStartTime)
                if (ttl < DateTime.UtcNow.Subtract(((DateTime)opsReq.CurrentStateStartTime)).TotalMinutes)
                    return true;

            return false;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vm"></param>
        /// <param name="spa"></param>
        /// <param name="vmDepReqs"></param>
        ///  
        //*********************************************************************
        private void ProcessFoundVm(
            Microsoft.WindowsAzure.Management.Compute.Models.Role vm,
            Models.ServiceProviderAccount spa, IEnumerable<VmDeploymentRequest> vmDepReqs)
        {
            //*** Look for VM in list of VmDepReqs, leave if found
            if (vmDepReqs.Any(vdr => vdr.TargetVmName.Equals(vm.RoleName, 
                StringComparison.InvariantCultureIgnoreCase)))
                return;

            //*** VM not found, so add to VmDepReq DB table
            var utcNow = DateTime.UtcNow;

            //*** TODO * Build as much actual config as possible here
            var config =
                @"<?xml version=""1.0"" encoding=""utf-8""?><VmConfig xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""></VmConfig>";

            var vdb = new Models.VmDeploymentRequest()
            {
                Active = true,
                AftsID = 0,
                Config = config,
                ConfigOriginal = config, 
                CurrentStateStartTime = utcNow, 
                CurrentStateTryCount = 0,
                ExceptionMessage = null, 
                ExceptionTypeCode = null,
                ID = 0,
                LastStatusUpdate = utcNow,
                LastState = CmpInterfaceModel.Constants.StatusEnum.Complete.ToString(), 
                OverwriteExisting = false,
                ParentAppID = "",
                ParentAppName = "",
                RequestDescription = "SyncVm:" + vm.RoleName,
                RequestName = "SyncVm:" + vm.RoleName,
                RequestType = CmpInterfaceModel.Constants.RequestTypeEnum.SyncVm.ToString(), 
                ServiceProviderAccountID = spa.ID, 
                ServiceProviderResourceGroup = null,
                ServiceProviderStatusCheckTag = null, 
                SourceServerName = null,
                SourceServerRegion = null,
                SourceVhdFilesCSV = null,
                StatusCode = CmpInterfaceModel.Constants.StatusEnum.Complete.ToString(), 
                StatusMessage = "Imported from Azure Subscription",
                TagData = "<SyncReq></SyncReq>",
                TagID = 0,
                TargetAccount = spa.AccountID,
                TargetAccountCreds = null,
                TargetAccountType = CmpInterfaceModel.Constants.TargetAccountTypeEnum.AzureSubscription.ToString(),
                TargetLocation = null,
                TargetLocationType = null, 
                TargetServicename = vm.Label,
                TargetServiceProviderType = CmpInterfaceModel.Constants.TargetServiceProviderTypeEnum.Azure.ToString(),
                TargetVmName = vm.RoleName,
                ValidationResults = null,
                VmSize = vm.RoleSize,
                WhenRequested = utcNow,
                WhoRequested = "CmpWorkerService"
            };

            _cdb.InsertVmDepRequest(vdb);
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        /// <param name="spa"></param>
        /// <param name="vmDepReqs"></param>
        ///  
        //*********************************************************************

        //*** NOTE * Network

        private void ProcessSub(Models.ServiceProviderAccount spa, List<Models.VmDeploymentRequest> vmDepReqs)
        {
            try
            {
                var vmOps = new VmOps(new Connection(
                    spa.AccountID, spa.CertificateThumbprint, 
                    spa.AzureADTenantId, spa.AzureADClientId, spa.AzureADClientKey));

                var vmList = vmOps.FetchVmList();

                foreach (var vm in vmList)
                    ProcessFoundVm(vm, spa, vmDepReqs);

                //LogThis(EventLogEntryType.Information, "VmOp Request Submitted OK", 2, 2);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.ProcessorSyncSubs.ProcessSub()", 100, 1);
            }
        }

        #endregion

        #region --- State Processors ------------------------------------------

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        //*** NOTE * Network

        public object SyncWithAzureSubscriptions()
        {
            List<Models.VmDeploymentRequest> vmDepReqs = null;
            _cdb = new CmpDb(_CmpDbConnectionString);
            ReadConfigValues(_cdb);

            try
            {
                var spaList = _cdb.FetchServiceProviderAccountList(String.Empty);

                vmDepReqs = _cdb.FetchVmDepRequests(null, true);

                foreach (var spa in spaList)
                {
                    try
                    {
                        ProcessSub(spa, vmDepReqs);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in SyncWithAzureSubscriptions() " +
                                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }
            finally
            {
                try
                {
                    if (null != vmDepReqs)
                        vmDepReqs.Clear();
                }
                catch (Exception)
                {
                }
            }

            return 0;
        }

        #endregion
    }
}
