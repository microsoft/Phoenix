//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a quota configuration synchronization state between a subscription and its plan.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public enum SubscriptionPlanSyncState
    {
        /// <summary>
        /// The subscription's quota is in sync with its plan.
        /// </summary>
        [EnumMember]
        InSync,

        /// <summary>
        /// The subscription's quota is out of sync with its plan.
        /// </summary>
        [EnumMember]
        OutOfSync,

        /// <summary>
        /// The subscription's quota is being configured.
        /// </summary>
        [EnumMember]
        Configuring,
    }
}
