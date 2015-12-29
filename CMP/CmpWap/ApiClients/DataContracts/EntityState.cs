//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// State of entity.
    /// </summary>
    [DataContract]
    public enum EntityState
    {
        /// <summary>
        /// Deleted state
        /// </summary>
        [EnumMember]
        Deleted,

        /// <summary>
        /// Enabled state
        /// </summary>
        [EnumMember]
        Enabled,

        /// <summary>
        /// Disabled state
        /// </summary>
        [EnumMember]
        Disabled,

        /// <summary>
        /// Migrated state
        /// </summary>
        [EnumMember]
        Migrated,

        /// <summary>
        /// Updated state
        /// </summary>
        [EnumMember]
        Updated,

        /// <summary>
        /// Registered state
        /// </summary>
        [EnumMember]
        Registered,

        /// <summary>
        /// Unregistered state
        /// </summary>
        [EnumMember]
        Unregistered,
    }
}
