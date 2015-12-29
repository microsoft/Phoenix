//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a plan add-on.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class PlanAddOn : IPlanEntity, IExtensibleDataObject
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
        /// Gets the state of the config.
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
        /// Gets the associated plans.
        /// </summary>
        [DataMember(Order = 9)]
        public IList<Plan> AssociatedPlans { get; set; }

        /// <summary>
        /// Gets or sets the max occurrences per plan.
        /// </summary>
        [DataMember(Order = 10)]
        public int MaxOccurrencesPerPlan { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        [DataMember(Order = 11)]
        public string Price { get; set; }

        /// <summary>
        /// Gets or sets the structure that contains extra data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
