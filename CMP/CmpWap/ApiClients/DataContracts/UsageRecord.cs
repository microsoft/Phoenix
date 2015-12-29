//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Usage Record
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class UsageRecord
    {
        /// <summary>
        /// Gets the record unique id.
        /// </summary>
        [DataMember]
        public long EventId { get; set; }

        /// <summary>
        /// Gets the external record unique id.
        /// </summary>
        [DataMember]
        public string ExternalRecordId { get; set; }

        /// <summary>
        /// Gets the resource id.
        /// </summary>
        [DataMember]
        public string ResourceId { get; set; }

        /// <summary>
        /// Gets the usage start date.
        /// </summary>
        [DataMember]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets usage the end date.
        /// </summary>
        [DataMember]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets the resource provider id.
        /// </summary>
        [DataMember]
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets the service type.
        /// </summary>
        [DataMember]
        public string ServiceType { get; set; }

        /// <summary>
        /// Gets the subscription id.
        /// </summary>
        [DataMember]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets the record generic properties.
        /// </summary>
        [DataMember]
        public IDictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Gets the record resource type.
        /// </summary>
        [DataMember]
        public IDictionary<string, string> Resources { get; set; }
    }
}