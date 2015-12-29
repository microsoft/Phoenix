//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Notification confirmation data contract
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class NotificationConfirmation
    {
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        [DataMember]
        public NotificationState State { get; set; }
    }
}
