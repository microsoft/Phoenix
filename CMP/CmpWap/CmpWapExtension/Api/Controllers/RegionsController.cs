//*****************************************************************************
// File: RegionsController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class is used to provide Azure Region Specific information.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Diagnostics;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    /// <remarks>
    /// This class is used to provide Azure Region Specific information.
    /// </remarks>
    public class RegionsController : BaseApiController
    {
        public static EventLog EventLog;
        public static List<Models.AzureRegion> OsList = new List<Models.AzureRegion>();

        #region Endpoints

        //*********************************************************************
        ///
        /// <summary>
        ///     This GET method lists valid Azure regions for VM provisioning.

        /// </summary>
        /// <returns>list of Azure regions</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.AzureRegion> ListRegions()
        {
            try
            {
                lock (OsList)
                {
                    if (0 == OsList.Count)
                        FetchAzureRegionFromDb();
                }
                return OsList;
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
        /// This GET method lists valid Azure regions for VM provisioning based
        /// on WAP subscription Id.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns>List of Azure regions.</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.AzureRegion> ListRegions(string subscriptionId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subscriptionId))
                {
                    var ex = new ArgumentNullException(subscriptionId);
                    Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                    throw ex;
                }
                lock (OsList)
                {
                    FetchAzureRegionFromDb(subscriptionId);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw;
            }
            return OsList;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  This GET method lists valid Azure regions for VM provisioning based
        ///  on a Plan ID.
        ///  </summary>
        ///  <param name="planId"></param>
        /// <param name="isPlan"></param>
        /// <returns>List of Azure regions.</returns>
        ///  
        //*********************************************************************
        [HttpGet]
        public List<Models.AzureRegion> ListRegions(string planId, bool isPlan)
        {
            if (string.IsNullOrWhiteSpace(planId))
            {
                var ex = new ArgumentNullException(planId);
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw ex;
            }
            return GetRegionsAvailableToPlan(planId);
        }

        #endregion

        #region Helper methods

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetchs Azure region specific information from the
        ///     WAP DB.
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchAzureRegionFromDb()
        {
            var cwdb = new CmpWapDb();
            var list = cwdb.FetchAzureRegionList(onlyActiveOnes: true);
            OsList.Clear();
            OsList.AddRange(list);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetchs Azure region specific information from the
        ///     WAP DB.
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchAzureRegionFromDb(string subscriptionId)
        {
            var cwdb = new CmpWapDb();
            OsList.Clear();
            List<AzureRegion> list;

            if (!string.IsNullOrWhiteSpace(subscriptionId))
            {
                var repository = cwdb as ICmpWapDbTenantRepository;
                list = repository.FetchAzureRegionList(subscriptionId);
            }
            else
            {
                list = cwdb.FetchAzureRegionList(onlyActiveOnes: true);
            }
            OsList.AddRange(list);


        }


        private List<Models.AzureRegion> GetRegionsAvailableToPlan(string planId)
        {
            var db = new CmpWapDb();
            var result = db.FetchAzureRegionListByPlan(planId);
            return result;
        }

        #endregion

        
    }
}
