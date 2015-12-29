using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace CmpWorkerService
{
    public partial class CmpWorker : ServiceBase
    {
        public EventLog _EventLog = null;
        CmpServiceLib.CmpService _CS;

        public CmpWorker()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _EventLog = new EventLog("Application");
                _EventLog.Source = CmpCommon.Constants.CmpAzureServiceWorkerRole_EventlogSourceName;
                _EventLog.WriteEntry("Service Starting", EventLogEntryType.Information, 1, 1);
            }
            catch (Exception)
            {
                _EventLog = null;
            }

            try
            {
                _CS = new CmpServiceLib.CmpService(_EventLog, null, null);
                _CS.AsynchStart();

                if (null != _EventLog)
                    _EventLog.WriteEntry("Service Started", EventLogEntryType.Information, 2, 1);
            }
            catch (Exception ex)
            {
                if (null != _EventLog)
                    _EventLog.WriteEntry("Service not started : " + CmpCommon.Utilities.UnwindExceptionMessages(ex), EventLogEntryType.Error, 100, 100);
            }

            //return base.OnStart();
        }

        protected override void OnStop()
        {
            if (null != _EventLog)
                _EventLog.WriteEntry("Service Stopping", EventLogEntryType.Information, 3, 1);

            try
            {
                _CS.AsynchStop();
            }
            catch (Exception)
            {
                //*** Ignore ***
            }

            if (null != _EventLog)
                _EventLog.WriteEntry("Service Stopped", EventLogEntryType.Information, 4, 1);

            base.OnStop();

        }
    }
}
