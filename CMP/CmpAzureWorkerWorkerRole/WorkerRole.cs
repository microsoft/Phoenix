using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace CmpAzureWorkerWorkerRole
{
    public class WorkerRole : RoleEntryPoint, IDisposable
    {
        public EventLog _EventLog = null;
        CmpServiceLib.CmpService _CS;

        // The name of your queue
        const string QueueName = "ProcessingQueue";
        const string _DiagnosticsConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        const QueueClient Client = null;
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        private void InitDiagnostics()
        {
            try
            {
                AzureAppInstrumentationLib.Diagnostics.Init(_EventLog, _DiagnosticsConnectionString);

                if (null != _EventLog)
                    _EventLog.WriteEntry(
                        "Windows Azure Diagnostics Initialized OK", EventLogEntryType.Information, 1, 1);
            }
            catch (Exception ex)
            {
                if (null != _EventLog)
                    _EventLog.WriteEntry(
                        "Startup Exception: Problem initializing Windows Azure Diagnostics : " +
                        CmpCommon.Utilities.UnwindExceptionMessages(ex),
                        EventLogEntryType.Error, 10, 10);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public void Dispose()
        { }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        bool JoinDomain()
        {
            string currectDomainName = null;

            try
            {
                if (ActiveDirLib.DomainJoin.IsDomainJoined(out currectDomainName))
                {
                    if (null != _EventLog)
                        _EventLog.WriteEntry("Domain Joined : " + currectDomainName, EventLogEntryType.Information, 10, 1);

                    return false;
                }
            }
            catch (Exception ex)
            {
                if (null != _EventLog)
                    _EventLog.WriteEntry("Exception while detecting domain : " + 
                        CmpCommon.Utilities.UnwindExceptionMessages(ex), EventLogEntryType.Error, 100, 100);
                return true;
            }

            try
            {
                if (null != _EventLog)
                    _EventLog.WriteEntry("Joining domain", EventLogEntryType.Information, 10, 1);

                var domainName = CloudConfigurationManager.GetSetting("CmpWorker.DomainJoin.DomainName");
                var userName = CloudConfigurationManager.GetSetting("CmpWorker.DomainJoin.UserName");
                var userPassword = CloudConfigurationManager.GetSetting("CmpWorker.DomainJoin.UserPassword");
                //string destinationOU = CloudConfigurationManager.GetSetting("CmpWorker.DomainJoin.OU");
                string destinationOU = null;

                using (var XK = new KryptoLib.X509Krypto())
                    userPassword = XK.DecrpytKText(userPassword);

                ActiveDirLib.DomainJoin.Join(domainName, userPassword, userName, destinationOU);
            }
            catch (Exception ex)
            {
                if (null != _EventLog)
                    _EventLog.WriteEntry("Exception while joining domain : " +
                        CmpCommon.Utilities.UnwindExceptionMessages(ex), EventLogEntryType.Error, 100, 100);
                return true;
            }

            try
            {
                Thread.Sleep(20000);

                if (null != _EventLog)
                    _EventLog.WriteEntry("Restarting after joining domain", EventLogEntryType.Information, 10, 1);

                System.Diagnostics.Process.Start("shutdown", "/r"); 
            }
            catch (Exception ex)
            {
                if (null != _EventLog)
                    _EventLog.WriteEntry("Exception while restarting after joining domain : " +
                        CmpCommon.Utilities.UnwindExceptionMessages(ex), EventLogEntryType.Error, 100, 100);
                return true;
            }

            return true;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            if (null != Client)
                Client.OnMessage((receivedMessage) =>
                    {
                        try
                        {
                            // Process the message
                            Trace.WriteLine("Processing Service Bus message: " + receivedMessage.SequenceNumber.ToString());
                        }
                        catch
                        {
                            // Handle any message processing specific exceptions here
                        }
                    });

            CompletedEvent.WaitOne();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public override bool OnStart()
        {
            try
            {
                _EventLog = new EventLog("Application");
                _EventLog.Source = CmpCommon.Constants.CmpAzureServiceWorkerRole_EventlogSourceName;
                _EventLog.WriteEntry("Service Starting", EventLogEntryType.Information, 1, 1);
            }
            catch(Exception)
            {
                _EventLog = null;
            }

            try
            {
                InitDiagnostics();

                if (JoinDomain())
                    return false;

                // Set the maximum number of concurrent connections 
                ServicePointManager.DefaultConnectionLimit = 12;

                // Create the queue if it does not exist already
                var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");

                var xk = new KryptoLib.X509Krypto(null);
                connectionString = xk.DecrpytKText(connectionString);

                var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
                if (!namespaceManager.QueueExists(QueueName))
                {
                    namespaceManager.CreateQueue(QueueName);
                }

                // Initialize the connection to Service Bus Queue
                //Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);

                _CS = new CmpServiceLib.CmpService(_EventLog, null, null);
                //_CS.PerformSingleRun();
                _CS.AsynchStart();

                if (null != _EventLog)
                    _EventLog.WriteEntry("Service Started", EventLogEntryType.Information, 2, 1);
            }
            catch(Exception ex)
            {
                if (null != _EventLog)
                    _EventLog.WriteEntry("Service not started : " + CmpCommon.Utilities.UnwindExceptionMessages(ex), EventLogEntryType.Error, 100, 100);
            }

            return base.OnStart();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public override void OnStop()
        {
            if (null != _EventLog)
                _EventLog.WriteEntry("Service Stopping", EventLogEntryType.Information, 3, 1);

            // Close the connection to Service Bus Queue
            if (null != Client)
                Client.Close();

            CompletedEvent.Set();

            _CS.AsynchStop();

            if (null != _EventLog)
                _EventLog.WriteEntry("Service Stopped", EventLogEntryType.Information, 4, 1);

            base.OnStop();
        }
    }
}

