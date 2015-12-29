//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Resource metric set
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "MetricSet")]
    public class ResourceMetricSet : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets metric name.
        /// </summary>
        [DataMember(Order = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the primary aggregation type.
        /// </summary>
        [DataMember(Order = 2)]
        public string PrimaryAggregationType { get; set; }

        /// <summary>
        /// Gets or sets metric unit.
        /// </summary>
        [DataMember(Order = 3)]
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets metric time grain.
        /// </summary>
        [DataMember(Order = 4)]
        public TimeSpan TimeGrain { get; set; }

        /// <summary>
        /// Gets or sets minimum metric timestamp.
        /// </summary>
        [DataMember(Order = 5)]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets maximum metric timestamp.
        /// </summary>
        [DataMember(Order = 6)]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets collection of metric values.
        /// </summary>
        [DataMember(Order = 7)]
        public List<ResourceMetricSample> Values { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
