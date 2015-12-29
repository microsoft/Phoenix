//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represent a quota configuration of a particular service.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class ServiceQuota : IQuotaSyncable, IQuotaConfigurable, IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        [DataMember(Order = 0)]
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the service instance id. Note that the tenant API should not populate this property.
        /// </summary>
        [DataMember(Order = 1)]
        public string ServiceInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the service.
        /// </summary>
        [DataMember(Order = 2)]
        public string ServiceDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the display name of the service instance.
        /// </summary>
        [DataMember(Order = 3)]
        public string ServiceInstanceDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the state of the configuration.
        /// </summary>
        [DataMember(Order = 4)]
        public QuotaConfigurationState ConfigState { get; set; }

        /// <summary>
        /// Gets or sets the state of the sync.
        /// </summary>
        [DataMember(Order = 5)]
        public QuotaSyncState QuotaSyncState { get; set; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        [DataMember(Order = 6)]
        public IList<ServiceQuotaSetting> Settings { get; set; }

        /// <summary>
        /// Gets or sets the structure that contains extra data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
