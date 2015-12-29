//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a resource provider test result
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "TestResult")]
    public class ResourceProviderVerificationTestResult : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the test URI.
        /// </summary>
        [DataMember(Order = 0)]
        public Uri TestUri { get; set; }

        /// <summary>
        /// Gets or sets the response status code.
        /// </summary>
        [DataMember(Order = 1)]
        public string ResponseStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
