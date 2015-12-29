using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class ServiceProviderAccount
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerNamesCSV { get; set; }
        public string Config { get; set; }
        public string TagData { get; set; }
        public Nullable<int> TagID { get; set; }
        public string AccountType { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public string CertificateBlob { get; set; }
        public string CertificateThumbprint { get; set; }
        public string AccountID { get; set; }
        public string AccountPassword { get; set; }
        public Nullable<bool> Active { get; set; }
        public string AzRegion { get; set; }
        public string AzAffinityGroup { get; set; }
        public string AzVNet { get; set; }
        public string AzSubnet { get; set; }
        public string AzStorageContainerUrl { get; set; }
        public string ResourceGroup { get; set; }
        public Nullable<int> CoreCountMax { get; set; }
        public Nullable<int> CoreCountCurrent { get; set; }
        public Nullable<int> VnetCountMax { get; set; }
        public Nullable<int> VnetCountCurrent { get; set; }
        public Nullable<int> StorageAccountCountMax { get; set; }
        public Nullable<int> StorageAccountCountCurrent { get; set; }
        public Nullable<int> VmsPerVnetCountMax { get; set; }
        public Nullable<int> VmsPerServiceCountMax { get; set; }
        public string AzureADTenantId { get; set; }
        public string AzureADClientId { get; set; }
        public string AzureADClientKey { get; set; }
    }
}
