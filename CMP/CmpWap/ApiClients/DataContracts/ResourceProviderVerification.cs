//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Resource provider verification API contract.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class ResourceProviderVerification
    {
        /// <summary>
        /// Gets or sets the resource provider.
        /// </summary>
        [DataMember(Order = 0)]
        public ResourceProvider ResourceProvider { get; set; }

        /// <summary>
        /// Gets or sets the list of verification tests.
        /// </summary>
        [DataMember(Order = 1)]
        public ResourceProviderVerificationTestList Tests { get; set; }
    }
}
