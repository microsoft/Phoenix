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
    /// Client management client library
    /// </summary>
    public class TenantManagementClient : ManagementClientBase, ITenantManagementClient
    {
        /// <summary>
        /// Initializes a new instance of the ManagementClient class.
        /// </summary>
        /// <param name="baseEndpoint">API endpoint</param>
        /// <param name="token">The token.</param>
        public TenantManagementClient(Uri baseEndpoint, string token)
            : base(baseEndpoint, ManagementClientMediaType.Xml, new TokenMessageProcessingHandler(token))
        {
        }

        #region Plans

        /// <summary>
        /// Get plan
        /// </summary>
        /// <param name="id">plan id</param>
        /// <param name="includePrice">if set to <c>true</c> [include price].</param>
        /// <param name="region">The region.</param>
        /// <param name="username">The username.</param>
        /// <returns> Async task. </returns>
        public Task<Plan> GetPlanAsync(string id, bool includePrice = false, string region = null, string username = null)
        {
            if (includePrice)
            {
                if (null != username)
                {
                    return this.GetAsync<Plan>(this.CreateRequestUri(
                        string.Format(CultureInfo.InvariantCulture, RelativePaths.Plan, id),
                        new KeyValuePair<string, string>("includePrice", includePrice.ToString()),
                        new KeyValuePair<string, string>("region", region),
                        new KeyValuePair<string, string>("username", username)));
                }
                else
                {
                    return this.GetAsync<Plan>(this.CreateRequestUri(
                       string.Format(CultureInfo.InvariantCulture, RelativePaths.Plan, id),
                       new KeyValuePair<string, string>("includePrice", includePrice.ToString()),
                       new KeyValuePair<string, string>("region", region)));
                }
            }
            else
            {
                return this.GetAsync<Plan>(this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Plan, id)));
            }
        }

        #endregion

        #region Addons

        /// <summary>
        /// Get add-on
        /// </summary>
        /// <param name="id">Add-on id</param>
        /// <param name="includePrice">if set to <c>true</c> [include price].</param>
        /// <param name="region">The region.</param>
        /// <param name="username">The username.</param>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <returns> Async task. </returns>
        public Task<PlanAddOn> GetAddOnAsync(string id, bool includePrice = false, string region = null, string username = null, string subscriptionId = null)
        {
            if (includePrice)
            {
                if (null != username && null != subscriptionId)
                {
                    return this.GetAsync<PlanAddOn>(this.CreateRequestUri(
                       string.Format(CultureInfo.InvariantCulture, RelativePaths.AddOn, id),
                       new KeyValuePair<string, string>("includePrice", includePrice.ToString()),
                       new KeyValuePair<string, string>("region", region),
                       new KeyValuePair<string, string>("username", username),
                       new KeyValuePair<string, string>("subscriptionId", subscriptionId)));
                }
                else if (null != username)
                {
                    return this.GetAsync<PlanAddOn>(this.CreateRequestUri(
                       string.Format(CultureInfo.InvariantCulture, RelativePaths.AddOn, id),
                       new KeyValuePair<string, string>("includePrice", includePrice.ToString()),
                       new KeyValuePair<string, string>("region", region),
                       new KeyValuePair<string, string>("username", username)));
                }
                else if (null != subscriptionId)
                {
                    return this.GetAsync<PlanAddOn>(this.CreateRequestUri(
                       string.Format(CultureInfo.InvariantCulture, RelativePaths.AddOn, id),
                       new KeyValuePair<string, string>("includePrice", includePrice.ToString()),
                       new KeyValuePair<string, string>("region", region),
                       new KeyValuePair<string, string>("subscriptionId", subscriptionId)));
                }
                else
                {
                    return this.GetAsync<PlanAddOn>(this.CreateRequestUri(
                    string.Format(CultureInfo.InvariantCulture, RelativePaths.AddOn, id),
                    new KeyValuePair<string, string>("includePrice", includePrice.ToString()),
                    new KeyValuePair<string, string>("region", region)));
                }
            }
            else
            {
                return this.GetAsync<PlanAddOn>(this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.AddOn, id)));
            }
        }

        #endregion

        #region Subscriptions

        /// <summary>
        /// Provision subscription
        /// </summary>
        /// <param name="provisioningInfo">provisioning info argument</param>
        /// <returns>async task</returns>
        public Task<Subscription> ProvisionSubscriptionAsync(AzureProvisioningInfo provisioningInfo)
        {
            return this.SendAsync<AzureProvisioningInfo, Subscription>(this.CreateRequestUri(RelativePaths.Subscriptions), HttpMethod.Post, provisioningInfo);
        }

        /// <summary>
        /// Lists all subscriptions that belongs to the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>Async task contiaining the resulting subscription list.</returns>
        public Task<SubscriptionList> ListSubscriptionsAsync(string userId)
        {
            return this.GetAsync<SubscriptionList>(this.CreateRequestUri(RelativePaths.Subscriptions), userId);
        }

        /// <summary>
        /// Gets the specified subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>Async task contiaining the resulting subscription.</returns>
        public Task<Subscription> GetSubscriptionAsync(string subscriptionId, string userId)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Subscription, subscriptionId));
            return this.GetAsync<Subscription>(requestUri, userId);
        }

        /// <summary>
        /// Updates the specified subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="friendlyName">The friendly name.</param>
        /// <returns>Async task.</returns>
        public Task UpdateSubscriptionFriendlyNameAsync(string subscriptionId, string friendlyName)
        {
            var subscription = new Subscription
            {
                SubscriptionID = subscriptionId,
                SubscriptionName = friendlyName,
                CoAdminNames = null // null means won't change for PATCH
            };

            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Subscription, subscriptionId));
            return this.SendAsync(requestUri, new HttpMethod(Constants.HttpMethods.Patch), subscription);
        }

        /// <summary>
        /// Updates the subscription co admins async.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="coAdminNames">The co admin names.</param>
        /// <returns>
        /// Async task.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "coAdminNames is a valid name")]
        public Task UpdateSubscriptionCoAdminsAsync(string subscriptionId, string[] coAdminNames)
        {
            var subscription = new Subscription
            {
                CoAdminNames = new List<string>(coAdminNames)
            };

            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Subscription, subscriptionId));
            return this.SendAsync(requestUri, new HttpMethod(Constants.HttpMethods.Patch), subscription);
        }
        #endregion

        #region Certificates

        /// <summary>
        /// Gets the management certificate
        /// </summary>
        /// <param name="subscriptionId">Subscription id</param>
        /// <param name="id">Certificate thumbprint</param>
        /// <param name="userId">The user id</param>
        /// <returns>Task of subscription certificate</returns>
        public Task<SubscriptionCertificate> GetCertificateAsync(string subscriptionId, string id, string userId)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Certificate, subscriptionId, id));
            return this.GetAsync<SubscriptionCertificate>(requestUri, userId);
        }

        /// <summary>
        /// Lists the management certificates for a give subscription
        /// </summary>
        /// <param name="subscriptionId">Subscription id</param>
        /// <param name="userId">The user id</param>
        /// <returns>Task of subscription certificate list</returns>
        public Task<SubscriptionCertificateList> ListCertificatesAsync(string subscriptionId, string userId)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Certificates, subscriptionId));
            return this.GetAsync<SubscriptionCertificateList>(requestUri, userId);
        }

        /// <summary>
        /// Adds a new management certificate
        /// </summary>
        /// <param name="subscriptionId">Subscription id</param>
        /// <param name="certificate">Managmeent certificate</param>
        /// <param name="userId">The user id</param>
        /// <returns>Async task</returns>
        public Task AddCertificateAsync(string subscriptionId, SubscriptionCertificate certificate, string userId)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Certificates, subscriptionId));
            return this.SendAsync(requestUri, HttpMethod.Post, certificate, userId);
        }

        /// <summary>
        /// Deletes an existing management certificate
        /// </summary>
        /// <param name="subscriptionId">Subscription id</param>
        /// <param name="id">Certificate thumbprint</param>
        /// <param name="userId">The user id</param>
        /// <returns>Async task</returns>
        public Task DeleteCertificateAsync(string subscriptionId, string id, string userId)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Certificate, subscriptionId, id));
            return this.SendAsync(requestUri, HttpMethod.Delete, userId);
        }

        #endregion
    }
}
