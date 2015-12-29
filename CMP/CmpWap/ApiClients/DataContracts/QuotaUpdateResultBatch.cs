//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a quota update result batch. This is used between the Management Service and resource providers to synchronize quota configuration.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    internal class QuotaUpdateResultBatch : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the updated subscription ids.
        /// </summary>
        [DataMember(Order = 1)]
        public IList<string> UpdatedSubscriptionIds { get; set; }

        /// <summary>
        /// Gets or sets the failed subscription ids.  RDFE will not assume that the quota has been updated or otherwise, it will simply mark this subscription as OutOfSync.
        /// </summary>
        [DataMember(Order = 2)]
        public IList<string> FailedSubscriptionIds { get; set; }

        /// <summary>
        /// Gets or sets the error.  This is a representative of all errors in the batch.  It can simply be the error of the first failed subscription. This should be null in the successful case.
        /// </summary>
        [DataMember(Order = 3)]
        public ResourceProviderError Error { get; set; }

        /// <summary>
        /// Gets or sets the structure that contains extra data (for forward compatibility).
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
