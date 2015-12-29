//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a list of usage notification events.
    /// </summary>
    /// <typeparam name="T">Type could be AdminSubscription, Plan, PlanAddon</typeparam>
    [CollectionDataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "UsageEvents", ItemName = "UsageEvent")]
    public class UsageEventList<T> : List<UsageEvent<T>>, IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the structure that contains extra data.
        /// </summary>
        /// <returns>An <see cref="T:System.Runtime.Serialization.ExtensionDataObject" /> that contains data that is not recognized as belonging to the data contract.</returns>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
