//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a subscription.  Compatible with RDFE's Microsoft.WindowsAzure.Management.Subscription.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class Subscription : IAddOnContainer, IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the subscription ID.
        /// </summary>
        [DataMember(Order = 0)]
        public string SubscriptionID { get; set; }

        /// <summary>
        /// Gets or sets the subscription name.
        /// </summary>
        [DataMember(Order = 1)]
        public string SubscriptionName { get; set; }

        /// <summary>
        /// Gets or sets the account admin email address.
        /// </summary>
        [DataMember(Order = 2)]
        public string AccountAdminLiveEmailId { get; set; }

        /// <summary>
        /// Gets or sets the service admin email address.
        /// </summary>
        [DataMember(Order = 3)]
        public string ServiceAdminLiveEmailId { get; set; }

        /// <summary>
        /// Gets or sets the co-admin names.
        /// </summary>
        [DataMember(Order = 4)]
        public IList<string> CoAdminNames { get; set; }

        /// <summary>
        /// Gets or sets the features.
        /// </summary>
        [DataMember(Order = 14)]
        public object[] Features { get; set; }

        /// <summary>
        /// Gets or sets the offer category.
        /// </summary>
        [DataMember(Order = 16)]
        public string OfferCategory { get; set; }

        /// <summary>
        /// Gets or sets the offer friendly id.
        /// </summary>
        [DataMember(Order = 15)]
        public string OfferFriendlyName { get; set; }

        /// <summary>
        /// Gets the registered services.
        /// </summary>
        public string RegisteredServices { get; set; }

        /// <summary>
        /// Gets or sets the subscription created time.
        /// </summary>
        [DataMember(Order = 18)]
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets the add on references.
        /// </summary>
        [DataMember(Order = 6)]
        public IList<PlanAddOnReference> AddOnReferences { get; set; }

        /// <summary>
        /// Gets the add-ons.
        /// </summary>
        [DataMember(Order = 7)]
        public IList<PlanAddOn> AddOns { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        [DataMember(Order = 8)]
        public SubscriptionState State { get; set; }

        /// <summary>
        /// Gets the quota sync state.
        /// </summary>
        [DataMember(Order = 9)]
        public QuotaSyncState QuotaSyncState { get; set; }

        /// <summary>
        /// Gets the activation state.
        /// </summary>
        [DataMember(Order = 10)]
        public ActivationSyncState ActivationSyncState { get; set; }

        /// <summary>
        /// Gets or sets the plan id.
        /// </summary>
        [DataMember(Order = 11)]
        public string PlanId { get; set; }

        /// <summary>
        /// Gets registered services mimic azure subscription(this is a workaround)
        /// </summary>
        /// <returns>List of Registered Services</returns>
        [DataMember(Order = 12)]
        public IList<SubscriptionService> Services { get; set; }

        /// <summary>
        /// Gets or sets the last error message.
        /// </summary>
        [DataMember(Order = 13)]
        public string LastErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
