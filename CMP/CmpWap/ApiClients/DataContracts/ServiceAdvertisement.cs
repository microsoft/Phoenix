//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a service advertisement.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "ServiceAdvertisement")]
    public class ServiceAdvertisement : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the service name.
        /// </summary>
        [DataMember(Order = 0, IsRequired = true)]
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the service display name.
        /// </summary>
        [DataMember(Order = 1)]
        public string ServiceDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the quotas.
        /// </summary>
        [DataMember(Order = 2)]
        public QuotaList Quotas { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}