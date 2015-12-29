//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Subscription data contract between Management service and resource providers
    /// </summary>
    [DataContract(Name = "Subscription", Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class ResourceProviderSubscription : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the subscription id.
        /// </summary>
        [DataMember(Order = 0)]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the name of the subscription.
        /// </summary>
        [DataMember(Order = 1)]
        public string SubscriptionName { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        [DataMember(Order = 2)]
        public ResourceProviderSubscriptionState State { get; set; }

        /// <summary>
        /// Gets or sets the state of the lifecycle (Only required if Resource Provider opts to implement asynchronous protocols).
        /// </summary>
        [DataMember(Order = 3)]
        public SubscriptionLifecycleState LifecycleState { get; set; }

        /// <summary>
        /// Gets or sets the last error message (Only required if Resource Provider opts to implement asynchronous protocols).
        /// </summary>
        [DataMember(Order = 4)]
        public string LastErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the quota settings.
        /// </summary>
        [DataMember(Order = 5)]
        public IList<ServiceQuotaSetting> QuotaSettings { get; set; }

        /// <summary>
        /// Gets or sets the subscription admin id.
        /// </summary>
        [DataMember(Order = 6)]
        public string AdminId { get; set; }

        /// <summary>
        /// Gets the co admin ids.
        /// </summary>
        [DataMember(Order = 7)]
        public IList<string> CoAdminIds { get; set; }

        /// <summary>
        /// Gets or sets the structure that contains extra data (for forward compatibility).
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
