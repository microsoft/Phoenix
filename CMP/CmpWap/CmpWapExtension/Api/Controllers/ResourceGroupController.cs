//*****************************************************************************
// File: ResourceGroupController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class is used to provide Resource Group related information.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    /// <remarks>
    /// This class is used to provide Resource Group related information.
    /// </remarks>
    public class ResourceGroupController : BaseApiController
    {
        public static EventLog EventLog;

        //*********************************************************************
        ///
        /// <summary>
        /// This method fetches Resource Group information from WAP DB.
        /// </summary>
        /// 
        //*********************************************************************

        private IEnumerable<ResourceProviderAcctGroup> FetchResourceGroupFromDb()
        {
            var vmsRepo = new VMServiceRepository();
            return vmsRepo.FetchServiceProviderAccountList().Select(spa => new ResourceProviderAcctGroup()
            {
                AdDomainMap = null,
                Name = spa.ResourceGroup,
                CreatedBy = "CmpWapExtension",
                CreatedOn = DateTime.Now,
                DomainId = 1,
                EnvironmentType = null,
                EnvironmentTypeId = 1,
                IsActive = true,
                LastUpdatedBy = "CmpWapExtension",
                LastUpdatedOn = DateTime.Now,
                NetworkNIC = null,
                NetworkNICId = 0,
                ResourceProviderAcctGroupId = 1
            }).ToList();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// This method fetches Resource Group information from WAP DB.
        /// </summary>
        /// <param name="wapSubscriptionId"></param>
        /// 
        //*********************************************************************

        private IEnumerable<ResourceProviderAcctGroup> FetchResourceGroupFromDb(string wapSubscriptionId)
        {
            ICmpWapDb cwdb = new CmpWapDb();

            var planId = cwdb.GetPlanMappedToWapSubscription(wapSubscriptionId);
            var spas = cwdb.FetchServiceProviderAccountsAssociatedWithPlan(planId);
            return spas.Select(spa => new ResourceProviderAcctGroup()
            {
                AdDomainMap = null, 
                Name = spa.ResourceGroup, 
                CreatedBy = "CmpWapExtension", 
                CreatedOn = DateTime.Now, 
                DomainId = 1, 
                EnvironmentType = null, 
                EnvironmentTypeId = 1, 
                IsActive = true, 
                LastUpdatedBy = "CmpWapExtension", 
                LastUpdatedOn = DateTime.Now, 
                NetworkNIC = null, 
                NetworkNICId = 0, 
                ResourceProviderAcctGroupId = 1
            }).ToList();
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This GET method lists valid Resource Group.
        /// </summary>
        /// <returns>list of resource group</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<ResourceProviderAcctGroup> ListResourceGroups()
        {
            try
            {
                var result = FetchResourceGroupFromDb().ToList();
                return result;
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
        ///     This GET method lists valid Resource Group based on a WAP Subscription
        ///     Id.
        /// </summary>
        /// <param name="subscriptionId">A WAP subscription</param>
        /// <returns>List of </returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<ResourceProviderAcctGroup> ListResourceGroups(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                var ex = new ArgumentNullException(subscriptionId);
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw ex;
            }

            try
            {
                var result = FetchResourceGroupFromDb(subscriptionId).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw;
            }
        }
    }
}
