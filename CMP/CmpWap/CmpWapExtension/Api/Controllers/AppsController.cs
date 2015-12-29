//*****************************************************************************
// File: AppsController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Get a list of applications associated with the Virtual Machine 
//          to be built.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{

    /// <remarks>
    ///     This class lists all the applications associated with the VM to be built.
    /// </remarks>
    public class AppsController : BaseApiController
    {
       
        public static List<Models.Application> appList = new List<Models.Application>();


        //*********************************************************************
        ///
        /// <summary>
        ///     Fetch list of applications from WAP DB
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchAppListFromDb()
        {
            var cwdb = new CmpWapDb();
            var list = cwdb.FetchAppList();
            appList.Clear();
            appList.AddRange(list);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     GET method to retrieve application list.
        /// </summary>
        /// <returns> list of applications</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.Application> ListApps()
        {
            try
            {
                lock (appList)
                {
                   FetchAppListFromDb();
                }
                return appList;
            }
            catch (Exception ex)
            {
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     GET method to retrieve application list based on WAP subscription Id
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns>list of applications</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.Application> ListApps(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                var ex = new ArgumentNullException(subscriptionId);
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw ex;
            }
            return ListApps();
        }

    }
}
