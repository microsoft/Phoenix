//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Offer state.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public enum FanoutState
    {
        /// <summary>
        /// Resource state is InSync
        /// </summary>
        [EnumMember]
        InSync = 0,

        /// <summary>
        /// Resource state is OutOfSync
        /// </summary>
        [EnumMember]
        OutOfSync = 1,

        /// <summary>
        /// Resource state is Invalid
        /// </summary>
        [EnumMember]
        Invalid = 2,
    }
}
