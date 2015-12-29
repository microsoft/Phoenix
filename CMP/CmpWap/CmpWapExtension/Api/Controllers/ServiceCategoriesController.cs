//*****************************************************************************
// File: ServiceCategoriesController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class is used to provide service category related information
//          for VM provisioning.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Diagnostics;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    /// <remarks>
    /// This class is used to provide service category related information
    /// for VM provisioning.
    /// </remarks>
    public class ServiceCategoriesController : BaseApiController
    {
        public static EventLog EventLog;
        public static List<Models.ServiceCategory> ServiceCategoryList = new List<Models.ServiceCategory>();


        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches service category related information from
        ///     WAp DB.
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchServiceCategoriesFromDb()
        {
            var cwdb = new CmpWapDb();
            var list = cwdb.FetchServiceCategoryInfoList();
            ServiceCategoryList.Clear();
            ServiceCategoryList.AddRange(list);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This GET method lists Service Category information.
        /// </summary>
        /// <returns>list of service category</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.ServiceCategory> ListServiceCategories()
        {
            try
            {
                lock (ServiceCategoryList)
                {
                    if (0 == ServiceCategoryList.Count)
                        FetchServiceCategoriesFromDb();
                }
                return ServiceCategoryList;
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
        ///     This GET method lists Service Category information based on 
        ///     subscription Id.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns>list of service category</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.ServiceCategory> ListServiceCategories(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                var ex = new ArgumentNullException(subscriptionId);
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw ex;
            }
            return ListServiceCategories();
        }

       
    }
}
