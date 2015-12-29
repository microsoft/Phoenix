//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represent a usage item in the usage summary.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class Usage
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        [DataMember(Order = 0)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        [DataMember(Order = 1)]
        public long CurrentValue { get; set; }

        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        [DataMember(Order = 2)]
        public long Limit { get; set; }

        /// <summary>
        /// Gets or sets the display name of the unit.
        /// </summary>
        [DataMember(Order = 3)]
        public string UnitDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the group id (optional).
        /// </summary>
        [DataMember(Order = 4)]
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the group (optional).
        /// </summary>
        [DataMember(Order = 5)]
        public string GroupDisplayName { get; set; }
    }
}
