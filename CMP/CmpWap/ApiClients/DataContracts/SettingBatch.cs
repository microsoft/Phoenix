//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Class for Setting Batch (request body)
    /// </summary>
    [DataContract]
    public class SettingBatch : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the method to be performed on the batch
        /// </summary>
        [DataMember]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the SettingStorePartitions
        /// </summary>
        [DataMember]
        public SettingStorePartitions SettingStorePartitions { get; set; }

        /// <summary>
        /// Gets or sets the collection of settings
        /// </summary>
        [DataMember]
        public SettingCollection SettingCollection { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// Returns a string representation of the object. User for logging purposes.
        /// </summary>
        public override string ToString()
        {
            return string.Format("SettingBatch: {{ Method : \"{0}\", Store : \"{1}\", Type: \"{2}\" }}", this.Method, this.SettingStorePartitions.Store, this.SettingStorePartitions.Type);
        }
    }
}
