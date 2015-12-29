//*****************************************************************************
// File: IISRoleServicesController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class is used for listing IIS Role Service associated with a VM.
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
    /// This class is used for listing IIS Role Service associated with a VM.
    /// </remarks>
    public class IISRoleServicesController : BaseApiController
    {

        public static List<Models.IISRoleService> appList = new List<Models.IISRoleService>();


        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches IIS RoleService from the WAP DB.
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchIISRoleServiceFromDb()
        {
            var cwdb = new CmpWapDb();
            var list = cwdb.FetchIisRoleServiceInfoList();
            appList.Clear();
            appList.AddRange(list);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method list all the IIS roles applicable for provisioning.
        /// </summary>
        /// <returns> list of IIS roles</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.IISRoleService> ListIISRoles()
        {
            try
            {
                lock (appList)
                {
                    if (0 == appList.Count)
                        FetchIISRoleServiceFromDb();
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
        ///     This method list all the IIS roles applicable for provisioning
        ///     based on WAP subscription Id.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns>list of IIS roles</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.IISRoleService> ListIISRoles(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                var ex = new ArgumentNullException(subscriptionId);
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw ex;
            }
            return ListIISRoles();
        }

    }
}
