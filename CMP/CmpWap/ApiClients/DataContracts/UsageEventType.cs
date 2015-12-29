//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Runtime.Serialization;
namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Usage Notification Event Type
    /// </summary>
    public enum UsageEventType
    {
        /// <summary>
        /// Represents a Plan
        /// </summary>
        [EnumMember]
        Plan,

        /// <summary>
        /// Represents a Subscription
        /// </summary>
        [EnumMember]
        Subscription,

        /// <summary>
        /// Represents an add-on
        /// </summary>
        [EnumMember]
        AddOn,

        /// <summary>
        /// Represents a change in association between a Plan and an add-on
        /// </summary>
        [EnumMember]
        PlanAddOn,

        /// <summary>
        /// Represents a change in association between a Subscription and an add-on
        /// </summary>
        [EnumMember]
        SubscriptionAddOn,

        /// <summary>
        /// Represents a change in association between a Resource Provider and a Plan
        /// </summary>
        [EnumMember]
        PlanService,

        /// <summary>
        /// Represents a change in association between a Resource Provider and an add-on
        /// </summary>
        [EnumMember]
        AddOnService,
    }
}
