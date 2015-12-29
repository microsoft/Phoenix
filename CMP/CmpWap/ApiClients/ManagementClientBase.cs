//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzurePack.Samples.DataContracts;

namespace Microsoft.WindowsAzurePack.Samples
{
    /// <summary>
    /// Base class for management API clients.
    /// </summary>
    public abstract class ManagementClientBase : IDisposable
    {
        /// <summary>
        /// Relative paths class
        /// </summary>
        protected class RelativePaths
        {
            public const string ResourceProviders = "resourceproviders";
            public const string ResourceProviderVerification = "resourceproviderverification"; 
            public const string Subscriptions = "subscriptions";
            public const string Plans = "plans";
            public const string AddOns = "addons";
            public const string Users = "users";
            public const string Settings = "settings";
            public const string Certificates = Subscription + "/certificates";
                        
            public const string ResourceProvider = ResourceProviders + "/{0}/{1}";
            public const string Subscription = Subscriptions + "/{0}";
            public const string SubscriptionQuota = Subscriptions + "/{0}" + "/quota";
            public const string SubscriptionAddOns = Subscription + "/addons";
            public const string SubscriptionAddOn = SubscriptionAddOns + "/{1}/{2}";
            public const string SubscriptionServices = Subscription + "/services";
            public const string SubscriptionService = SubscriptionServices + "/{1}";
            public const string SubscriptionUsageSummaries = Subscription + "/usagesummaries";
            public const string Plan = Plans + "/{0}";
            public const string PlanQuota = Plan + "/quota";
            public const string PlanMetrics = Plan + "/metrics";
            public const string PlanServices = Plan + "/services";
            public const string PlanService = PlanServices + "/{1}";
            public const string PlanAddOns = Plan + "/addons";
            public const string PlanAddOn = PlanAddOns + "/{1}";
            public const string AddOn = AddOns + "/{0}";
            public const string AddOnQuota = AddOn + "/quota";
            public const string AddOnMetrics = AddOn + "/metrics";
            public const string AddOnServices = AddOn + "/services";
            public const string AddOnService = AddOnServices + "/{1}";
            public const string User = Users + "/{0}";
            public const string InvalidUserTokens = "/invalidusertokens";
            public const string Certificate = Certificates + "/{1}";
        }

        private const string UnknownErrorCode = "UnknownError";

        private readonly HttpClient httpClient;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementClientBase"/> class.
        /// </summary>
        /// <param name="baseEndpoint">The base endpoint.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="handler">Message processing handler</param>
        public ManagementClientBase(Uri baseEndpoint, ManagementClientMediaType mediaType, MessageProcessingHandler handler = null)
        {
            this.BaseAddress = baseEndpoint;
            this.MediaType = mediaType;
            this.httpClient = new HttpClient(handler);
            this.disposed = false;
        }

        /// <summary>
        /// Gets or sets the type of the media.
        /// </summary>
        /// <value>
        /// The type of the media.
        /// </value>
        protected ManagementClientMediaType MediaType { get; set; }

        /// <summary>
        /// Gets or sets the base address.
        /// </summary>
        /// <value>
        /// The base address.
        /// </value>
        protected Uri BaseAddress { get; set; }

        /// <summary>
        /// Creates the request URI.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="queryStringParameters">The query string parameters.</param>
        /// <returns>Request URI</returns>
        protected Uri CreateRequestUri(string relativePath, params KeyValuePair<string, string>[] queryStringParameters)
        {
            string queryString = string.Empty;

            if (queryStringParameters != null && queryStringParameters.Length > 0)
            {
                NameValueCollection queryStringProperties = HttpUtility.ParseQueryString(this.BaseAddress.Query);
                foreach (KeyValuePair<string, string> queryStringParameter in queryStringParameters)
                {
                    queryStringProperties[queryStringParameter.Key] = queryStringParameter.Value;
                }

                queryString = queryStringProperties.ToString();
            }

            return this.CreateRequestUri(relativePath, queryString);
        }

        /// <summary>
        /// Creates the request URI.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="queryString">The query string.</param>
        /// <returns>Request URI</returns>
        protected Uri CreateRequestUri(string relativePath, string queryString)
        {
            var endpoint = new Uri(this.BaseAddress, relativePath);
            var uriBuilder = new UriBuilder(endpoint) { Query = queryString };
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Sends a GET request.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="userId">The user id. Only required by the tenant API.</param>
        /// <returns>Response object.</returns>
        protected async Task<T> GetAsync<T>(Uri requestUri, string userId = null)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, requestUri);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                message.Headers.Add(Constants.Headers.PrincipalId, HttpUtility.UrlEncode(userId));
            }

            using (HttpResponseMessage response = await this.httpClient.SendAsync(message))
            {
                await this.ThrowIfResponseNotSuccessfulAsync(response);

                return await response.Content.ReadAsAsync<T>();
            }
        }

        /// <summary>
        /// Sends an http request.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="userId">The user id. Only required by the tenant API.</param>
        protected Task SendAsync(Uri requestUri, HttpMethod httpMethod, string userId = null)
        {
            return this.SendAsync<object>(requestUri, httpMethod, userId);
        }

        /// <summary>
        /// Sends an http request.
        /// </summary>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="userId">The user id.</param>
        protected Task<TOutput> SendAsync<TOutput>(Uri requestUri, HttpMethod httpMethod, string userId = null)
        {
            var message = new HttpRequestMessage(httpMethod, requestUri);
            return this.SendAsync<TOutput>(requestUri, httpMethod, message, true, userId);
        }

        /// <summary>
        /// Sends an http request.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="body">The body.</param>
        /// <param name="userId">The user id. Only required by the tenant API.</param>
        protected Task SendAsync<TInput>(Uri requestUri, HttpMethod httpMethod, TInput body, string userId = null)
        {
            return this.SendAsync<TInput, object>(requestUri, httpMethod, body, userId);
        }

        /// <summary>
        /// Sends an http request.
        /// </summary>
        /// <typeparam name="TInput">Input type.</typeparam>
        /// <typeparam name="TOutput">Output type.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="body">The body.</param>
        /// <param name="userId">The user id. Only required by the tenant API.</param>
        protected Task<TOutput> SendAsync<TInput, TOutput>(Uri requestUri, HttpMethod httpMethod, TInput body, string userId = null)
        {
            var message = new HttpRequestMessage(httpMethod, requestUri)
            {
                Content = new ObjectContent<TInput>(body, this.CreateMediaTypeFormatter())
            };

            return this.SendAsync<TOutput>(requestUri, httpMethod, message, true, userId);
        }

        private async Task<TOutput> SendAsync<TOutput>(Uri requestUri, HttpMethod httpMethod, HttpRequestMessage message, bool hasResult, string userId = null)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                message.Headers.Add(Constants.Headers.PrincipalId, userId);
            }

            using (HttpResponseMessage response = await this.httpClient.SendAsync(message))
            {
                await this.ThrowIfResponseNotSuccessfulAsync(response);

                if (!hasResult)
                {
                    return default(TOutput);
                }

                return await response.Content.ReadAsAsync<TOutput>();
            }
        }

        private async Task ThrowIfResponseNotSuccessfulAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                ManagementServiceError managementServiceError = null;

                try
                {
                    managementServiceError = await response.Content.ReadAsAsync<ManagementServiceError>();
                }
                catch (InvalidOperationException)
                {
                    // ReadAsAsync will synchronously throw InvalidOperationException when there is no default formatter for the ContentType.
                    // We will treat these cases as an unknown error
                }

                string errorCode = UnknownErrorCode;
                string errorMessage = Resources.UnknownErrorMessage;
                List<ErrorDetail> errorDetails = null;

                if (managementServiceError != null)
                {
                    errorCode = managementServiceError.Code;
                    errorMessage = managementServiceError.Message;
                    errorDetails = managementServiceError.Details;
                }

                throw new ManagementClientException(response.StatusCode, errorCode, errorMessage, errorDetails);
            }
        }

        private MediaTypeFormatter CreateMediaTypeFormatter()
        {
            MediaTypeFormatter formatter;

            if (this.MediaType == ManagementClientMediaType.Json)
            {
                formatter = new JsonMediaTypeFormatter();
            }
            else
            {
                formatter = new XmlMediaTypeFormatter();
            }

            return formatter;
        }

        #region Settings

        /// <summary>
        /// Executes the setting method async.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="settingStorePartitions">The settingStorePartitions.</param>
        /// <param name="settingCollection">collection of settings.</param>
        /// <returns>Async task.</returns>
        public Task<SettingCollection> CallSettingMethodAsync(SettingMethods method, SettingStorePartitions settingStorePartitions, SettingCollection settingCollection)
        {
            var requestUri = this.CreateRequestUri(RelativePaths.Settings);
            var batch = new SettingBatch
            {
                SettingStorePartitions = settingStorePartitions,
                SettingCollection = settingCollection,
                Method = method.ToString()
            };
            return this.SendAsync<SettingBatch, SettingCollection>(requestUri, HttpMethod.Post, batch);
        }

        /// <summary>
        /// Executes the setting method async.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="settingStorePartitions">The settingStorePartitions.</param>
        /// <param name="setting">the setting.</param>
        /// <returns>Async task.</returns>
        public Task<SettingCollection> CallSettingMethodAsync(SettingMethods method, SettingStorePartitions settingStorePartitions, Setting setting)
        {
            return this.CallSettingMethodAsync(method, settingStorePartitions, new SettingCollection { setting });
        }

        #endregion

        #region Users

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="name">The user name.</param>
        /// <returns>Async Task.</returns>
        public Task<User> GetUserAsync(string name)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.User, name));
            return this.GetAsync<User>(requestUri);
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Async Task.</returns>
        public Task<User> CreateUserAsync(User user)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.Users));
            return this.SendAsync<User, User>(requestUri, HttpMethod.Post, user);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Async Task.</returns>
        public Task<User> UpdateUserAsync(User user)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.User, user.Name));
            return this.SendAsync<User, User>(requestUri, HttpMethod.Put, user);
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="name">The user name.</param>
        /// <param name="deleteSubscriptions">The flag to indicate whether to also delete the user subscriptions and all existing resources</param>
        /// <param name="forceDelete">The flag to indicate whether to force deletion of the user and all its subscriptions but not the underlying resources</param>
        /// <returns>Async Task.</returns>
        public Task DeleteUserAsync(string name, bool deleteSubscriptions, bool forceDelete)
        {
            Uri requestUri = this.CreateRequestUri(
                string.Format(CultureInfo.InvariantCulture, RelativePaths.User, name),
                new KeyValuePair<string, string>("deleteSubscriptions", deleteSubscriptions.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>("force", forceDelete.ToString(CultureInfo.InvariantCulture)));
            return this.SendAsync(requestUri, HttpMethod.Delete);
        }

        #endregion

        #region Plans

        /// <summary>
        /// List all plans
        /// </summary>
        /// <returns>Async task</returns>
        public Task<PlanList> ListPlansAsync()
        {
            return this.GetAsync<PlanList>(this.CreateRequestUri(RelativePaths.Plans));
        }

        #endregion

        #region Add-ons

        /// <summary>
        /// List all add-ons
        /// </summary>
        /// <returns>Async task</returns>
        public Task<PlanAddOnList> ListAddOnsAsync()
        {
            return this.GetAsync<PlanAddOnList>(this.CreateRequestUri(RelativePaths.AddOns));
        }

        #endregion

        #region Subscriptions

        /// <summary>
        /// Delete subscription
        /// </summary>
        /// <param name="subscriptionId">subscription id</param>
        /// <param name="forceDelete">The flag to indicate whether to force deletion of the subscription from the management service regardless of its status in the underlying resource providers.</param>
        /// <returns>async task</returns>
        public Task<Subscription> DeprovisionSubscriptionAsync(string subscriptionId, bool forceDelete = false)
        {
            Uri requestUri = this.CreateRequestUri(
                string.Format(CultureInfo.InvariantCulture, RelativePaths.Subscription, subscriptionId),
                new KeyValuePair<string, string>("force", forceDelete.ToString(CultureInfo.InvariantCulture)));
            return this.SendAsync<Subscription>(requestUri, HttpMethod.Delete);
        }

        /// <summary>
        /// Adds the add-on to subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="addOn">The add on.</param>
        /// <returns>async task</returns>
        public Task<PlanAddOnReference> SubscribeToAddOn(string subscriptionId, PlanAddOnReference addOn)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.SubscriptionAddOns, subscriptionId));
            return this.SendAsync<PlanAddOnReference, PlanAddOnReference>(requestUri, HttpMethod.Post, addOn);
        }

        /// <summary>
        /// Removes the add-on from subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="addOnId">The add-on id.</param>
        /// <param name="addOnInstanceId">The add on instance id.</param>
        /// <returns>async task</returns>
        public Task UnsubscribeFromAddOnAsync(string subscriptionId, string addOnId, string addOnInstanceId)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.SubscriptionAddOn, subscriptionId, addOnId, addOnInstanceId));
            return this.SendAsync(requestUri, HttpMethod.Delete);
        }

        /// <summary>
        /// Lists the subscription usage summaries.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <returns>async task</returns>
        public Task<UsageSummaryList> ListUsageSummariesAsync(string subscriptionId)
        {
            return this.GetAsync<UsageSummaryList>(this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.SubscriptionUsageSummaries, subscriptionId)));
        }

        /// <summary>
        /// Syncs the subscription async.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <returns>async task</returns>
        public Task<Subscription> SyncSubscriptionAsync(string subscriptionId)
        {
            Uri requestUri = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, RelativePaths.SubscriptionQuota, subscriptionId), new KeyValuePair<string, string>("sync", bool.TrueString));
            return this.SendAsync<Subscription, Subscription>(requestUri, HttpMethod.Put, null);
        }

        #endregion

        #region Misc

        /// <summary>
        /// Invalidates the user token async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="tokenExpiryTime">The token expiry time.</param>
        /// <returns>Async task.</returns>
        public Task InvalidateUserTokenAsync(string userId, DateTime tokenExpiryTime)
        {
            Uri requestUri = this.CreateRequestUri(RelativePaths.InvalidUserTokens);
            var tokenToInvalidate = new UserToken()
            {
                Username = userId,
                TokenExpiryTime = tokenExpiryTime,
            };

            return this.SendAsync<UserToken>(requestUri, HttpMethod.Post, tokenToInvalidate);
        }

        #endregion

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/system.idisposable.aspx
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/system.idisposable.aspx
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this.disposed)
                {
                    this.httpClient.Dispose();
                }
            }

            this.disposed = true;
        }
    }
}
