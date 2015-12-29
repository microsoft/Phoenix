//*****************************************************************************
// File: DisksController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Get a list of detached disks in a given VM's Subscription.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    public class DisksController : ApiController
    {

        /// <summary>
        ///     Get a list of detached disks in a given VM's Subscription.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="vmId">Id associated with a VM in CMP DB</param>
        /// <returns>Disk Object</returns>

        [HttpGet]
        [EnableQuery]
        public IQueryable<DataVirtualHardDisk> GetDetachedDisks(string subscriptionId, int vmId)
        {
            try
            {
                var cwdb = new CmpWapDb();
                var foundVmDepRequest = cwdb.FetchVmDepRequest(vmId);

                var cmpi = new VMServiceRepository(_eventLog);
                return cmpi.GetDetachedDisks(foundVmDepRequest.CmpRequestID).Select(d => new DataVirtualHardDisk
                {
                    DiskName = d.DiskName,
                }).AsQueryable();
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.DisksController.GetDetachedDisks()", 100, 1);
                throw;
            }
        }

        #region --- Utilities -------------------------------------------------

        static EventLog _eventLog = null;

        /// <summary></summary>
        public static EventLog eventLog
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
                    catch (Exception)
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
                if (null != eventLog)
                    eventLog.WriteEntry(prefix + " : " +
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
                if (null != eventLog)
                    eventLog.WriteEntry(message, type, id, category);
            }
            catch (Exception ex2)
            { string x = ex2.Message; }
        }

        #endregion
    }
}
