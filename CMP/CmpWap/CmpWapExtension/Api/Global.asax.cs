// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Diagnostics;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api
{
    public class CmpWapExtensionApi : HttpApplication
    {
        static EventLog _eventLog;

        /// <summary></summary>
        public static EventLog Log
        {
            set { _eventLog = value; }
            get
            {
                if (null == _eventLog)
                {
                    try
                    {
                        _eventLog = new EventLog("Application")
                        {
                            Source = CmpCommon.Constants.CmpWapConnector_EventlogSourceName
                        };
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }

                return _eventLog;
            }
        }

        protected void Application_Start()
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            SyncWorker.StartAsync(Log);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.AddHeader("Pragma", "no-cache"); // HTTP 1.0.
            Response.AddHeader("Expires", "0"); // Proxies.
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            
        }
    }
}