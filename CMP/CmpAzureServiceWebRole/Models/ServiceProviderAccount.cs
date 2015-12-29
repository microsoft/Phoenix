using System;

namespace CmpAzureServiceWebRole.Models
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
    }
}
