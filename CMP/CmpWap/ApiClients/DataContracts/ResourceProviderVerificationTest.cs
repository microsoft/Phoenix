//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a resource provider test Uri
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class ResourceProviderVerificationTest : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the test URI.
        /// </summary>
        [DataMember(Order = 0)]
        public Uri TestUri { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this test is for the Admin API.
        /// </summary>
        [DataMember(Order = 1)]
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
