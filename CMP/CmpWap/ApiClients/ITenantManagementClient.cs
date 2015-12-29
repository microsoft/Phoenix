//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.Samples.DataContracts;

namespace Microsoft.WindowsAzurePack.Samples
{
    /// <summary>
    /// Tenant management client library
    /// </summary>
    public interface ITenantManagementClient
    {
        /// <summary>
        /// Provision subscription
        /// </summary>
        /// <param name="provisioningInfo">provisioning info argument</param>
        Task<Subscription> ProvisionSubscriptionAsync(AzureProvisioningInfo provisioningInfo);

        /// <summary>
        /// Lists all subscriptions that belongs to the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        Task<SubscriptionList> ListSubscriptionsAsync(string userId);

        /// <summary>
        /// Gets the specified subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="userId">The user id.</param>
        Task<Subscription> GetSubscriptionAsync(string subscriptionId, string userId);

        /// <summary>
        /// Updates the specified subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="friendlyName">The friendly name.</param>
        Task UpdateSubscriptionFriendlyNameAsync(string subscriptionId, string friendlyName);

        /// <summary>
        /// Updates the subscription co-administrators.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="coAdminNames">The co-admin names.</param>
        Task UpdateSubscriptionCoAdminsAsync(string subscriptionId, string[] coAdminNames);

        /// <summary>
        /// Syncs the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        Task<Subscription> SyncSubscriptionAsync(string subscriptionId);

        /// <summary>
        /// Lists the subscription usage summaries.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        Task<UsageSummaryList> ListUsageSummariesAsync(string subscriptionId);

        /// <summary>
        /// Delete subscription
        /// </summary>
        /// <param name="subscriptionId">subscription id</param>
        /// <param name="forceDelete">The flag to indicate whether to force deletion of the subscription from the management service regardless of its status in the underlying resource providers.</param>
        Task<Subscription> DeprovisionSubscriptionAsync(string subscriptionId, bool forceDelete = false);

        /// <summary>
        /// Gets the management certificate
        /// </summary>
        /// <param name="subscriptionId">Subscription id</param>
        /// <param name="id">Certificate thumbprint</param>
        /// <param name="userId">The user id</param>
        Task<SubscriptionCertificate> GetCertificateAsync(string subscriptionId, string id, string userId);

        /// <summary>
        /// Lists the management certificates for a give subscription
        /// </summary>
        /// <param name="subscriptionId">Subscription id</param>
        /// <param name="userId">The user id</param>
        Task<SubscriptionCertificateList> ListCertificatesAsync(string subscriptionId, string userId);

        /// <summary>
        /// Adds a new management certificate
        /// </summary>
        /// <param name="subscriptionId">Subscription id</param>
        /// <param name="certificate">Managmeent certificate</param>
        /// <param name="userId">The user id</param>
        Task AddCertificateAsync(string subscriptionId, SubscriptionCertificate certificate, string userId);

        /// <summary>
        /// Deletes an existing management certificate
        /// </summary>
        /// <param name="subscriptionId">Subscription id</param>
        /// <param name="id">Certificate thumbprint</param>
        /// <param name="userId">The user id</param>
        Task DeleteCertificateAsync(string subscriptionId, string id, string userId);

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
        /// Invalidates the user token async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="tokenExpiryTime">The token expiry time.</param>
        Task InvalidateUserTokenAsync(string userId, DateTime tokenExpiryTime);

        /// <summary>
        /// Calls the setting method async.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="settingStorePartitions">The settingStorePartitions.</param>
        /// <param name="settingCollection">collection of settings.</param>
        Task<SettingCollection> CallSettingMethodAsync(SettingMethods method, SettingStorePartitions settingStorePartitions, SettingCollection settingCollection);

        /// <summary>
        /// Calls the setting method async.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="settingStorePartitions">The settingStorePartitions.</param>
        /// <param name="setting">the setting.</param>
        Task<SettingCollection> CallSettingMethodAsync(SettingMethods method, SettingStorePartitions settingStorePartitions, Setting setting);

        /// <summary>
        /// Get plan
        /// </summary>
        /// <param name="id">plan id</param>
        /// <param name="includePrice">if set to <c>true</c> [include price].</param>
        /// <param name="region">The region.</param>
        /// <param name="username">The username.</param>
        Task<Plan> GetPlanAsync(string id, bool includePrice = false, string region = null, string username = null);

        /// <summary>
        /// List all public add-on
        /// </summary>
        Task<PlanList> ListPlansAsync();

        /// <summary>
        /// Get plan
        /// </summary>
        /// <param name="id">add-on id</param>
        /// <param name="includePrice">if set to <c>true</c> [include price].</param>
        /// <param name="region">The region.</param>
        /// <param name="username">The username.</param>
        /// <param name="subscriptionId">The subscription id.</param>
        Task<PlanAddOn> GetAddOnAsync(string id, bool includePrice = false, string region = null, string username = null, string subscriptionId = null);

        /// <summary>
        /// List all public add-ons
        /// </summary>
        Task<PlanAddOnList> ListAddOnsAsync();

        /// <summary>
        /// Adds the add-on to subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="addOn">The add on.</param>
        Task<PlanAddOnReference> SubscribeToAddOn(string subscriptionId, PlanAddOnReference addOn);

        /// <summary>
        /// Removes the add-on from subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="addOnId">The add-on id.</param>
        /// <param name="addOnInstanceId">The add on instance id.</param>
        Task UnsubscribeFromAddOnAsync(string subscriptionId, string addOnId, string addOnInstanceId);
    }
}
