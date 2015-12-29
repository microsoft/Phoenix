//*****************************************************************************
// File: NetworkNICsController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class is used to provide network NIC related information for
//          provisioning.
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
    /// This class is used to provide netowrk NIC related information for
    /// provisioning.
    /// </remarks>
    public class NetworkNICsController : BaseApiController
    {

        public static List<Models.NetworkNIC> nicList = new List<Models.NetworkNIC>();
      
        //*********************************************************************
        ///
        /// <summary>
        /// This method fetches network NICs from WAP DB
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchNetworkNICFromDb()
        {
            var cwdb = new CmpWapDb();
            var list = cwdb.FetchNetworkNicInfoList();
            nicList.Clear();
            nicList.AddRange(list);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///  This GET method provides list of network NICs.
        /// </summary>
        /// <returns>list of network NICS</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.NetworkNIC> ListNetworkNICs()
        {
            try
            {
                lock (nicList)
                {
                    if (0 == nicList.Count)
                        FetchNetworkNICFromDb();
                }
                return nicList;
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
        ///     This GET method provides list of network NICs based on 
        ///     WAP subscription Id.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns>list of network NICs</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.NetworkNIC> ListNetworkNICs(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                var ex = new ArgumentNullException(subscriptionId);
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw ex;
            }
            return ListNetworkNICs();
        }

        
    }
}
