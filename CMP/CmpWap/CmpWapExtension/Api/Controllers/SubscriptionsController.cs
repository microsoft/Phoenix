//*****************************************************************************
// File: AppsController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This method is used to provide subscription related information.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;


namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    /// <remarks>
    ///     This method is used to provide subscription related information.
    /// </remarks>
    public class SubscriptionsController : ApiController
    {
        /// <summary>
        ///     This GET method lists the Azure Subscriptions associated with WAP subscriptions.
        /// </summary>
        /// <returns>list of subscription</returns>
        [HttpPost]
        public List<Subscription> GetSubscriptionList([FromBody]string [] wapSubscriptions)
        {
            //Null parameter check
            if (wapSubscriptions == null)
                return Enumerable.Empty<Subscription>().ToList();

            var cwdb = new CmpWapDb();
            List<Subscription> resultList = new List<Subscription>();

            foreach (var wapSubscription in wapSubscriptions)
            {
                var planName = cwdb.GetPlanMappedToWapSubscription(wapSubscription);
                var spas = cwdb.FetchServiceProviderAccountsAssociatedWithPlan(planName);
                resultList.AddRange(spas.Select(spa => new Subscription
                {
                    WapSubscriptionId = wapSubscription, 
                    PlanNameId = planName, 
                    AzureSubscriptionId = spa.AccountID
                }));
            }
            return resultList;
        }
    }
}
