//*****************************************************************************
// File: ScriptJobsController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class provides methods to perform Script Job related activities.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Web.Http;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System.Diagnostics;
using System.Linq;
using System.Web.Http.OData;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    /// <remarks>
    ///     This class provides methods to perform Script Job related activities.
    /// </remarks>
    public class ScriptJobsController : ApiController
    {
        //*********************************************************************
        /// 
        ///  <summary>
        ///     This mehtod is used to submit script jobs for execution.
        ///  </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="scriptJob">Script Job Object</param>
        ///  
        //*********************************************************************
        [HttpPost]
        public IHttpActionResult SubmitScriptJob(
            string subscriptionId, [FromBody] ScriptJob scriptJob)
        {
            try
            {
                LogThis(EventLogEntryType.Information, "ScriptJob Submitted", 2, 1);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                scriptJob = ScriptInterface.SubmitScriptJob(scriptJob, subscriptionId);
                LogThis(EventLogEntryType.Information, "ScriptJob Submitted OK", 2, 2);

                return Ok(scriptJob);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "CmpWapExtension.ScriptJobsController.SubmitScriptJob()", 100, 1);
                return InternalServerError(ex);
            }
        }

        #region --- Get ------------------------

        //*********************************************************************
        ///
        /// <summary>
        ///  This GET method is used to list script jobs being executed for a 
        ///  particular subscription Id.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns>list of script jobs</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        [EnableQuery]
        public IQueryable<ScriptJob> ListScriptJobs(string subscriptionId)
        {
            try
            {
                return ScriptInterface.GetScriptJobList(subscriptionId).AsQueryable();
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "CmpWapExtension.ScriptJobsController.ListScriptJobs()", 100, 1);
                throw;
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  This method gets a particular script job based on a particular 
        ///  subscription Id and SMA job Id.
        ///  </summary>
        ///  <param name="subscriptionId"></param>
        ///  <param name="smaJobId"></param>
        ///  
        //*********************************************************************

        [HttpGet]
        public ScriptJob GetScriptJob(string subscriptionId, string smaJobId)
        {
            try
            {
                return ScriptInterface.GetScriptJob(subscriptionId, smaJobId);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "CmpWapExtension.ScriptJobsController.GetScriptJob()", 100, 1);
                throw;
            }
        }

        #endregion

        #region --- Utilities -------------------------------------------------

        static EventLog _eventLog = null;

        /// <summary></summary>
        public static EventLog EventLog 
        { 
            set { _eventLog = value; }
            get 
            {
                if (null == _eventLog)
                {
                    try
                    {
                        _eventLog = new EventLog("Application");
                        _eventLog.Source = CmpCommon.Constants.CmpWapConnector_EventlogSourceName;
                    }
                    catch(Exception)
                    {
                        return null;
                    }
                }

                return _eventLog;
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
            try
            {
                if (null != EventLog)
                    EventLog.WriteEntry(prefix + " : " +
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex), type, id, category);
            }
            catch (Exception ex2)
            { string x = ex2.Message; }
        }
        private void LogThis(EventLogEntryType type, string message,
            int id, short category)
        {
            try
            {
                if (null != EventLog)
                    EventLog.WriteEntry(message, type, id, category);
            }
            catch (Exception ex2)
            { string x = ex2.Message; }
        }

        #endregion
    }
}
