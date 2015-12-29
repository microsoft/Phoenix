using System;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CmpAzureServiceWebRole
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        static EventLog _EventLog = null;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        void InitEventlog()
        {
            if (null == _EventLog)
            {
                try
                {
                    _EventLog = new EventLog("Application");
                    _EventLog.Source = CmpCommon.Constants.CmpAzureServiceWebRole_EventlogSourceName;
                    _EventLog.WriteEntry("Service Starting", EventLogEntryType.Information, 1, 1);
                }
                catch (Exception)
                {
                    _EventLog = null;
                }

                Controllers.VmDeploymentsController.eventLog = _EventLog;
                Controllers.VmMigrationsController.eventLog = _EventLog;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        void InitCmpContextConnectionString()
        {
            try
            {
                var XK = new KryptoLib.X509Krypto(null);
                Controllers.VmDeploymentsController.cmpDbConnectionString =
                    XK.GetKTextConnectionString("CMPContext", "CMPContextPassword");
                Controllers.VmMigrationsController.cmpDbConnectionString =
                    Controllers.VmDeploymentsController.cmpDbConnectionString;
            }
            catch (Exception ex)
            {
                if (null != _EventLog)
                    _EventLog.WriteEntry("Exception when reading CMPContext connection string : " + 
                        ex.Message, EventLogEntryType.Error, 100, 100);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        protected void Application_Start()
        {
            InitEventlog();
            InitCmpContextConnectionString();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
