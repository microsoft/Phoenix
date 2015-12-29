//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a service enabled by a subscription.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class SubscriptionService : IQuotaSyncable, IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [DataMember(Order = 0)]
        public string Type { get; set; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        [DataMember(Order = 1)]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the quota sync state.
        /// </summary>
        [DataMember(Order = 2)]
        public QuotaSyncState QuotaSyncState { get; set; }

        /// <summary>
        /// Gets or sets the activation sync state.
        /// </summary>
        [DataMember(Order = 3)]
        public ActivationSyncState ActivationSyncState { get; set; }

        /// <summary>
        /// Gets or sets the base quota settings.
        /// </summary>
        [DataMember(Order = 4)]
        public IList<ServiceQuotaSetting> BaseQuotaSettings { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}