//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Represents a notification event.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    public class NotificationEvent<T>
    {
        /// <summary>
        /// Gets or sets the method performed on the entity. (POST, PUT, etc.)
        /// </summary>
        [DataMember]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the final state of the entity.
        /// </summary>
        [DataMember]
        public T Entity { get; set; }

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
    }
}