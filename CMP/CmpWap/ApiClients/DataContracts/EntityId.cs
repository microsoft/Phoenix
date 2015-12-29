//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Type representing identifier of Entity under subscription.
    /// </summary>
    [DataContract]
    public class EntityId
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        [DataMember(IsRequired = true, Order = 0)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets Created
        /// </summary>
        [DataMember(IsRequired = false, Order = 1)]
        public DateTime Created { get; set; }
    }
}
