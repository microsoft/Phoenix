//*****************************************************************************
// File: EnvironmentTypesController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class lists different environment types for new
//          virtual machine provisioning.
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
    ///     This class lists different environment types for new
    //      virtual machine provisioning.
    /// </remarks>
    public class EnvironmentTypesController : BaseApiController
    {
       
        public static List<Models.EnvironmentType> roleList = new List<Models.EnvironmentType>();


        //*********************************************************************
        ///
        /// <summary>
        ///     This methid fetches environment types from WAP DB
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchEnvironmentTypeFromDb()
        {
            var cwdb = new CmpWapDb();
            var list = cwdb.FetchEnvironmentTypeInfoList();
            roleList.Clear();
            roleList.AddRange(list);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method lists environment types present in the WAP DB.
        /// </summary>
        /// <returns> list of Environment types</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.EnvironmentType> ListEnvironmentTypes()
        {
            try
            {
                lock (roleList)
                {
                    if (0 == roleList.Count)
                        FetchEnvironmentTypeFromDb();
                }
                return roleList;
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
        ///     This method lists environment types present in the WAP DB based
        ///     on subscription.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.EnvironmentType> ListEnvironmentTypes(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                var ex = new ArgumentNullException(subscriptionId);
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw ex;
            }
            return ListEnvironmentTypes();
        }

    }
}
