//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Error contract between the Management service and resource providers
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "Error")]
    public class ResourceProviderError : IExtensibleDataObject
    {
        /// <summary>
        /// Gets Error code
        /// </summary>
        [DataMember(Order = 1)]
        public string Code { get; set; }

        /// <summary>
        /// Gets Error message
        /// </summary>
        [DataMember(Order = 2)]
        public string Message { get; set; }

        /// <summary>
        /// Gets Extended code
        /// </summary>
        [DataMember(Order = 3)]
        public string ExtendedCode { get; set; }

        /// <summary>
        /// Gets Message template
        /// </summary>
        [DataMember(Order = 4)]
        public string MessageTemplate { get; set; }

        /// <summary>
        /// Gets Error parameters
        /// </summary>
        [DataMember(Order = 5)]
        public List<string> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the inner errors.
        /// </summary>
        [DataMember(Order = 6)]
        public List<ResourceProviderError> InnerErrors { get; set; }

        /// <summary>
        /// Gets or sets the structure that contains extra data (for forward compatibility).
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
