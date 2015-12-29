//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// A base class for ResourceProvider endpoints
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class ResourceProviderEndpoint : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the forwarding address.
        /// </summary>
        [DataMember(Order = 0, IsRequired = true)]
        public Uri ForwardingAddress { get; set; }

        /// <summary>
        /// Gets or sets the authentication mode.
        /// </summary>
        [DataMember(Order = 1)]
        public AuthenticationMode AuthenticationMode { get; set; }

        /// <summary>
        /// Gets or sets the authentication username.
        /// </summary>
        [DataMember(Order = 2)]
        public string AuthenticationUsername { get; set; }

        /// <summary>
        /// Gets or sets the authentication password.
        /// </summary>
        [DataMember(Order = 3)]
        public string AuthenticationPassword { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
