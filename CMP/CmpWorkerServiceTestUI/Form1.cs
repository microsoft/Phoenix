using System;
using System.Diagnostics;
using System.Windows.Forms;
using CmpCommon;

namespace CmpWorkerServiceTestUI
{
    public partial class Form1 : Form
    {
        public EventLog _EventLog = null;
        CmpServiceLib.CmpService _CS;

        public Form1()
        {
            InitializeComponent();
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            if (null != _CS)
                _CS.AsynchStop();

            this.Close();
        }

        private void button_OneRun_Click(object sender, EventArgs e)
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
                _CS.PerformSingleRun();
                //_CS.AsynchStart();

                MessageBox.Show("Single run complete", "Single Run Complete", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                //if (null != _EventLog)
                //    _EventLog.WriteEntry("Service Started", EventLogEntryType.Information, 2, 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show( Utilities.UnwindExceptionMessages(ex), "Single Run Exception", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                //if (null != _EventLog)
                //    _EventLog.WriteEntry("Service not started : " + CmpCommon.Utilities.UnwindExceptionMessages(ex), EventLogEntryType.Error, 100, 100);
            }
        }

        private void button_RunAsService_Click(object sender, EventArgs e)
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
                //_CS.PerformSingleRun();
                _CS.AsynchStart();

                if (null != _EventLog)
                    _EventLog.WriteEntry("Service Started", EventLogEntryType.Information, 2, 1);
            }
            catch (Exception ex)
            {
                if (null != _EventLog)
                    _EventLog.WriteEntry("Service not started : " + CmpCommon.Utilities.UnwindExceptionMessages(ex), EventLogEntryType.Error, 100, 100);
            }
        }
    }
}
