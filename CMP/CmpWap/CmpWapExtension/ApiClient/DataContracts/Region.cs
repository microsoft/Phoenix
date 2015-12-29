// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
    /// <summary>
    /// This is a data contract class between extensions and resource provider
    /// CreateVM contains data contract of data which shows up in "VMs" tab inside Cmp Tenant Extension
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
    public class Region
    {
        /// <summary>
        /// Name of the file share
        /// </summary>
        [DataMember(Order = 1)]
        public int AzureRegionId { get; set; }

        /// <summary>
        /// Name of the file share
        /// </summary>
        [DataMember(Order = 2)]
        public string Name { get; set; }

        /// <summary>
        /// SubscriptionId of user who created this file share
        /// </summary>
        [DataMember(Order = 3)]
        public string Description { get; set; }

        /// <summary>
        /// SubscriptionId of user who created this file share
        /// </summary>
        [DataMember(Order = 4)]
        public string OsImageContainer { get; set; }

        /// <summary>
        /// SubscriptionId of user who created this file share
        /// </summary>
        [DataMember(Order = 5)]
        public bool IsActive { get; set; }

        ///// <summary>
        ///// SubscriptionId of user who created this file share
        ///// </summary>
        //[DataMember(Order = 6)]
        //public string TagData { get; set; }

        ///// <summary>
        ///// SubscriptionId of user who created this file share
        ///// </summary>
        //[DataMember(Order = 7)]
        //public bool IsActive { get; set; }

        [DataMember(Order = 6)]
        public System.DateTime CreatedOn { get; set; }

        [DataMember(Order = 7)]
        public string CreatedBy { get; set; }

        [DataMember(Order = 8)]
        public System.DateTime LastUpdatedOn { get; set; }

        [DataMember(Order = 9)]
        public string LastUpdatedBy { get; set; }
    }
}
