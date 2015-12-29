//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a quota
    /// </summary>
    [DataContract(Name = "Quota", Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class Quota : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the quota name.
        /// </summary>
        [DataMember(Order = 0, IsRequired = true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the quota unit.
        /// </summary>
        [DataMember(Order = 1)]
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the quota quantity.
        /// </summary>
        [DataMember(Order = 2)]
        public double? Quantity { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
