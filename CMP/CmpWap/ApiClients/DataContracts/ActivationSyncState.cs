//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represent an activation sync state.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public enum ActivationSyncState
    {
        /// <summary>
        /// In sync
        /// </summary>
        [EnumMember]
        InSync = 0,

        /// <summary>
        /// Syncing
        /// </summary>
        [EnumMember]
        Syncing,

        /// <summary>
        /// Out of Sync
        /// </summary>
        [EnumMember]
        OutOfSync
    }
}
