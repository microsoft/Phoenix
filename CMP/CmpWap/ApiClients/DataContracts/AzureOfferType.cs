//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Azure offer type.
    /// </summary>
    [DataContract(Namespace = "http://www.microsoft.com/Azure/ProvisioningAgent/1.0")]
    public enum AzureOfferType
    {
        /// <summary>
        /// Buy type.
        /// </summary>
        [EnumMember]
        Buy = 1,

        /// <summary>
        /// Trail type.
        /// </summary>
        [EnumMember]
        Trial = 0,
    }
}
