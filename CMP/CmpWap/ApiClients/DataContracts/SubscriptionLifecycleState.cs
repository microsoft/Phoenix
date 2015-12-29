//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a subscription lifecycle state.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public enum SubscriptionLifecycleState
    {
        /// <summary>
        /// The subscription is provisioned.
        /// </summary>
        [EnumMember]
        Provisioned = 0,

        /// <summary>
        /// The subscription is being provisioned.
        /// </summary>
        [EnumMember]
        Provisioning,

        /// <summary>
        /// The subscription is being updated.
        /// </summary>
        [EnumMember]
        Updating,

        /// <summary>
        /// The subscription is being deleted.
        /// </summary>
        [EnumMember]
        Deleting,

        /// <summary>
        /// The subscription has been deleted.  This state is optional and it's semantically the same as subscription does not exist (404).
        /// This is for the case when a Resource Provider need to keep the subscription record around (e.g. for billing). 
        /// </summary>
        [EnumMember]
        Deleted,

        /// <summary>
        /// The subscription is out-of-sync due to an error from a previous operation.
        /// </summary>
        [EnumMember]
        OutOfSync,
    }
}
