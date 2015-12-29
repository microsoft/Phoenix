//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Data type for point-in-time metric event
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "MetricSample")]
    public class ResourceMetricSample : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets metric timestamp.
        /// </summary>
        [DataMember(Order = 1)]
        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// Gets or sets total value.
        /// </summary>
        [DataMember(Order = 2)]
        public double? Total { get; set; }

        /// <summary>
        /// Gets or sets minimum value.
        /// </summary>
        [DataMember(Order = 3)]
        public double? Minimum { get; set; }

        /// <summary>
        /// Gets or sets maximum value.
        /// </summary>
        [DataMember(Order = 4)]
        public double? Maximum { get; set; }

        /// <summary>
        /// Gets or sets count value.
        /// </summary>
        [DataMember(Order = 5)]
        public int? Count { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
