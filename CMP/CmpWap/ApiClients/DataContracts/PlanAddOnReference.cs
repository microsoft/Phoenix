//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a Plan-AddOn reference.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class PlanAddOnReference
    {
        /// <summary>
        /// Gets or sets the add on id.
        /// </summary>
        [DataMember(Order = 0)]
        public string AddOnId { get; set; }

        /// <summary>
        /// Gets the add on instance id.
        /// </summary>
        [DataMember(Order = 1)]
        public string AddOnInstanceId { get; set; }

        /// <summary>
        /// Gets the acquisition time.
        /// </summary>
        [DataMember(Order = 2)]
        public DateTime? AcquisitionTime { get; set; }
    }
}
