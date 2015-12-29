using System;
using System.Collections.Generic;
using System.Diagnostics;
using CmpInterfaceModel;
using CmpInterfaceModel.Models;
using System.Threading;

namespace CmpServiceLib
{
    //**********************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    ///         
    //**********************************************************************

    public class ProcessorOps : IDisposable
    {
        private const int OpsTtl = 480;
        private static string _cmpDbConnectionString = null;
        private static EventLog _eventLog = null;

        private bool _submittedQueueBlocked = false;
        private bool _allQueuesBlocked = false;

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
        public ProcessorOps(EventLog eLog)
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

        public ProcessorOps(EventLog eLog, string cmpDbConnectionString)
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
            _eventLog = eLog;

            /*_autoLocalAdminUserName =
                Microsoft.Azure.CloudConfigurationManager.GetSetting("AUTOLOCALADMINUSERNAME");

            _vmsPerVnetLimit_t =
                Microsoft.Azure.CloudConfigurationManager.GetSetting("VMSPERVNETLIMIT");

            if (null != _vmsPerVnetLimit_t)
                if (!int.TryParse(_vmsPerVnetLimit_t, out _vmsPerVnetLimit))
                    throw new Exception("Non integer value specified for VMSPERVNETLIMIT in app.config");*/

            _cmpDbConnectionString = cmpDbConnectionString;
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

        public void ProcessOpsRequests(string cmpDbConnectionString)
        {
            try
            {
                _cmpDbConnectionString = cmpDbConnectionString;
                ProcessOpsSubmissions();
                ProcessOpsProcessing();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessOpsRequests() " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion
        #region --- Async Methods --------------------------------------------

        int _SleepTime = 5 * 1000;

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

        public void ProcessOpsRequestsAsynch( string cmpDbConnectionString)
        {
            try
            {
                _cmpDbConnectionString = cmpDbConnectionString;

                _workerThreadList = new List<Thread>(2);

                AddProcessor(ProcessOpsSubmissions);
                AddProcessor(ProcessOpsProcessing);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessOpsRequestsAsynch() " +
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
                    "Exception in CmpServiceLib.ProcessorOps.Worker() setup", 100, 100);
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
                        "Exception in CmpServiceLib.ProcessorOps.Worker(" + methodName + ")", 100, 100);
                }

                Thread.Sleep(_SleepTime);
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
            if (null == _eventLog)
                return;

            if (null == ex)
                _eventLog.WriteEntry(prefix, type, id, category);
            else
                _eventLog.WriteEntry(prefix + " : " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex), type, id, category);
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
                            _defaultStateTtlMinutes = Convert.ToInt32(Microsoft.Azure.CloudConfigurationManager.GetSetting("DefaultStateTtlMinutes"));
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opsReq"></param>
        /// 
        //*********************************************************************

        private void ProcessOp(Models.OpRequest opsReq)
        {
            try
            {
                //LogThis(EventLogEntryType.Information, "VmOp Request Submitted", 2, 1);

                var opcode = CmpInterfaceModel.Constants.VmOpcodeEnum.Undefined;

                var opSpec = CmpInterfaceModel.Utilities.DeSerialize(typeof(OpSpec), opsReq.Config) as OpSpec;

                if(null == opSpec)
                {
                    opsReq.StatusMessage = "Config of given Ops Request could not be deserialized";
                    opsReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                    return;
                }

                try
                {
                    opcode = (CmpInterfaceModel.Constants.VmOpcodeEnum)Enum.Parse(
                        typeof(CmpInterfaceModel.Constants.VmOpcodeEnum), opsReq.RequestType);
                }
                catch (Exception)
                {
                    opsReq.StatusMessage = "Unknown opcode '" + opsReq.RequestType + "'";
                    opsReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                    return;
                }

                CmpServiceLib.CmpService cmp = null;

                switch (opcode)
                {
                    case CmpInterfaceModel.Constants.VmOpcodeEnum.START:
                        cmp = new CmpServiceLib.CmpService(_eventLog, _cmpDbConnectionString, null);
                        opsReq.ServiceProviderStatusCheckTag = cmp.VmStart(opSpec.TargetId);
                        break;
                    case CmpInterfaceModel.Constants.VmOpcodeEnum.STOP:
                        cmp = new CmpServiceLib.CmpService(_eventLog, _cmpDbConnectionString, null);
                        opsReq.ServiceProviderStatusCheckTag = cmp.VmStop(opSpec.TargetId);
                        break;
                    case CmpInterfaceModel.Constants.VmOpcodeEnum.DEALLOCATE:
                        cmp = new CmpServiceLib.CmpService(_eventLog, _cmpDbConnectionString, null);
                        opsReq.ServiceProviderStatusCheckTag = cmp.VmDeallocate(opSpec.TargetId);
                        break;
                    case CmpInterfaceModel.Constants.VmOpcodeEnum.RESIZE:
                        cmp = new CmpServiceLib.CmpService(_eventLog, _cmpDbConnectionString, null);
                        opsReq.ServiceProviderStatusCheckTag = cmp.VmResize(opSpec.TargetId, opSpec.Vmsize);
                        break;
                    case CmpInterfaceModel.Constants.VmOpcodeEnum.ADDISK:
                        cmp = new CmpServiceLib.CmpService(_eventLog, _cmpDbConnectionString, null);
                        opsReq.ServiceProviderStatusCheckTag = cmp.VmAddDisk(opSpec.TargetId, opSpec.Disks);
                        break;
                    case CmpInterfaceModel.Constants.VmOpcodeEnum.RESTART:
                        cmp = new CmpServiceLib.CmpService(_eventLog, _cmpDbConnectionString, null);
                        opsReq.ServiceProviderStatusCheckTag = cmp.VmRestart(opSpec.TargetId);
                        break;
                    case CmpInterfaceModel.Constants.VmOpcodeEnum.DELETE:
                        cmp = new CmpServiceLib.CmpService(_eventLog, _cmpDbConnectionString, null);
                        opsReq.ServiceProviderStatusCheckTag = cmp.VmDelete(opSpec.TargetId, false, false);
                        break;
                    case CmpInterfaceModel.Constants.VmOpcodeEnum.DELETEFROMSTORAGE:
                        cmp = new CmpServiceLib.CmpService(_eventLog, _cmpDbConnectionString, null);
                        opsReq.ServiceProviderStatusCheckTag = cmp.VmDelete(opSpec.TargetId, true, false);
                        break;
                    case CmpInterfaceModel.Constants.VmOpcodeEnum.DELETEONEXCEPTION:
                        cmp = new CmpServiceLib.CmpService(_eventLog, _cmpDbConnectionString, null);
                        opsReq.ServiceProviderStatusCheckTag = cmp.VmDelete(opSpec.TargetId, true, false);
                        break;
                    case CmpInterfaceModel.Constants.VmOpcodeEnum.DETACH:
                        cmp = new CmpServiceLib.CmpService(_eventLog, _cmpDbConnectionString, null);
                        opsReq.ServiceProviderStatusCheckTag = cmp.DetachDisk(opSpec.TargetId, opSpec.Disks[0], false);
                        break;
                    case CmpInterfaceModel.Constants.VmOpcodeEnum.DETACHANDDELETE:
                        cmp = new CmpServiceLib.CmpService(_eventLog, _cmpDbConnectionString, null);
                        opsReq.ServiceProviderStatusCheckTag = cmp.DetachDisk(opSpec.TargetId, opSpec.Disks[0], true);
                        break;
                    case CmpInterfaceModel.Constants.VmOpcodeEnum.ATTACHEXISTING:
                        cmp = new CmpServiceLib.CmpService(_eventLog, _cmpDbConnectionString, null);
                        opsReq.ServiceProviderStatusCheckTag = cmp.AttachExistingDisk(opSpec.TargetId, opSpec.Disks[0]);
                        break;
                    case CmpInterfaceModel.Constants.VmOpcodeEnum.Undefined:
                        break;
                }

                opsReq.StatusMessage = Constants.StatusEnum.Processing.ToString();
                opsReq.StatusCode = Constants.StatusEnum.Processing.ToString();

                //LogThis(EventLogEntryType.Information, "VmOp Request Submitted OK", 2, 2);
            }
            catch (Exception ex)
            {
                    LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.OpsController.SubmitOp()", 100, 1);
                    //throw new Microsoft.WindowsAzurePack.CmpWapExtension.Common.PortalException(ex1.Message);
                    opsReq.StatusMessage = Constants.StatusEnum.Exception.ToString();
                    opsReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                    opsReq.ExceptionMessage = "Exception in ProcessOp(): "+ CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex);
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

        public object ProcessOpsSubmissions()
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            ReadConfigValues(cdb);

            if (_submittedQueueBlocked)
                return 0;

            List<Models.OpRequest> opsReqList = null;

            try
            {
                opsReqList = cdb.FetchOpsRequests(
                    Constants.StatusEnum.Submitted.ToString(), true);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessOpsSubmissions() " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }

            foreach (var opsReq in opsReqList)
            {
                try
                {
                    ProcessOp(opsReq);

                    opsReq.CurrentStateStartTime = DateTime.UtcNow;

                    if (null == opsReq.CurrentStateTryCount)
                        opsReq.CurrentStateTryCount = 0;

                    cdb.SetOpsRequestStatus(opsReq, null);
                }
                catch (Exception ex)
                {
                    opsReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                    opsReq.ExceptionMessage = "Exception in ProcessOpsSubmissions() " +
                        Utilities.UnwindExceptionMessages(ex);
                    //Utilities.SetVmReqExceptionType(opsReq,
                    //    Constants.RequestExceptionTypeCodeEnum.Admin);
                    cdb.SetOpsRequestStatus(opsReq, null);
                }
            }

            return 0;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public object ProcessOpsProcessing()
        {
            var cdb = new CmpDb(_cmpDbConnectionString);
            ReadConfigValues(cdb);

            if (_submittedQueueBlocked)
                return 0;

            List<Models.OpRequest> opsReqList = null;

            try
            {
                opsReqList = cdb.FetchOpsRequests(
                    Constants.StatusEnum.Processing.ToString(), true);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessOpsProcessing() " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }

            var cmp = new CmpServiceLib.CmpService(_eventLog, _cmpDbConnectionString, null);

            foreach (var opsReq in opsReqList)
            {
                try
                {
                    var opSpec = CmpInterfaceModel.Utilities.DeSerialize(
                        typeof(OpSpec), opsReq.Config) as OpSpec;

                    if(null == opSpec)
                        throw new Exception("Unable to deserialize opsReq.Config");

                    var resp = cmp.CheckOpsStatus(opSpec.TargetId, opsReq.ServiceProviderStatusCheckTag);

                    if (resp.HadError)
                    {
                        if (resp.Body.Contains("(503)"))
                            continue;

                        if (resp.Body.Contains("retry"))
                            continue;

                        if (resp.Body.Contains("try again"))
                            continue;

                        opsReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                        opsReq.StatusMessage = resp.Body;
                        opsReq.ExceptionMessage = "Error in ProcessOpsProcessing() : " + resp.Body;
                    }
                    else
                    {
                        var stat = new AzureAdminClientLib.AzureAdminTaskStatus(resp.Body, opsReq.RequestType);

                        switch (stat.Result)
                        {
                            case AzureAdminClientLib.AzureAdminTaskStatus.ResultEnum.Succeeded:
                                if (stat.Status.Equals("InProgress"))
                                {
                                    if (HasTimedOut(opsReq, OpsTtl))
                                    {
                                        opsReq.StatusCode = Constants.StatusEnum.Agedout.ToString();
                                        opsReq.StatusMessage = Constants.StatusEnum.Agedout.ToString();
                                    }
                                    else
                                        continue;
                                }
                                else
                                {
                                    opsReq.StatusCode = Constants.StatusEnum.Complete.ToString();
                                    opsReq.StatusMessage = resp.Body;
                                    opsReq.ExceptionMessage = "";
                                }

                                break;

                            case AzureAdminClientLib.AzureAdminTaskStatus.ResultEnum.Success:
                                if (stat.Status.Equals("InProgress"))
                                {
                                    if (HasTimedOut(opsReq, OpsTtl))
                                    {
                                        opsReq.StatusCode = Constants.StatusEnum.Agedout.ToString();
                                        opsReq.StatusMessage = Constants.StatusEnum.Agedout.ToString();
                                    }
                                    else
                                        continue;
                                }
                                else
                                {
                                    opsReq.StatusCode = Constants.StatusEnum.Complete.ToString();
                                    opsReq.StatusMessage = resp.Body;
                                    opsReq.ExceptionMessage = "";
                                }

                                break;

                            case AzureAdminClientLib.AzureAdminTaskStatus.ResultEnum.Failed:

                                if (stat.ErrorMessage.ToLower().Contains("(503)"))
                                    continue;

                                if (stat.ErrorMessage.ToLower().Contains("retry"))
                                    continue;

                                if (stat.ErrorMessage.ToLower().Contains("try again"))
                                    continue;

                                opsReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                                opsReq.StatusMessage = resp.Body;
                                opsReq.ExceptionMessage = "Exception in ProcessOpsProcessing() : Code: '" +
                                    stat.ErrorCode + "', Message: " + stat.ErrorMessage;

                                break;
                        }
                    }

                    opsReq.CurrentStateStartTime = DateTime.UtcNow;

                    if (null == opsReq.CurrentStateTryCount)
                        opsReq.CurrentStateTryCount = 0;

                    cdb.SetOpsRequestStatus(opsReq, null);
                }
                catch (Exception ex)
                {
                    opsReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                    opsReq.ExceptionMessage = "Exception in ProcessOpsProcessing() " +
                        Utilities.UnwindExceptionMessages(ex);
                    //Utilities.SetVmReqExceptionType(opsReq,
                    //    Constants.RequestExceptionTypeCodeEnum.Admin);
                    cdb.SetOpsRequestStatus(opsReq, null);
                }
            }

            return 0;
        }

        #endregion
    }
}

