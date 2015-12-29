using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using Microsoft.Web.Administration;

namespace CmpAzureServiceWebRole
{
    public class WebRole : RoleEntryPoint
    {
        EventLog _eventLog = null;
        string _cmpDbConnectionString = null;
        string _aftsDbConnectionString = null;

        const string DiagnosticsConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
        const string SiteNameFromServiceModel = "Web";
        const string SslConfigFlags = @"Ssl,SslRequireCert,SslNegotiateCert";  

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
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            Init();
            return base.OnStart();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        private void Init()
        {
            try
            {
                InitEventlog();
                InitDiagnostics();
                GetDbConnectionString();
                FetchUserAccounts();
                AddHandlers();
                InitSsl();
            }
            catch (Exception ex)
            {
                if (null != _eventLog)
                    _eventLog.WriteEntry("Site Initialization Failure", EventLogEntryType.Error, 10, 10);
            }

            if (null != _eventLog)
                _eventLog.WriteEntry("Initialized OK", EventLogEntryType.Information, 2, 1);
        }

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
                AzureAppInstrumentationLib.Diagnostics.Init(_eventLog, DiagnosticsConnectionString);

                if (null != _eventLog)
                    _eventLog.WriteEntry(
                        "Windows Azure Diagnostics Initialized OK", EventLogEntryType.Information, 1, 1);
            }
            catch (Exception ex)
            {
                if (null != _eventLog)
                    _eventLog.WriteEntry(
                        "Startup Exception: Problem initializing Windows Azure Diagnostics : " + 
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex),
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

        private void InitEventlog()
        {
            try
            {
                _eventLog = new EventLog("Application")
                {
                    Source = CmpCommon.Constants.CmpAzureServiceWebRole_EventlogSourceName
                };
                _eventLog.WriteEntry("Service Starting", EventLogEntryType.Information, 1, 1);
            }
            catch (Exception)
            {
                _eventLog = null;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        private void InitSsl()
        {
            try
            {
                var clientAuthMechanism =
                    Microsoft.Azure.CloudConfigurationManager.GetSetting("ClientAuthMechanism");

                if (null == clientAuthMechanism)
                    return;

                if ( !clientAuthMechanism.Equals(
                        CmpInterfaceModel.Constants.ClinetAuthMechanismEnum.ClientCert.ToString(),
                        StringComparison.InvariantCultureIgnoreCase))
                    return;

                using (var server = new ServerManager())
                {
                    var siteName = string.Format("{0}_{1}", RoleEnvironment.CurrentRoleInstance.Id, SiteNameFromServiceModel);
                    var config = server.GetApplicationHostConfiguration();
                    var accessSection = config.GetSection("system.webServer/security/access", siteName);
                    accessSection["sslFlags"] = SslConfigFlags;

                    server.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                if (null != _eventLog)
                    _eventLog.WriteEntry(
                        "Startup Exception: Exception in InitSSL() : " +
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex),
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

        private void GetDbConnectionString()
        {
            try
            {
                var xk = new KryptoLib.X509Krypto(null);
                _cmpDbConnectionString = xk.GetKTextConnectionString(
                    "CMPContext","CMPContextPassword");

                if (null == _cmpDbConnectionString)
                {
                    if(null != _eventLog)
                        _eventLog.WriteEntry(
                            "Startup Exception: Unable to get DB connection string. Check the .config file 'CMPContext' and 'CMPContextPassword' keys", 
                            EventLogEntryType.Error, 10, 10);
                    return;
                }

                if(null != _eventLog)
                    _eventLog.WriteEntry("CMP DB Connection string : " + 
                        GetClippenConnectionString(_cmpDbConnectionString), 
                        EventLogEntryType.Information, 1, 1);
            }
            catch (Exception ex)
            {
                if(null != _eventLog)
                    _eventLog.WriteEntry(
                        "Startup Exception: Exception in GetDbConnectionString() CMP : " + 
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex), 
                        EventLogEntryType.Error, 10, 10);
            }

            //*** Uncomment for AFTS support ***
            /*try
            {
                var xk = new KryptoLib.X509Krypto(null);
                _aftsDbConnectionString = xk.GetKTextConnectionString(
                    "AzureFileTransferContext", "AzureFileTransferContextPassword");

                if (null == _aftsDbConnectionString)
                {
                    if (null != _eventLog)
                        _eventLog.WriteEntry(
                            "Startup Exception: Unable to get AFTS DB connection string. Check the .config file 'AzureFileTransferContext' and 'AzureFileTransferContextPassword' keys",
                            EventLogEntryType.Error, 10, 10);
                    return;
                }

                if (null != _eventLog)
                    _eventLog.WriteEntry("AFTS DB Connection string : " +
                        GetClippenConnectionString(_cmpDbConnectionString),
                        EventLogEntryType.Information, 1, 1);
            }
            catch (Exception ex)
            {
                if (null != _eventLog)
                    _eventLog.WriteEntry(
                        "Startup Exception: Exception in GetDbConnectionString() AFTS : " +
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex),
                        EventLogEntryType.Error, 10, 10);
            }*/
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchUserAccounts()
        {
            return;

            try
            {
                var cs = new CmpServiceLib.CmpService(
                    _eventLog, _cmpDbConnectionString, _aftsDbConnectionString);

                var userAcountList = cs.FetchUserAccounts();

                if (null == userAcountList)
                {
                    userAcountList = WebHostBasicAuth.Modules.BasicAuthHttpModule.GetDefaultAccountList();

                    if (null != _eventLog)
                        _eventLog.WriteEntry("Using default service user account.",
                            EventLogEntryType.Information, 1, 1);
                }
                else if (0 == userAcountList.Count)
                {
                    userAcountList = WebHostBasicAuth.Modules.BasicAuthHttpModule.GetDefaultAccountList();

                    if (null != _eventLog)
                        _eventLog.WriteEntry("Using default service user account.",
                            EventLogEntryType.Information, 1, 1);
                }
                else
                    if (null != _eventLog)
                        _eventLog.WriteEntry("Using DB source service user accounts.",
                            EventLogEntryType.Information, 1, 1);
                
                WebHostBasicAuth.Modules.BasicAuthHttpModule.UserList =
                    new List<WebHostBasicAuth.Modules.BasicCreds>();

                foreach (var UA in userAcountList)
                    WebHostBasicAuth.Modules.BasicAuthHttpModule.UserList.Add(
                        new WebHostBasicAuth.Modules.BasicCreds(UA.Name, UA.Password));
                
                if (null != _eventLog)
                    _eventLog.WriteEntry("Service user accounts set OK", 
                        EventLogEntryType.Information, 1, 1);
            }
            catch (Exception ex)
            {
                if (null != _eventLog)
                    _eventLog.WriteEntry("Startup Exception: Unable to fetch CMP service user accounts : " +
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex), 
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

        private void AddHandlers()
        {
            //System.Web.Http.GlobalConfiguration.Configuration.MessageHandlers.Add(
            //    new Controllers.CustomCertificateMessageHandler());

            System.Web.Http.GlobalConfiguration.Configuration.MessageHandlers.Add(
                new CustomCertificateMessageHandler());

            //System.Web.Http.GlobalConfiguration.Configuration.MessageHandlers.Add(
            //    new Controllers.BasicAuthMessageHandler());
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        string GetClippenConnectionString(string connectionString)
        {
            string clippedString = null;

            try
            {
                var index = _cmpDbConnectionString.IndexOf("Password=");
                clippedString = _cmpDbConnectionString.Substring(0, index + 9) + "XXX";
                var index2 = _cmpDbConnectionString.Substring(index).IndexOf(";");
                clippedString += _cmpDbConnectionString.Substring(index + index2);
            }
            catch (Exception)
            {
                clippedString = "[MALFORMED]";
            }

            return clippedString;
        }
    }
}
