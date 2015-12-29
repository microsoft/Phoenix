//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represent a configuration state for plans and add-ons.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public enum QuotaConfigurationState
    {
        /// <summary>
        /// Plan/add-on is not configured.
        /// </summary>
        [EnumMember]
        NotConfigured = 0,

        /// <summary>
        /// Plan/add-on is configured.
        /// </summary>
        [EnumMember]
        Configured,
    }
}
