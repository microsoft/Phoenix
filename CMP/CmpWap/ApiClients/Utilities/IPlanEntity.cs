//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Interface for Plan and AddOn
    /// </summary>
    public interface IPlanEntity
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Gets the advertisement.
        /// </summary>
        IList<PlanAdvertisement> Advertisements { get; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        PlanState State { get; set; }

        /// <summary>
        /// Gets the quota sync state.
        /// </summary>
        QuotaSyncState QuotaSyncState { get; }

        /// <summary>
        /// Gets or sets the last error message.
        /// </summary>
        string LastErrorMessage { get; set; }

        /// <summary>
        /// Gets the service quotas.
        /// </summary>
        IList<ServiceQuota> ServiceQuotas { get; }

        /// <summary>
        /// Gets or sets the subscription count.
        /// </summary>
        int SubscriptionCount { get; set; }
    }
}
