//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// The status of a subscription
    /// </summary>
    public enum SubscriptionStatus
    {
        /// <summary>
        /// Subscription is Active and tenant has access to resources
        /// </summary>
        [EnumMember]
        Active,

        /// <summary>
        /// Subscription is suspended and tenant does not have access to resources (the resource have not been deleted)
        /// </summary>
        [EnumMember]
        Suspended,
    }
}