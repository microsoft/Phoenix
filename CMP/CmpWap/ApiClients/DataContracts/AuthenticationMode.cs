//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents an authentication mode of a resource provider.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public enum AuthenticationMode
    {
        /// <summary>
        /// No authentication.
        /// </summary>
        [EnumMember]
        None,

        /// <summary>
        /// Basic authentication.
        /// </summary>
        [EnumMember]
        Basic,

        /// <summary>
        /// Windows authentication.
        /// </summary>
        [EnumMember]
        Windows,
    }
}