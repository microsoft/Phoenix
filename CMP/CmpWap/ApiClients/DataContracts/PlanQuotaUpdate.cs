//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a plan quota to update.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class PlanQuotaUpdate
    {
        /// <summary>
        /// Gets the service quotas.
        /// </summary>
        [DataMember(Order = 0)]
        public IList<ServiceQuota> ServiceQuotas { get; internal set; }        
    }
}
