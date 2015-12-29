//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a Resource Provider reference.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class ResourceProviderReference : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the resource provider name.
        /// </summary>
        [DataMember(Order = 0)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the instance id.
        /// </summary>
        [DataMember(Order = 1)]
        public string InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the state of the configuration.
        /// </summary>
        [DataMember(Order = 2)]
        public QuotaConfigurationState ConfigState { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
