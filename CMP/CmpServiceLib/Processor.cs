using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CmpServiceLib
{
    class Processor
    {
        /*const string VMIMAGE_EULA           = "This is the EULA";
        const string VMIMAGE_DESCRIPTION    = "This image was built from";
        const string VMIMAGE_FAMILY         = "MSIT";

        const string VmProcessorCommandSetup = "Import-Module ‘D:\\FLUMigrationModules\\VMCloudMigration.psm1‘";
        const string VmProcessorCommandTemplate = "GET-VMDETAILSONPREM -Id {0} -CallBack {1} -VMName {2} -AzureName {3}";*/

        readonly EventLog _eventLog = null;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public Processor(EventLog eLog)
        {
            _eventLog = eLog;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aftsStatList"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        AftsStatus CalculateCumulativeAftsStatus(List<AftsStatus> aftsStatList)
        {
            var atfsStat = new AftsStatus 
                {StatusCode = "NA"};

            foreach (var atfss in aftsStatList)
            {
                switch (atfss.StatusCode)
                {
                    case "COMPLETE":

                        if (atfsStat.StatusCode.Equals("NA"))
                            atfsStat.StatusCode = "COMPLETE";

                        break;

                    case "FAILEDNORETRY":

                        atfsStat.StatusCode = "FAILEDNORETRY";

                        if (null == atfsStat.ResultDescription)
                            atfsStat.ResultDescription = "AFTS Exception : ";

                        atfsStat.ResultDescription += atfss.ResultDescription + "---";

                        break;

                    case "TRANSFERRING":

                        if (!atfsStat.StatusCode.Equals("FAILEDNORETRY"))
                            atfsStat.StatusCode = "TRANSFERRING";

                        break;
                }
            }

            return atfsStat;
        }

        //*********************************************************************
        //
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public void PerformSingleRun(string cmpDbConnectionString, string aftsDbConnectionString)
        {
            var processVms =
                Microsoft.Azure.CloudConfigurationManager.GetSetting("ProcessVmsActive") as string;

            if (null == processVms)
                processVms = "T";

            if (processVms.Contains("T"))
            {
                ProcessVmRequests(cmpDbConnectionString, aftsDbConnectionString);
                ProcessOpsRequests(cmpDbConnectionString);
                ProcessSyncAzureSubs(cmpDbConnectionString);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public void ProcessVmRequests(string cmpDbConnectionString, string aftsDbConnectionString)
        {
            using (var pvm = new ProcessorVm(_eventLog))
            {
                pvm.ProcessVmRequests(cmpDbConnectionString, aftsDbConnectionString);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmpDbConnectionString"></param>
        /// 
        //*********************************************************************

        public void ProcessOpsRequests(string cmpDbConnectionString)
        {
            using (var po = new ProcessorOps(_eventLog))
            {
                po.ProcessOpsRequests(cmpDbConnectionString);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmpDbConnectionString"></param>
        /// 
        //*********************************************************************

        public void ProcessSyncAzureSubs(string cmpDbConnectionString)
        {
            try
            {
                using (var pss = new ProcessorSyncSubs(_eventLog))
                {
                    pss.ProcessSyncAzureSubs(cmpDbConnectionString);
                }

                LogThis(null, EventLogEntryType.Information,
                    "One run ProcessSyncAzureSubs() complete", 0, 0);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessSyncAzureSubs() : " + 
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        ProcessorVm _pvm = null;
        ProcessorOps _po = null;
        private ProcessorSyncSubs _pss = null;
        private ProcessorSyncAzureInfo _psai = null;

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="cmpDbConnectionString"></param>
        /// <param name="aftsDbConnectionString"></param>
        ///  
        //**********************************************************************

        public void AsynchStart(string cmpDbConnectionString, string aftsDbConnectionString)
        {
            try
            {
                _pvm = new ProcessorVm(_eventLog);
                _pvm.ProcessVmRequestsAsynch(cmpDbConnectionString, aftsDbConnectionString);

                _po = new ProcessorOps(_eventLog);
                _po.ProcessOpsRequestsAsynch(cmpDbConnectionString);

                _pss = new ProcessorSyncSubs(_eventLog);
                _pss.ProcessSyncAzureSubsAsync(cmpDbConnectionString);

                _psai = new ProcessorSyncAzureInfo(_eventLog);
                _psai.ProcessSyncAzureInfo(cmpDbConnectionString);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, 
                    "Exception in CmpServiceLib.Processor.AsynchStart()", 0, 0);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public void AsynchStop()
        {
            if (null != _pvm)
            {
                try
                {
                    _pvm.StopProcessingAsynch();
                    _po.StopProcessingAsynch();
                    _pss.StopProcessingAsynch();
                    _psai.StopProcessingAsynch();
                }
                catch (Exception)
                {
                }

                try
                {
                    _pvm.Dispose();
                    _po.Dispose();
                    _pss.Dispose();
                    _psai.Dispose();
                }
                catch (Exception)
                {
                }
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

        private void LogThis(Exception ex, EventLogEntryType type,
            string prefix, int id, short category)
        {
            if (null != _eventLog)
            {
                if (null == ex)
                    _eventLog.WriteEntry(prefix + " : ", type, id, category);
                else
                    _eventLog.WriteEntry(prefix + " : " +
                        CmpCommon.Utilities.UnwindExceptionMessages(ex), type, id, category);
            }
        }
    }
}
