//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// This class holds the identifiers for a Setting Store (the store name, type, and ID)
    /// </summary>
    [DataContract]
    public class SettingStorePartitions
    {
        /// <summary>
        /// Gets or sets the store name
        /// </summary>
        [DataMember]
        public string Store { get; set; }

        /// <summary>
        /// Gets or sets the store type
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the ids
        /// </summary>
        [DataMember]
        public IList<string> Ids { get; set; }
    }
}
