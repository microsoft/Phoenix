//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a plan.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class Plan : IPlanEntity, IAddOnContainer, IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember(Order = 0)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        [DataMember(Order = 1)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        [DataMember(Order = 2)]
        public PlanState State { get; set; }

        /// <summary>
        /// Gets the state of the configuration.
        /// </summary>
        [DataMember(Order = 3)]
        public QuotaConfigurationState ConfigState { get; set; }


        /// <summary>
        /// Gets the quota sync state.
        /// </summary>
        [DataMember(Order = 4)]
        public QuotaSyncState QuotaSyncState { get; set; }

        /// <summary>
        /// Gets or sets the last error message.
        /// </summary>
        [DataMember(Order = 5)]
        public string LastErrorMessage { get; set; }

        /// <summary>
        /// Gets the advertisements.
        /// </summary>
        [DataMember(Order = 6)]
        public IList<PlanAdvertisement> Advertisements { get; set; }

        /// <summary>
        /// Gets the service quotas.
        /// </summary>
        [DataMember(Order = 7)]
        public IList<ServiceQuota> ServiceQuotas { get; set; }

        /// <summary>
        /// Gets or sets the subscription count.
        /// </summary>
        [DataMember(Order = 8)]
        public int SubscriptionCount { get; set; }

        /// <summary>
        /// Gets or sets the max subscriptions per account.
        /// </summary>
        [DataMember(Order = 9)]
        public int MaxSubscriptionsPerAccount { get; set; }

        /// <summary>
        /// Gets the add on references.
        /// </summary>
        [DataMember(Order = 10)]
        public IList<PlanAddOnReference> AddOnReferences { get; set; }

        /// <summary>
        /// Gets the add-ons.
        /// </summary>
        [DataMember(Order = 11)]
        public IList<PlanAddOn> AddOns { get; set; }

        /// <summary>
        /// Gets or sets the invitation code.
        /// </summary>
        [DataMember(Order = 12)]
        public string InvitationCode { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        [DataMember(Order = 13)]
        public string Price { get; set; }
       
        /// <summary>
        /// Gets or sets the structure that contains extra data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
