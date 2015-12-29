//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Resource provider verification result API contract.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class ResourceProviderVerificationResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the verification has one or more failures
        /// </summary>
        [DataMember(Order = 0)]
        public bool HasFailures { get; set; }

        /// <summary>
        /// Gets or sets the detailed results.
        /// </summary>
        [DataMember(Order = 1)]
        public IList<ResourceProviderVerificationTestResult> DetailedResults { get; set; }
    }
}
