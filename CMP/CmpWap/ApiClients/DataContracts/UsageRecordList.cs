//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a list of usage records.
    /// </summary>
    [CollectionDataContract(Name = "UsageRecords", ItemName = "UsageRecord", Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class UsageRecordList : List<UsageRecord>, IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the structure that contains extra data.
        /// </summary>
        /// <returns>An <see cref="T:System.Runtime.Serialization.ExtensionDataObject"/> that contains data that is not recognized as belonging to the data contract.</returns>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
