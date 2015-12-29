//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a plan state.
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public enum PlanState
    {
        /// <summary>
        /// The plan is private.  Only Admin can see and manage it. 
        /// </summary>
        [EnumMember]
        Private = 0,

        /// <summary>
        /// The plan is public.  Tenants can see an self-subscribe to it.
        /// </summary>
        [EnumMember]
        Public,

        /// <summary>
        /// The plan is decommissioned. New subscriptions will not be able to use the add-on. 
        /// </summary>
        [EnumMember]
        Decommissioned,
    }
}
