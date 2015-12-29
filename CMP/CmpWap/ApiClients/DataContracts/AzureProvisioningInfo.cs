//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Azure subscription provisioning info.
    /// </summary>
    [DataContract(Namespace = "http://www.microsoft.com/Azure/ProvisioningAgent/1.0")]
    public class AzureProvisioningInfo : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the account admin live email id.
        /// </summary>
        /// <value>
        /// The account admin live email id.
        /// </value>
        [DataMember(Order = 0)]
        public string AccountAdminLiveEmailId { get; set; }

        /// <summary>
        /// Gets or sets the account admin live puid.
        /// </summary>
        /// <value>
        /// The account admin live puid.
        /// </value>
        [DataMember(Order = 1)]
        public string AccountAdminLivePuid { get; set; }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        /// <value>
        /// The account id.
        /// </value>
        [DataMember(Order = 2)]
        public Guid AccountId { get; set; }

        /// <summary>
        /// Gets or sets the name of the friendly.
        /// </summary>
        /// <value>
        /// The name of the friendly.
        /// </value>
        [DataMember(Order = 3)]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the offer category.
        /// </summary>
        /// <value>
        /// The offer category.
        /// </value>
        [DataMember(Order = 4)]
        public string OfferCategory { get; set; }

        /// <summary>
        /// Gets or sets the offer info.
        /// </summary>
        /// <value>
        /// The offer info.
        /// </value>
        [DataMember(Order = 5)]
        public string OfferInfo { get; set; }

        /// <summary>
        /// Gets or sets the type of the offer.
        /// </summary>
        /// <value>
        /// The type of the offer.
        /// </value>
        [DataMember(Order = 6)]
        public AzureOfferType OfferType { get; set; }

        /// <summary>
        /// Gets or sets the reason code.
        /// </summary>
        /// <value>
        /// The reason code.
        /// </value>
        [DataMember(Order = 7)]
        public string ReasonCode { get; set; }

        /// <summary>
        /// Gets or sets the service admin live email id.
        /// </summary>
        /// <value>
        /// The service admin live email id.
        /// </value>
        [DataMember(Order = 8)]
        public string ServiceAdminLiveEmailId { get; set; }

        /// <summary>
        /// Gets or sets the service admin live puid.
        /// </summary>
        /// <value>
        /// The service admin live puid.
        /// </value>
        [DataMember(Order = 9)]
        public string ServiceAdminLivePuid { get; set; }

        /// <summary>
        /// Gets or sets the subscription id.
        /// </summary>
        /// <value>
        /// The subscription id.
        /// </value>
        [DataMember(Order = 10, IsRequired = true)]
        public Guid SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [DataMember(Order = 11)]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the plan id.
        /// </summary>
        [DataMember(Order = 12)]
        public string PlanId { get; set; }

        /// <summary>
        /// Gets or sets the co-admin names.
        /// </summary>
        [DataMember(Order = 13)]
        public IList<string> CoAdminNames { get; set; }

        /// <summary>
        /// Gets or sets the structure that contains extra data.
        /// </summary>
        /// <returns>An <see cref="T:System.Runtime.Serialization.ExtensionDataObject"/> that contains data that is not recognized as belonging to the data contract.</returns>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
