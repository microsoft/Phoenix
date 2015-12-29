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
    public class ServiceProviderAccount
    {
        /// <summary> </summary>
        [DataMember(Order = 1)]
        public int ID { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 2)]
        public string Name { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 3)]
        public string Description { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 4)]
        public string OwnerNamesCSV { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 5)]
        public string Config { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 6)]
        public string TagData { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 7)]
        public int TagID { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 8)]
        public string AccountType { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 9)]
        public System.DateTime ExpirationDate { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 10)]
        public string CertificateBlob { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 11)]
        public string CertificateThumbprint { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 12)]
        public string AccountID { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 13)]
        public string AccountPassword { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 14)]
        public bool Active { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 15)]
        public string AzRegion { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 16)]
        public string AzAffinityGroup { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 17)]
        public string AzVNet { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 18)]
        public string AzSubnet { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 19)]
        public string AzStorageContainerUrl { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 20)]
        public string ResourceGroup { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 21)]
        public int CoreCountMax { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 22)]
        public int CoreCountCurrent { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 23)]
        public string TenantID { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 24)]
        public string ClientID { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 25)]
        public string ClientKey { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 26)]
        public string PlanId { get; set; }
    }
}
