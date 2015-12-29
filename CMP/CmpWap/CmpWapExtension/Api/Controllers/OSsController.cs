//*****************************************************************************
// File: OSsController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class is used to provide OS related information.
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
    /// This class is used to provide OS related information.
    /// </remarks>
    public class OSsController : BaseApiController
    {
        public static EventLog EventLog;
        public static List<Models.VmOs> osList = new List<Models.VmOs>();


        //*********************************************************************
        ///
        /// <summary>
        /// This method fetches OS information from WAP DB.
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchVmOsFromDb()
        {
            var cwdb = new CmpWapDb();
            var list = cwdb.FetchOsInfoList(onlyActiveOnes: true);
            osList.Clear();
            osList.AddRange(list);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// This method fetches OS information from WAP DB for subcription 
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchVmOsFromDb(string subscriptionId)
        {
            var cwdb = new CmpWapDb();
            if (!string.IsNullOrEmpty(subscriptionId))
            {
                var repository = cwdb as ICmpWapDbTenantRepository;
                var list = repository.FetchOsInfoList(subscriptionId);
                osList.Clear();
                osList.AddRange(list);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This GET method returns list of valid Operating System.
        /// </summary>
        /// <returns>list of OS</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.VmOs> ListOSs()
        {
            try
            {
                lock (osList)
                {
                    if (0 == osList.Count)
                        FetchVmOsFromDb();
                }
                return osList;
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
        /// This GET method returns list of valid Operating System based on
        /// subscription Id.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.VmOs> ListOSs(string subscriptionId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subscriptionId))
                {
                    var ex = new ArgumentNullException(subscriptionId);
                    Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                    throw ex;
                }

                lock (osList)
                {

                    FetchVmOsFromDb(subscriptionId);
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw;
            }
            return osList;
        }

       
    }
}
