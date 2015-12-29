// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// A management certificate for a subscription
    /// </summary>
    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure", Name = "SubscriptionCertificate")]
    public class SubscriptionCertificate
    {
        /// <summary>
        /// Gets or sets the base64 representation of the management certificate public key.
        /// </summary>
        [DataMember(Order = 0)]
        public string SubscriptionCertificatePublicKey { get; set; }

        /// <summary>
        /// Gets or sets the thumb print that uniquely identifies the management certificate.
        /// </summary>
        [DataMember(Order = 1)]
        public string SubscriptionCertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets the certificate’s raw data in base-64 encoded .cer format.
        /// </summary>
        [DataMember(Order = 2)]
        public string SubscriptionCertificateData { get; set; }

        /// <summary>
        /// Gets or sets the time that the management certificate was created, in UTC. 
        /// </summary>
        [DataMember(Order = 3)]
        public DateTime TimeCreated { get; set; }
    }
}
