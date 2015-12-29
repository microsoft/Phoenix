//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Model for a query, including sorting, paging and filter string
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class Query
    {
        /// <summary>
        /// Gets or sets the number of items to skip as part of this query
        /// </summary>
        [DataMember]
        public int? Skip { get; set; }

        /// <summary>
        /// Gets or sets the number of items to take as part of this query
        /// </summary>
        [DataMember]
        public int? Take { get; set; }

        /// <summary>
        /// Gets or sets the property to sort on as part of this query
        /// </summary>
        [DataMember]
        public string SortProperty { get; set; }

        /// <summary>
        /// Gets or sets the direction on which to sort on as part of this query
        /// </summary>
        [DataMember]
        public string SortDirection { get; set; }

        /// <summary>
        /// Gets or sets the string on which to filter as part of this query
        /// </summary>
        [DataMember, DefaultValue("")]
        public string Filter { get; set; }
    }
}
