//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a subscription state.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public enum SubscriptionState
    {
        /// <summary>
        /// No state change during the Update API.
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// The subscription is active
        /// </summary>
        [EnumMember]
        Active = 1,

        /// <summary>
        /// The subscription is suspended
        /// </summary>
        [EnumMember]
        Suspended = 2,

        /// <summary>
        /// The subscription is being deleted or partially deleted.
        /// Only deletion operation will be allowed for subscriptions in this state
        /// </summary>
        [EnumMember]
        DeletePending = 3,
    }
}
