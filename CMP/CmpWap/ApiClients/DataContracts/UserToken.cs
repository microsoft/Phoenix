//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// The User.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "UserToken")]
    public class UserToken : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember(Order = 0)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [DataMember(Order = 1)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the token expiry time.
        /// </summary>
        [DataMember(Order = 2)]
        public DateTime TokenExpiryTime { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
