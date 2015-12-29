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
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "User")]
    public class User : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember(Order = 0)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [DataMember(Order = 1)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user state.
        /// </summary>
        [DataMember(Order = 2)]
        public UserState State { get; set; }

        /// <summary>
        /// Gets or sets the user created time.
        /// </summary>
        [DataMember(Order = 3)]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Gets or sets the number of subscriptions owned by this user.
        /// </summary>
        [DataMember(Order = 4)]
        public int SubscriptionCount { get; set; }

        /// <summary>
        /// Gets or sets the activation sync state.
        /// </summary>
        [DataMember(Order = 5)]
        public ActivationSyncState ActivationSyncState { get; set; }

        /// <summary>
        /// Gets or sets the last error message.
        /// </summary>
        [DataMember(Order = 6)]
        public string LastErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
