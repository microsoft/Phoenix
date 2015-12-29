//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Usage Notification Event
    /// </summary>
    /// <typeparam name="T">Type could be AdminSubscription, Plan, PlanAddon</typeparam>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class UsageEvent<T> : UsageEvent
    {
        /// <summary>
        /// Gets or sets the final state of the entity.
        /// </summary>
        [DataMember]
        public T Entity { get; set; }
    }

    /// <summary>
    /// Usage Notification Event
    /// </summary>
    /// <typeparam name="T">Type could be AdminSubscription, Plan, PlanAddon</typeparam>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public abstract class UsageEvent
    {
        /// <summary>
        /// Gets or sets the method performed on the entity. (POST, PUT, etc.)
        /// </summary>
        [DataMember]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the entity parent id.
        /// </summary>
        [DataMember]
        public string EntityParentId { get; set; }

        /// <summary>
        /// Gets or sets the time of the notification event.
        /// </summary>
        [DataMember]
        public DateTime NotificationEventTimeCreated { get; set; }

        /// <summary>
        /// Gets or sets the event id.
        /// </summary>
        [DataMember]
        public long EventId { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        [DataMember]
        public NotificationState State { get; set; }

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        public UsageEventType EventType { get; set; }
    }
}
