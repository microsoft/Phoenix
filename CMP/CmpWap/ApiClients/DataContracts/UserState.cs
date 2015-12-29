//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// User state.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public enum UserState
    {
        /// <summary>
        /// User state is pending validation.
        /// </summary>
        [EnumMember]
        PendingValidation = 0,

        /// <summary>
        /// User state is active.
        /// </summary>
        [EnumMember]
        Active = 1,

        /// <summary>
        /// User state is suspended.
        /// </summary>
        [EnumMember]
        Suspended = 2,

        /// <summary>
        /// The user is being deleted or partially deleted
        /// Only deletion operation will be allowed for subscriptions in this state
        /// </summary>
        [EnumMember]
        DeletePending = 3,
    }
}
