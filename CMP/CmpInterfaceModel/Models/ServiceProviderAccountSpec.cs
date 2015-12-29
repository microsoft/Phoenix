using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmpInterfaceModel.Models
{
    public class ServiceProviderAccountSpec
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerNamesCSV { get; set; }
        public string Config { get; set; }
        public string TagData { get; set; }
        public int TagID { get; set; }
        public string AccountType { get; set; }
        public System.DateTime ExpirationDate { get; set; }
        public string CertificateBlob { get; set; }
        public string CertificateThumbprint { get; set; }
        public string AccountID { get; set; }
        public string AccountPassword { get; set; }
        public bool Active { get; set; }
        public string AzRegion { get; set; }
        public string AzAffinityGroup { get; set; }
        public string AzVNet { get; set; }
        public string AzSubnet { get; set; }
        public string AzStorageContainerUrl { get; set; }
        public string ResourceGroup { get; set; }
        public int CoreCountMax { get; set; }
        public int CoreCountCurrent { get; set; }
        public int VnetCountMax { get; set; }
        public int VnetCountCurrent { get; set; }
        public int StorageAccountCountMax { get; set; }
        public int StorageAccountCountCurrent { get; set; }
        public int VmsPerVnetCountMax { get; set; }
        public int VmsPerServiceCountMax { get; set; }
        public string AzureADTenantId { get; set; }
        public string AzureADClientId { get; set; }
        public string AzureADClientKey { get; set; }
    }
}
