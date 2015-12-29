//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a service usage summary.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class ServiceUsageSummary
    {
        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        [DataMember(Order = 0)]
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the display name of the service.
        /// </summary>
        [DataMember(Order = 1)]
        public string ServiceDisplayName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the usage data is retrieved successfully from the service.
        /// </summary>
        [DataMember(Order = 2)]
        public bool RetrievedSuccessfully { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [DataMember(Order = 3)]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the usages.
        /// </summary>
        [DataMember(Order = 4)]
        public IList<Usage> Usages { get; set; }
    }
}
