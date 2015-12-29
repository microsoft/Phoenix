//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.Samples.DataContracts;

namespace Microsoft.WindowsAzurePack.Samples
{
    /// <summary>
    /// Admin client library
    /// </summary>
    public interface IAdminManagementClient
    {
        #region Resource Provider

        /// <summary>
        /// Create resource provider
        /// </summary>
        /// <param name="resourceProvider">resource provider</param>
        Task<ResourceProvider> CreateResourceProviderAsync(ResourceProvider resourceProvider);

        /// <summary>
        /// Verifies the resource provider.
        /// </summary>
        /// <param name="resourceProvider">The resource provider.</param>
        /// <param name="verificationTests">The verification tests.</param>
        Task<ResourceProviderVerificationResult> VerifyResourceProviderAsync(ResourceProvider resourceProvider, ResourceProviderVerificationTestList verificationTests);

        /// <summary>
        /// Update resource provider
        /// </summary>
        /// <param name="name">resource provider name</param>
        /// <param name="instanceId">resource provider instance id</param>
        /// <param name="resourceProvider">resource provider</param>
        Task<ResourceProvider> UpdateResourceProviderAsync(string name, string instanceId, ResourceProvider resourceProvider);

        /// <summary>
        /// Get resource provider
        /// </summary>
        /// <param name="name">resource provider name</param>
        /// <param name="instanceId">resource provider instance id</param>
        Task<ResourceProvider> GetResourceProviderAsync(string name, string instanceId);

        /// <summary>
        /// List resource providers
        /// </summary>
        /// <param name="includeSystemResourceProviders">if set to <c>true</c> [include system resource providers].</param>
        Task<ResourceProviderList> ListResourceProvidersAsync(bool includeSystemResourceProviders = false);

        /// <summary>
        /// Delete resource provider
        /// </summary>
        /// <param name="name">resource provider name</param>
        /// <param name="instanceId">resource provider instance id</param>
        Task DeleteResourceProviderAsync(string name, string instanceId);

        #endregion

        #region Subscription

        /// <summary>
        /// Provision subscription
        /// </summary>
        /// <param name="provisioningInfo">provisioning info argument</param>
        Task<Subscription> ProvisionSubscriptionAsync(AzureProvisioningInfo provisioningInfo);

        /// <summary>
        /// Enable subscription
        /// </summary>
        /// <param name="subscriptionId">subscription id argument</param>
        Task<Subscription> EnableSubscriptionAsync(string subscriptionId);

        /// <summary>
        /// Disable subscription
        /// </summary>
        /// <param name="subscriptionId">subscription id argument</param>
        Task<Subscription> DisableSubscriptionAsync(string subscriptionId);

        /// <summary>
        /// Updates the subscription co-administrators .
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="coAdminNames">The co-admin names.</param>
        Task<Subscription> UpdateSubscriptionCoAdminsAsync(string subscriptionId, string[] coAdminNames);

        /// <summary>
        /// Migrates the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="targetPlanId">The target plan id.</param>
        Task<Subscription> MigrateSubscriptionAsync(string subscriptionId, string targetPlanId);

        /// <summary>
        /// Get subscription
        /// </summary>
        /// <param name="subscriptionId">subscription id</param>
        Task<Subscription> GetSubscriptionAsync(string subscriptionId);

        /// <summary>
        /// List all subscriptions matching the given query.  Filter only matches against user or offer name.
        /// </summary>
        Task<QueryResult<Subscription>> ListSubscriptionsAsync(Query query);

        /// <summary>
        /// Lists the plan subscriptions.
        /// </summary>
        /// <param name="planId">The plan id.</param>
        /// <param name="query">The query.</param>
        Task<QueryResult<Subscription>> ListPlanSubscriptionsAsync(string planId, Query query);

        /// <summary>
        /// Lists the add-on subscriptions.
        /// </summary>
        /// <param name="addOnId">The add-on id.</param>
        /// <param name="query">The query.</param>
        Task<QueryResult<Subscription>> ListAddOnSubscriptionsAsync(string addOnId, Query query);

        /// <summary>
        /// Lists the user subscriptions.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="query">The query.</param>
        Task<QueryResult<Subscription>> ListUserSubscriptionsAsync(string userId, Query query);

        /// <summary>
        /// Delete subscription
        /// </summary>
        /// <param name="subscriptionId">subscription id</param>
        /// <param name="forceDelete">The flag to indicate whether to force deletion of the subscription from the management service regardless of its status in the underlying resource providers.</param>
        Task<Subscription> DeprovisionSubscriptionAsync(string subscriptionId, bool forceDelete = false);

        /// <summary>
        /// Adds the add-on to subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="addOn">The add-on.</param>
        Task<PlanAddOnReference> SubscribeToAddOn(string subscriptionId, PlanAddOnReference addOn);

        /// <summary>
        /// Removes the add-on from subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="addOnId">The add-on id.</param>
        /// <param name="addOnInstanceId">The add-on instance id.</param>
        Task UnsubscribeFromAddOnAsync(string subscriptionId, string addOnId, string addOnInstanceId);

        /// <summary>
        /// Lists the subscription usage summaries.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        Task<UsageSummaryList> ListUsageSummariesAsync(string subscriptionId);

        /// <summary>
        /// Syncs the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        Task<Subscription> SyncSubscriptionAsync(string subscriptionId);
        #endregion

        #region Plan

        /// <summary>
        /// Create plan
        /// </summary>
        /// <param name="plan">plan to create</param>
        Task<Plan> CreatePlanAsync(Plan plan);

        /// <summary>
        /// Update plan
        /// </summary>
        /// <param name="id">plan id</param>
        /// <param name="plan">plan to update</param>
        Task<Plan> UpdatePlanAsync(string id, Plan plan);

        /// <summary>
        /// Syncs the plan.
        /// </summary>
        /// <param name="id">The id.</param>
        Task<Plan> SyncPlanAsync(string id);

        /// <summary>
        /// Get plan
        /// </summary>
        /// <param name="id">plan id</param>
        Task<Plan> GetPlanAsync(string id);

        /// <summary>
        /// List all plans
        /// </summary>
        Task<PlanList> ListPlansAsync();

        /// <summary>
        /// Delete plan
        /// </summary>
        /// <param name="id">plan id</param>
        Task DeletePlanAsync(string id);

        /// <summary>
        /// Get plan metrics
        /// </summary>
        /// <param name="planId">plan id</param>
        /// <param name="startTime">start time</param>
        /// <param name="endTime">end time</param>
        Task<ResourceMetricSets> GetPlanMetricsAsync(string planId, DateTime? startTime, DateTime? endTime);

        /// <summary>
        /// Updates the plan quota
        /// </summary>
        /// <param name="id">The plan id</param>
        /// <param name="quota">The quota to update</param>
        Task<Plan> UpdatePlanQuotaAsync(string id, PlanQuotaUpdate quota);

        /// <summary>
        /// Adds the service to plan
        /// </summary>
        /// <param name="planId">The plan id</param>
        /// <param name="service">The service</param>
        Task<ResourceProviderReference> AddServiceToPlanAsync(string planId, ResourceProviderReference service);

        /// <summary>
        /// Removes the service from plan
        /// </summary>
        /// <param name="planId">The plan id</param>
        /// <param name="serviceId">The service id</param>
        Task RemoveServiceFromPlanAsync(string planId, string serviceId);

        /// <summary>
        /// Adds the add-on to plan.
        /// </summary>
        /// <param name="planId">The plan id.</param>
        /// <param name="addOn">The add-on to add.</param>
        Task<PlanAddOnReference> LinkAddOnToPlanAsync(string planId, PlanAddOnReference addOn);

        /// <summary>
        /// Removes the add-on from plan.
        /// </summary>
        /// <param name="planId">The plan id.</param>
        /// <param name="addOnId">The add-on id.</param>
        Task UnlinkAddOnFromPlanAsync(string planId, string addOnId);

        /// <summary>
        /// Updates the state of the plan add-on.
        /// </summary>
        /// <param name="planId">The plan id.</param>
        /// <param name="addOnId">The add-on id.</param>
        /// <param name="newState">The new state.</param>
        Task<PlanAddOnReference> UpdatePlanAddOnStateAsync(string planId, string addOnId, PlanState newState);

        #endregion

        #region Add-on

        /// <summary>
        /// Create add-on
        /// </summary>
        /// <param name="addOn">add-on to create</param>
        Task<PlanAddOn> CreateAddOnAsync(PlanAddOn addOn);

        /// <summary>
        /// Update add-on
        /// </summary>
        /// <param name="id">add-on id</param>
        /// <param name="addOn">add-on to update</param>
        Task<PlanAddOn> UpdateAddOnAsync(string id, PlanAddOn addOn);

        /// <summary>
        /// Syncs the add-on.
        /// </summary>
        /// <param name="id">The id.</param>
        Task<PlanAddOn> SyncAddOnAsync(string id);

        /// <summary>
        /// Get add-on
        /// </summary>
        /// <param name="id">Add-on id</param>
        Task<PlanAddOn> GetAddOnAsync(string id);

        /// <summary>
        /// List all add-ons
        /// </summary>
        Task<PlanAddOnList> ListAddOnsAsync();

        /// <summary>
        /// Gets the add-on metrics.
        /// </summary>
        /// <param name="addOnId">The add-on id.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        Task<ResourceMetricSets> GetAddOnMetricsAsync(string addOnId, DateTime? startTime, DateTime? endTime);

        /// <summary>
        /// Delete add-on
        /// </summary>
        /// <param name="id">add-on id</param>
        Task DeleteAddOnAsync(string id);

        /// <summary>
        /// Updates the add-on quota
        /// </summary>
        /// <param name="id">The add-on id</param>
        /// <param name="quota">The quota to update</param>
        Task<PlanAddOn> UpdateAddOnQuotaAsync(string id, PlanQuotaUpdate quota);

        /// <summary>
        /// Adds the service to add-on
        /// </summary>
        /// <param name="addOnId">The add-on id</param>
        /// <param name="service">The service to add</param>
        Task<ResourceProviderReference> AddServiceToAddOnAsync(string addOnId, ResourceProviderReference service);

        /// <summary>
        /// Removes the service from add-on
        /// </summary>
        /// <param name="addOnId">The add-on id</param>
        /// <param name="serviceId">The service id to remove</param>
        Task RemoveServiceFromAddOnAsync(string addOnId, string serviceId);

        #endregion

        #region User

        /// <summary>
        /// Lists the users.
        /// </summary>
        /// <param name="query">The query.</param>
        Task<QueryResult<User>> ListUsersAsync(Query query);

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="name">The user name.</param>
        Task<User> GetUserAsync(string name);

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        Task<User> CreateUserAsync(User user);

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        Task<User> UpdateUserAsync(User user);

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="name">The user name.</param>
        /// <param name="deleteSubscriptions">The flag to indicate whether to also delete the user subscriptions and all existing resources</param>
        /// <param name="forceDelete">The flag to indicate whether to force deletion of the user and all its subscriptions but not the underlying resources</param>
        Task DeleteUserAsync(string name, bool deleteSubscriptions, bool forceDelete);

        /// <summary>
        /// Invalidates the user token.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="tokenExpiryTime">The token expiry time.</param>
        Task InvalidateUserTokenAsync(string userId, DateTime tokenExpiryTime);

        #endregion

        #region Settings
        
        /// <summary>
        /// Calls the setting method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="settingStorePartitions">The settingStorePartitions.</param>
        /// <param name="settingCollection">collection of settings.</param>
        Task<SettingCollection> CallSettingMethodAsync(SettingMethods method, SettingStorePartitions settingStorePartitions, SettingCollection settingCollection);

        /// <summary>
        /// Calls the setting method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="settingStorePartitions">The settingStorePartitions.</param>
        /// <param name="setting">the setting.</param>
        Task<SettingCollection> CallSettingMethodAsync(SettingMethods method, SettingStorePartitions settingStorePartitions, Setting setting);
        
        #endregion
    }
}
