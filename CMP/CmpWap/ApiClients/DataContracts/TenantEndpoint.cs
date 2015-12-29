//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents an tenant endpoint of a resource provider.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class TenantEndpoint : ResourceProviderEndpoint
    {
        /// <summary>
        /// Gets or sets the source URI template.
        /// </summary>
        [DataMember(Order = 4)]
        public string SourceUriTemplate { get; set; }

        /// <summary>
        /// Gets or sets the destination URI template.
        /// </summary>
        [DataMember(Order = 5)]
        public string TargetUriTemplate { get; set; }
    }
}