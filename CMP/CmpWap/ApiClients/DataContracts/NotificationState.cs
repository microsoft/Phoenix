//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Notification state data contract
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public enum NotificationState
    {
        /// <summary>
        /// Acknowledged by the Management Service that the operation was committed
        /// </summary>
        [EnumMember]
        Acknowledged = 0,

        /// <summary>
        /// Rejected by the Billing Service
        /// </summary>
        [EnumMember]
        Rejected = 1,

        /// <summary>
        /// Pending Billing Service's approval
        /// </summary>
        [EnumMember]
        PendingApproval = 2,

        /// <summary>
        /// Approved by the Billing Service
        /// </summary>
        [EnumMember]
        Approved = 3,
    }
}
