//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a quota update batch. This is used between the Management Service and resource providers to synchronize quota configuration.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class QuotaUpdateBatch : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the base quota.
        /// </summary>
        [DataMember(Order = 0)]
        public ServiceQuotaSettingList BaseQuota { get; set; }

        /// <summary>
        /// Gets the add on quotas.
        /// </summary>
        [DataMember(Order = 1)]
        public IList<ServiceQuotaSettingList> AddOnQuotas { get; set; }

        /// <summary>
        /// Gets or sets the subscription ids to update.
        /// </summary>
        [DataMember(Order = 2)]
        public IList<string> SubscriptionIdsToUpdate { get; set; }

        /// <summary>
        /// Gets or sets the structure that contains extra data (for forward compatibility).
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
