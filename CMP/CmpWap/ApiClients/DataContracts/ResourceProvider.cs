//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a resource provider.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class ResourceProvider : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the resource provider name.
        /// </summary>
        [DataMember(Order = 0, IsRequired = true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        [DataMember(Order = 1)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the resource provider description.
        /// </summary>
        [DataMember(Order = 2)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the resource provider is enabled.
        /// </summary>
        [DataMember(Order = 3)]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pass-through proxy is enabled.
        /// </summary>
        [DataMember(Order = 4)]
        public bool PassThroughEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow anonymous access.
        /// </summary>
        [DataMember(Order = 5)]
        public bool AllowAnonymousAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow multiple instances].
        /// </summary>
        [DataMember(Order = 6)]
        public bool AllowMultipleInstances { get; set; }

        /// <summary>
        /// Gets or sets the admin endpoint.
        /// </summary>
        [DataMember(Order = 7)]
        public ResourceProviderEndpoint AdminEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the tenant endpoint.
        /// </summary>
        [DataMember(Order = 8)]
        public TenantEndpoint TenantEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the usage endpoint.
        /// </summary>
        [DataMember(Order = 9)]
        public ResourceProviderEndpoint UsageEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the health check endpoint.
        /// </summary>
        [DataMember(Order = 10)]
        public ResourceProviderEndpoint HealthCheckEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the notification endpoint.
        /// </summary>
        [DataMember(Order = 11)]
        public ResourceProviderEndpoint NotificationEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the instance id.
        /// </summary>
        [DataMember(Order = 12)]
        public string InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the instance.
        /// </summary>
        [DataMember(Order = 13)]
        public string InstanceDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the size of the max quota update batch.
        /// </summary>
        [DataMember(Order = 14)]
        public int MaxQuotaUpdateBatchSize { get; set; }

        /// <summary>
        /// Gets or sets the subscription status update polling interval.
        /// </summary>
        [DataMember(Order = 15)]
        public TimeSpan SubscriptionStatusPollingInterval { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [DataMember(Order = 16)]
        public ResourceProviderType Type { get; set; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        [DataMember(Order = 17)]
        public IList<ServiceSetting> Settings { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}