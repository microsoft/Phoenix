using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AzureAdminClientLib;
using CmpServiceLib.Models;

namespace CmpServiceLib
{
    public class ProcessorSyncAzureInfo : IDisposable
    {
        private static string _cmpDbConnectionString = null;
        private static EventLog _eventLog = null;

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
        public ProcessorSyncAzureInfo(EventLog eLog)
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

        public ProcessorSyncAzureInfo(EventLog eLog, string cmpDbConnectionString)
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

        //*** NOTE * Network
        //*** NOTE * Refresh

        public void ProcessSyncAzureInfo(string cmpDbConnectionString)
        {
            try
            {
                _cmpDbConnectionString = cmpDbConnectionString;

                _workerThreadList = new List<Thread>(1);

                AddProcessor(SyncWithAzure);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessOpsRequests() " + 
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- Async Methods --------------------------------------------

        readonly int _sleepTime = 24 * 60 * 60 * 1000;

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
        /// <summary>
        /// Sync the CmpWapDb AzureRegion and VmSize tables with Azure domain data
        /// </summary>
        /// 
        //*********************************************************************
        public object SyncWithAzure()
        {
            var ars = new AzureRefreshService(null, _cmpDbConnectionString);
            var azureCatalogue = ars.FetchAzureInformationWithArm();
            UpdateAzureRoleSizes(azureCatalogue);
            return 0;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        /// <param name="azureCatalogueSet"></param>
        ///  
        //*********************************************************************
        private void UpdateAzureRoleSizes(IEnumerable<AzureCatalogue> azureCatalogueSet)
        {
            try
            {
                var cmpDb = new CmpDb(_cmpDbConnectionString);

                var cmpVmSizes = cmpDb.FetchAzureRoleSizeList().ToList();
                var cmpWapVmSizeNames = cmpVmSizes.Select(vs => vs.Name).ToList();
                var azureCatalogueVmSizes = new List<AzureVmSizeArmData>();

                //Eliminate dupes, we only need them to establish the mappings in another method.
                foreach (var regionInCatalogue in azureCatalogueSet)
                {
                    azureCatalogueVmSizes.AddRange(regionInCatalogue.VmSizes);
                }
                azureCatalogueVmSizes = azureCatalogueVmSizes.Distinct(new AzureVmSizeArmData.AzureVmSizeComparer()).ToList();

                foreach (var vmSize in azureCatalogueVmSizes)
                {
                    if (!cmpWapVmSizeNames.Any(x => string.Equals(vmSize.name, x, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        /* Translating AzureVmSizeArmData object to AzureRoleSize object. They are essentially the same, 
                         * but with slightly different attributes. Azure gives them one way, CMP needs
                         * them another, hence the translation of the attributes that do match. Others are set with default values
                         */
                        var newVmSize = new AzureRoleSize
                        {
                            Name = vmSize.name,
                            CoreCount = vmSize.numberOfCores,
                            RamMb = vmSize.memoryInMB,
                            DiskSizeVmOs = vmSize.osDiskSizeInMB
                        };

                        cmpDb.InsertAzureRoleSize(newVmSize);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception caught in UpdateAzureRoleSizes: " + ex.ToString());
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

        private void LogThis(Exception ex, EventLogEntryType type, string prefix,
            int id, short category)
        {
            if (null == _eventLog)
                return;

            if (null == ex)
                _eventLog.WriteEntry(prefix, type, id, category);
            else
                _eventLog.WriteEntry(prefix + " : " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex),
                    type, id, category);
        }

        #endregion
    }
}
