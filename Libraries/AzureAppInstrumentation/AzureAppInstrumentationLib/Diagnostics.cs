using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Diagnostics;

namespace AzureAppInstrumentationLib
{
    public class Diagnostics
    {
        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eLog"></param>
        /// <param name="diagnosticsConnectionString"></param>
        /// 
        //*********************************************************************

        public static void Init(EventLog eLog, string diagnosticsConnectionString)
        {
            try
            {
                /*
                 * commented out when migrated from Microsoft.WindowsAzure.Diagnostics 2.3 to 2.5
                 */ 
                 
                 
                // Fetch a copy of the existing config
                var diagConfig =
                    DiagnosticMonitor.GetDefaultInitialConfiguration();

                //***********************************************************
                //*** Event Logs ********************************************
                //***********************************************************

                diagConfig.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(5);
                diagConfig.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;

                //diagConfig.WindowsEventLog.DataSources.Add("System!*");
                diagConfig.WindowsEventLog.DataSources.Add("Application!*");

                //Specify the scheduled transfer
                diagConfig.WindowsEventLog.ScheduledTransferLogLevelFilter = LogLevel.Verbose;
                diagConfig.WindowsEventLog.ScheduledTransferPeriod = System.TimeSpan.FromMinutes(1);

                //***********************************************************
                //*** Perf Counters *****************************************
                //***********************************************************

                diagConfig.PerformanceCounters.DataSources.Add(
                    new PerformanceCounterConfiguration()
                    {
                        CounterSpecifier = @"\Processor(_Total)\% Processor Time",
                        SampleRate = TimeSpan.FromSeconds(60)
                    });

                diagConfig.PerformanceCounters.DataSources.Add(
                    new PerformanceCounterConfiguration()
                    {
                        CounterSpecifier = @"\Memory\Available Bytes",
                        SampleRate = TimeSpan.FromSeconds(60)
                    });

                diagConfig.PerformanceCounters.DataSources.Add(
                    new PerformanceCounterConfiguration()
                    {
                        CounterSpecifier = @"\ASP.NET\Applications Running",
                        SampleRate = TimeSpan.FromSeconds(60)
                    });

                diagConfig.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(5);

                //***********************************************************
                //*** Infrastructure Logs ***********************************
                //***********************************************************

                diagConfig.DiagnosticInfrastructureLogs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;
                diagConfig.DiagnosticInfrastructureLogs.ScheduledTransferPeriod = TimeSpan.FromMinutes(5);

                //***********************************************************
                //*** Trace *************************************************
                //***********************************************************

                var tmpListener = new DiagnosticMonitorTraceListener();
                System.Diagnostics.Trace.Listeners.Add(tmpListener);

                //***********************************************************
                //*** Startup ***********************************************
                //***********************************************************

                // Start diagnostics with this custom local buffering configuration 
                DiagnosticMonitor.Start(diagnosticsConnectionString, diagConfig);

                Trace.TraceInformation("Trace: Initialized OK");
                eLog.WriteEntry("Event: Initialized OK", EventLogEntryType.Information, 1, 1);
                
            }
            catch (Exception ex)
            {
                DiagnosticsUtils.WriteExceptionToBlobStorage(ex, "Exception in Initialization");
            }
        }
    }

    public class DiagnosticsUtils
    {
        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// 
        //*********************************************************************

        public static void WriteExceptionToBlobStorage(Exception ex)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(
                    RoleEnvironment.GetConfigurationSettingValue("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString"));

                var container = storageAccount.CreateCloudBlobClient().GetContainerReference("exceptions");
                container.CreateIfNotExist();

                var blob = container.GetBlobReference(string.Format("exception-{0}-{1}.log", RoleEnvironment.CurrentRoleInstance.Id, DateTime.UtcNow.Ticks));
                blob.UploadText(ex.ToString());
            }
            catch (Exception)
            {
                //Whatever
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="prefix"></param>
        /// 
        //*********************************************************************

        public static void WriteExceptionToBlobStorage(Exception ex, string prefix)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(
                    RoleEnvironment.GetConfigurationSettingValue("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString"));

                var container = storageAccount.CreateCloudBlobClient().GetContainerReference("exceptions");
                container.CreateIfNotExist();

                var blob = container.GetBlobReference(string.Format("exception-{0}-{1}.log", RoleEnvironment.CurrentRoleInstance.Id, DateTime.UtcNow.Ticks));
                blob.UploadText(prefix + "---" + ex.ToString());
            }
            catch (Exception)
            {
                //Whatever
            }
        }
    }

}
