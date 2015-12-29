//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Class for Setting
    /// </summary>
    [DataContract]
    public class Setting : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the name of the setting.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the setting's path.
        /// </summary>
        [DataMember]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the setting's value.
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the expiration period in days
        /// </summary>
        [DataMember]
        public int? ExpirationInDays { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
