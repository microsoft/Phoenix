//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a provider service. Compatible with RDFE's Microsoft.WindowsAzure.ResourceManagement.Service.  
    /// </summary>
    //// We use 'ProviderService' for the class name instead of 'Service' to avoid collision with our Service namespace and it's more meaningful as well.
    [DataContract(Name = "Service", Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class ProviderService : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the servicey type.
        /// </summary>
        [DataMember(Order = 1)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the service state.
        /// </summary>
        [DataMember(Order = 2)]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
