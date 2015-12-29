// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Globalization;
using Microsoft.WindowsAzurePack.Samples.DataContracts;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models
{   
    //This is a model class which contains data contract we send to Controller which then shows up in UI
    //EndPointModel contains data related to resource provider endpoint which is registered with Admin API
    public class EndpointModel
    {
        /// <summary>
        /// Address of CmpWapExtension resource provider
        /// </summary>
        public string EndpointAddress { get; set; }

        /// <summary>
        /// Username used by Admin API to connect with CmpWapExtension Resource Provider
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password used by Admin API to connect with CmpWapExtension Resource Provider
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Convert to the admin endpoint API contract.
        /// This endpoint point is used by Admin API
        /// </summary>
        public ResourceProviderEndpoint ToAdminEndpoint()
        {
            return new ResourceProviderEndpoint()
            {
                ForwardingAddress = new Uri(this.EndpointAddress + "/admin"),
                AuthenticationMode = AuthenticationMode.Basic,
                AuthenticationUsername = this.Username,
                AuthenticationPassword = this.Password
            };
        }

        /// <summary>
        /// To the notification endpoint API contract.
        /// /// This endpoint point is used for provisioing subscriptions
        /// </summary>
        public ResourceProviderEndpoint ToNotificationEndpoint()
        {
            return new ResourceProviderEndpoint()
            {
                ForwardingAddress = new Uri(this.EndpointAddress + "/admin"),
                AuthenticationMode = AuthenticationMode.Basic,
                AuthenticationUsername = this.Username,
                AuthenticationPassword = this.Password
            };
        }

        /// <summary>
        /// To the tenant endpoint API contract.
        /// /// This endpoint point is used by Tenant API
        /// </summary>
        public TenantEndpoint ToTenantEndpoint()
        {
            return new TenantEndpoint()
            {
                ForwardingAddress = new Uri(this.EndpointAddress),
                AuthenticationMode = AuthenticationMode.Basic,
                AuthenticationUsername = this.Username,
                AuthenticationPassword = this.Password,
                SourceUriTemplate = string.Format(CultureInfo.InvariantCulture, "{{subid}}/services/{0}/{{*path}}", CmpWapExtensionClient.RegisteredServiceName),
                TargetUriTemplate = "subscriptions/{subid}/{*path}"
            };
        }

        /// <summary>
        /// To the usage endpoint API contract.
        /// This endpoint point is used to expose usage information 
        /// </summary>
        public ResourceProviderEndpoint ToUsageEndpoint()
        {
            return new ResourceProviderEndpoint()
            {
                ForwardingAddress = new Uri(this.EndpointAddress + "/usage"),
                AuthenticationMode = AuthenticationMode.Basic,
                AuthenticationUsername = this.Username,
                AuthenticationPassword = this.Password,                
            };
        }

        /// <summary>
        /// Convert from the resource provider endpoint.
        /// </summary>
        /// <param name="resourceProviderEndpoint">The resource provider endpoint.</param>
        public static EndpointModel FromResourceProviderEndpoint(ResourceProviderEndpoint resourceProviderEndpoint)
        {
            return new EndpointModel()
            {
                EndpointAddress = resourceProviderEndpoint.ForwardingAddress.AbsoluteUri,
                Username = resourceProviderEndpoint.AuthenticationUsername,
                Password = resourceProviderEndpoint.AuthenticationPassword
            };
        }
    }
}
