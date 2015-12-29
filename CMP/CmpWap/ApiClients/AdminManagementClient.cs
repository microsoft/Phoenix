//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.Samples.DataContracts;

namespace Microsoft.WindowsAzurePack.Samples
{
    /// <summary>
    /// Admin client library
    /// </summary>
    public class AdminManagementClient : ManagementClientBase, IAdminManagementClient
    {
        private static class QueryStrings
        {
            public const string EnableFanout = "enableFanout";
            public const string FilterSource = "filterProperty";
            public const string PlanSubscriptionQueryFormat = "planId={0}&{1}";
            public const string AddOnSubscriptionQueryFormat = "addOnId={0}&{1}";
            public const string UserSubscriptionQueryFormat = "userId={0}&{1}";
        }

        /// <summary>
        /// Initializes a new instance of the AdminManagementClient class.
        /// </summary>
        /// <param name="adminBaseEndpoint">Admin endpoint</param>
        /// <param name="token">Access token</param>
        public AdminManagementClient(Uri adminBaseEndpoint, string token)
            : base(adminBaseEndpoint, ManagementClientMediaType.Xml, new TokenMessageProcessingHandler(token))
        {
        }

        /// <summary>
        /// Initializes a new instance of the AdminManagementClient class.
        /// </summary>
        /// <param name="adminBaseEndpoint">Admin endpoint</param>
        /// <param name="token">Access token</param>
        public AdminManagementClient(Uri adminBaseEndpoint, MessageProcessingHandler handler)
            : base(adminBaseEndpoint, ManagementClientMediaType.Xml, handler)
        {
            
        }

        #region Resource Provider

        /// <summary>
        /// Create resource provider
        /// </summary>
        /// <param name="resourceProvider">Resource provider</param>
        public Task<ResourceProvider> CreateResourceProviderAsync(ResourceProvider resourceProvider)
        {
            return this.SendAsync<ResourceProvider, ResourceProvider>(
                this.CreateRequestUri(RelativePaths.ResourceProviders), HttpMethod.Post, resourceProvider);
        }

        /// <summary>
        /// Verifies the resource provider.
        /// </summary>
        /// <param name="resourceProvider">The resource provider.</param>
        /// <param name="verificationTests">The verification tests.</param>
        public Task<ResourceProviderVerificationResult> VerifyResourceProviderAsync(
            ResourceProvider resourceProvider, 
            ResourceProviderVerificationTestList verificationTests)
        {
            var verificationInfo = new ResourceProviderVerification()
            {
                ResourceProvider = resourceProvider,
                Tests = verificationTests
            };

            return this.SendAsync<ResourceProviderVerification, ResourceProviderVerificationResult>(
                this.CreateRequestUri(RelativePaths.ResourceProviderVerification), HttpMethod.Post, verificationInfo);
        }

        /// <summary>
        /// Update resource provider
        /// </summary>
        /// <param name="name">resource provider name</param>
        /// <param name="instanceId">resource provider instance id</param>
        /// <param name="resourceProvider">resource provider</param>
        public Task<ResourceProvider> UpdateResourceProviderAsync(string name, string instanceId, ResourceProvider resourceProvider)
        {
            return this.SendAsync<ResourceProvider, ResourceProvider>(
                this.CreateRequestUri(
                    string.Format(CultureInfo.InvariantCulture, RelativePaths.ResourceProvider, name, instanceId)), 
                    HttpMethod.Put, 
                    resourceProvider);
        }

        /// <summary>
        /// Get resource provider
        /// </summary>
        /// <param name="name">resource provider name</param>
        /// <param name="instanceId">resource provider instance id</param>
        public Task<ResourceProvider> GetResourceProviderAsync(string name, string instanceId)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.ResourceProvider, name, instanceId));
            return this.GetAsync<ResourceProvider>(requestUri);
        }

        /// <summary>
        /// List resource providers
        /// </summary>
        /// <param name="includeSystemResourceProviders">if set to <c>true</c> [include system resource providers].</param>
        public Task<ResourceProviderList> ListResourceProvidersAsync(bool includeSystemResourceProviders = false)
        {
            return this.GetAsync<ResourceProviderList>(
                this.CreateRequestUri(
                    RelativePaths.ResourceProviders, 
                    new KeyValuePair<string, string>("includeSystemResourceProviders", includeSystemResourceProviders.ToString())));
        }

        /// <summary>
        /// Delete resource provider
        /// </summary>
        /// <param name="name">resource provider name</param>
        /// <param name="instanceId">resource provider instance id</param>
        public Task DeleteResourceProviderAsync(string name, string instanceId)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.ResourceProvider, name, instanceId));
            return this.SendAsync(requestUri, HttpMethod.Delete);
        }

        #endregion

        #region Subscription

        /// <summary>
        /// Provision subscription
        /// </summary>
        /// <param name="provisioningInfo">provisioning info argument</param>
        public Task<Subscription> ProvisionSubscriptionAsync(AzureProvisioningInfo provisioningInfo)
        {
            return this.SendAsync<AzureProvisioningInfo, Subscription>(this.CreateRequestUri(RelativePaths.Subscriptions), HttpMethod.Post, provisioningInfo);
        }

        /// <summary>
        /// Enable subscription
        /// </summary>
        /// <param name="subscriptionId">subscription id argument</param>
        public Task<Subscription> EnableSubscriptionAsync(string subscriptionId)
        {
            var provisioningInfo = new AzureProvisioningInfo()
            {
                Status = SubscriptionStatus.Active.ToString(),
                SubscriptionId = new Guid(subscriptionId)
            };

            return this.SendAsync<AzureProvisioningInfo, Subscription>(
                this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Subscription, subscriptionId)),
                new HttpMethod(Microsoft.WindowsAzurePack.Samples.DataContracts.Constants.HttpMethods.Patch),
                provisioningInfo);
        }

        /// <summary>
        /// Disable subscription
        /// </summary>
        /// <param name="subscriptionId">subscription id argument</param>
        public Task<Subscription> DisableSubscriptionAsync(string subscriptionId)
        {
            var provisioningInfo = new AzureProvisioningInfo()
            {
                Status = SubscriptionStatus.Suspended.ToString(),
                SubscriptionId = new Guid(subscriptionId)
            };

            return this.SendAsync<AzureProvisioningInfo, Subscription>(
                this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Subscription, subscriptionId)),
                new HttpMethod(Microsoft.WindowsAzurePack.Samples.DataContracts.Constants.HttpMethods.Patch),
                provisioningInfo);
        }

        /// <summary>
        /// Migrates the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="targetPlanId">The target plan id.</param>
        public Task<Subscription> MigrateSubscriptionAsync(string subscriptionId, string targetPlanId)
        {
            var provisioningInfo = new AzureProvisioningInfo()
            {
                SubscriptionId = new Guid(subscriptionId),
                PlanId = targetPlanId
            };

            return this.SendAsync<AzureProvisioningInfo, Subscription>(
                this.CreateRequestUri(
                    string.Format(CultureInfo.InvariantCulture, RelativePaths.Subscription, subscriptionId),
                    new KeyValuePair<string, string>("migrate", bool.TrueString)),
                new HttpMethod(Microsoft.WindowsAzurePack.Samples.DataContracts.Constants.HttpMethods.Patch),
                provisioningInfo);
        }

        /// <summary>
        /// Updates the subscription co-administrators.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="coAdminNames">The co-admin names.</param>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "coAdminNames is a valid name")]
        public Task<Subscription> UpdateSubscriptionCoAdminsAsync(string subscriptionId, string[] coAdminNames)
        {
            var provisioningInfo = new AzureProvisioningInfo()
            {
                SubscriptionId = new Guid(subscriptionId),
                CoAdminNames = new List<string>(coAdminNames)
            };

            return this.SendAsync<AzureProvisioningInfo, Subscription>(
                this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Subscription, subscriptionId)),
                new HttpMethod(Microsoft.WindowsAzurePack.Samples.DataContracts.Constants.HttpMethods.Patch),
                provisioningInfo);
        }

        /// <summary>
        /// Get subscription
        /// </summary>
        /// <param name="subscriptionId">subscription id</param>
        public Task<Subscription> GetSubscriptionAsync(string subscriptionId)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Subscription, subscriptionId));
            return this.GetAsync<Subscription>(requestUri);
        }

        /// <summary>
        /// List subscriptions matching the given query
        /// </summary>
        public Task<QueryResult<Subscription>> ListSubscriptionsAsync(Query query)
        {
            return this.GetAsync<QueryResult<Subscription>>(this.CreateRequestUri(RelativePaths.Subscriptions, query.GetQueryString()));
        }

        /// <summary>
        /// Lists the plan subscriptions.
        /// </summary>
        /// <param name="planId">The plan id.</param>
        /// <param name="query">The query.</param>
        public Task<QueryResult<Subscription>> ListPlanSubscriptionsAsync(string planId, Query query)
        {
            var queryString = string.Format(CultureInfo.InvariantCulture, QueryStrings.PlanSubscriptionQueryFormat, planId, query.GetQueryString());
            return this.GetAsync<QueryResult<Subscription>>(this.CreateRequestUri(RelativePaths.Subscriptions, queryString));
        }

        /// <summary>
        /// Lists the add on subscriptions.
        /// </summary>
        /// <param name="addOnId">The add on id.</param>
        /// <param name="query">The query.</param>
        public Task<QueryResult<Subscription>> ListAddOnSubscriptionsAsync(string addOnId, Query query)
        {
            var queryString = string.Format(CultureInfo.InvariantCulture, QueryStrings.AddOnSubscriptionQueryFormat, addOnId, query.GetQueryString());
            return this.GetAsync<QueryResult<Subscription>>(this.CreateRequestUri(RelativePaths.Subscriptions, queryString));
        }

        /// <summary>
        /// Lists the user subscriptions.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="query">The query.</param>
        public Task<QueryResult<Subscription>> ListUserSubscriptionsAsync(string userId, Query query)
        {
            var queryString = string.Format(CultureInfo.InvariantCulture, QueryStrings.UserSubscriptionQueryFormat, userId, query.GetQueryString());
            return this.GetAsync<QueryResult<Subscription>>(this.CreateRequestUri(RelativePaths.Subscriptions, queryString));
        }
        #endregion

        #region Plan

        /// <summary>
        /// Create plan
        /// </summary>
        /// <param name="plan">plan to create</param>
        public Task<Plan> CreatePlanAsync(Plan plan)
        {
            return this.SendAsync<Plan, Plan>(this.CreateRequestUri(RelativePaths.Plans), HttpMethod.Post, plan);
        }

        /// <summary>
        /// Get plan
        /// </summary>
        /// <param name="id">plan id</param>
        public Task<Plan> GetPlanAsync(string id)
        {
            return this.GetAsync<Plan>(this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Plan, id)));
        }

        /// <summary>
        /// Update plan
        /// </summary>
        /// <param name="id">plan id</param>
        /// <param name="plan">plan to update</param>
        public Task<Plan> UpdatePlanAsync(string id, Plan plan)
        {
            return this.SendAsync<Plan, Plan>(
                this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Plan, id)), 
                HttpMethod.Put, 
                plan);
        }

        /// <summary>
        /// Syncs the plan async.
        /// </summary>
        /// <param name="id">The id.</param>
        public Task<Plan> SyncPlanAsync(string id)
        {
            return this.SendAsync<PlanQuotaUpdate, Plan>(
                this.CreateRequestUri(
                    string.Format(CultureInfo.InvariantCulture, RelativePaths.PlanQuota, id), 
                    new KeyValuePair<string, string>("sync", bool.TrueString)),
                HttpMethod.Put,
                null);
        }

        /// <summary>
        /// Delete plan
        /// </summary>
        /// <param name="id">plan id</param>
        public Task DeletePlanAsync(string id)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Plan, id));
            return this.SendAsync(requestUri, HttpMethod.Delete);
        }

        /// <summary>
        /// Get paln metrics
        /// </summary>
        /// <param name="planId">plan id</param>
        /// <param name="startTime">start time in UTC (defaults to end time - 1 hour)</param>
        /// <param name="endTime">end time in UTC (defaults to current time)</param>
        public Task<ResourceMetricSets> GetPlanMetricsAsync(string planId, DateTime? startTime, DateTime? endTime)
        {
            DateTime validEndTime = endTime.HasValue ? endTime.Value : DateTime.UtcNow;
            DateTime validStartTime = startTime.HasValue ? startTime.Value : validEndTime.Subtract(TimeSpan.FromHours(1));

            return this.GetAsync<ResourceMetricSets>(this.CreateRequestUri(
                string.Format(CultureInfo.InvariantCulture, RelativePaths.PlanMetrics, planId),
                new KeyValuePair<string, string>("startTime", validStartTime.ToString("o")),
                new KeyValuePair<string, string>("endTime", validEndTime.ToString("o"))));
        }

        /// <summary>
        /// Updates the plan quota
        /// </summary>
        /// <param name="id">The plan id</param>
        /// <param name="quota">The quota to update</param>
        public Task<Plan> UpdatePlanQuotaAsync(string id, PlanQuotaUpdate quota)
        {
            return this.SendAsync<PlanQuotaUpdate, Plan>(
                this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.PlanQuota, id)), HttpMethod.Put, quota);
        }

        /// <summary>
        /// Adds the service to plan
        /// </summary>
        /// <param name="planId">The plan id</param>
        /// <param name="service">The service</param>
        public Task<ResourceProviderReference> AddServiceToPlanAsync(string planId, ResourceProviderReference service)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.PlanServices, planId));
            return this.SendAsync<ResourceProviderReference, ResourceProviderReference>(requestUri, HttpMethod.Post, service);
        }

        /// <summary>
        /// Removes the service from plan
        /// </summary>
        /// <param name="planId">The plan id</param>
        /// <param name="serviceId">The service id</param>
        public Task RemoveServiceFromPlanAsync(string planId, string serviceId)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.PlanService, planId, serviceId));
            return this.SendAsync(requestUri, HttpMethod.Delete);
        }

        /// <summary>
        /// Adds the add-on to plan.
        /// </summary>
        /// <param name="planId">The plan id.</param>
        /// <param name="addOn">The add on to add.</param>
        public Task<PlanAddOnReference> LinkAddOnToPlanAsync(string planId, PlanAddOnReference addOn)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.PlanAddOns, planId));
            return this.SendAsync<PlanAddOnReference, PlanAddOnReference>(requestUri, HttpMethod.Post, addOn);
        }

        /// <summary>
        /// Removes the add-on from plan.
        /// </summary>
        /// <param name="planId">The plan id.</param>
        /// <param name="addOnId">The add-on id.</param>
        public Task UnlinkAddOnFromPlanAsync(string planId, string addOnId)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.PlanAddOn, planId, addOnId));
            return this.SendAsync(requestUri, HttpMethod.Delete);
        }

        /// <summary>
        /// Updates the state of the plan add-on.
        /// </summary>
        /// <param name="planId">The plan id.</param>
        /// <param name="addOnId">The add-on id.</param>
        /// <param name="newState">The new state.</param>
        public Task<PlanAddOnReference> UpdatePlanAddOnStateAsync(string planId, string addOnId, PlanState newState)
        {
            var input = new PlanAddOnReference()
            {
                AddOnId = addOnId
            };

            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.PlanAddOn, planId, addOnId));

            return this.SendAsync<PlanAddOnReference, PlanAddOnReference>(this.CreateRequestUri(RelativePaths.PlanAddOns), HttpMethod.Put, input);
        }

        #endregion

        #region Add-on

        /// <summary>
        /// Create add-on
        /// </summary>
        /// <param name="addOn">add-on to create</param>
        public Task<PlanAddOn> CreateAddOnAsync(PlanAddOn addOn)
        {
            return this.SendAsync<PlanAddOn, PlanAddOn>(this.CreateRequestUri(RelativePaths.AddOns), HttpMethod.Post, addOn);
        }

        /// <summary>
        /// Get add-on
        /// </summary>
        /// <param name="id">Add-on id</param>
        public Task<PlanAddOn> GetAddOnAsync(string id)
        {
            return this.GetAsync<PlanAddOn>(this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.AddOn, id)));
        }

        /// <summary>
        /// Gets the add-on metrics async.
        /// </summary>
        /// <param name="addOnId">The add on id.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        public Task<ResourceMetricSets> GetAddOnMetricsAsync(string addOnId, DateTime? startTime, DateTime? endTime)
        {
            DateTime validEndTime = endTime.HasValue ? endTime.Value : DateTime.UtcNow;
            DateTime validStartTime = startTime.HasValue ? startTime.Value : validEndTime.Subtract(TimeSpan.FromHours(1));

            return this.GetAsync<ResourceMetricSets>(this.CreateRequestUri(
                string.Format(CultureInfo.InvariantCulture, RelativePaths.AddOnMetrics, addOnId),
                new KeyValuePair<string, string>("startTime", validStartTime.ToString("o")),
                new KeyValuePair<string, string>("endTime", validEndTime.ToString("o"))));
        }

        /// <summary>
        /// Update add-on
        /// </summary>
        /// <param name="id">add-on id</param>
        /// <param name="addOn">add-on to update</param>
        public Task<PlanAddOn> UpdateAddOnAsync(string id, PlanAddOn addOn)
        {
            return this.SendAsync<PlanAddOn, PlanAddOn>(
                this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.AddOn, id)), 
                HttpMethod.Put, 
                addOn);
        }

        /// <summary>
        /// Syncs the add on async.
        /// </summary>
        /// <param name="id">The id.</param>
        public Task<PlanAddOn> SyncAddOnAsync(string id)
        {
            return this.SendAsync<PlanQuotaUpdate, PlanAddOn>(
                this.CreateRequestUri(
                    string.Format(CultureInfo.InvariantCulture, RelativePaths.AddOnQuota, id), 
                    new KeyValuePair<string, string>("sync", "true")),
                HttpMethod.Put,
                null);
        }

        /// <summary>
        /// Delete add-on
        /// </summary>
        /// <param name="id">add-on id</param>
        public Task DeleteAddOnAsync(string id)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.AddOn, id));
            return this.SendAsync(requestUri, HttpMethod.Delete);
        }

        /// <summary>
        /// Updates the add-on quota
        /// </summary>
        /// <param name="id">The add-on id</param>
        /// <param name="quota">The quota to update</param>
        public Task<PlanAddOn> UpdateAddOnQuotaAsync(string id, PlanQuotaUpdate quota)
        {
            return this.SendAsync<PlanQuotaUpdate, PlanAddOn>(
                this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.AddOnQuota, id)), 
                HttpMethod.Put, 
                quota);
        }

        /// <summary>
        /// Adds the service to add-on
        /// </summary>
        /// <param name="addOnId">The add-on id</param>
        /// <param name="service">The service to add</param>
        public Task<ResourceProviderReference> AddServiceToAddOnAsync(string addOnId, ResourceProviderReference service)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.AddOnServices, addOnId));
            return this.SendAsync<ResourceProviderReference, ResourceProviderReference>(requestUri, HttpMethod.Post, service);
        }

        /// <summary>
        /// Removes the service from add-on
        /// </summary>
        /// <param name="addOnId">The add-on id</param>
        /// <param name="serviceId">The service id to remove</param>
        public Task RemoveServiceFromAddOnAsync(string addOnId, string serviceId)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.AddOnService, addOnId, serviceId));
            return this.SendAsync(requestUri, HttpMethod.Delete);
        }

        #endregion

        #region User

        /// <summary>
        /// Lists the users.
        /// </summary>
        /// <param name="query">The query.</param>
        public Task<QueryResult<User>> ListUsersAsync(Query query)
        {
            return this.GetAsync<QueryResult<User>>(this.CreateRequestUri(RelativePaths.Users, query.GetQueryString()));
        }

        #endregion
    }
}