//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a lists of method names supported by Settings
    /// </summary>
    [DataContract]
    public enum SettingMethods
    {
        /// <summary>
        /// the Get method
        /// </summary>
        [EnumMember]
        Get = 0,

        /// <summary>
        /// the InsertOrUpdate method
        /// </summary>
        [EnumMember]
        InsertOrUpdate = 1,

        /// <summary>
        /// the Delete method
        /// </summary>
        [EnumMember]
        Delete = 2,
    }
}
