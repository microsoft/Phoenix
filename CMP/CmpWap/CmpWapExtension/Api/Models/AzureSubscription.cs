using System;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Interfaces;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public class AzureSubscription : IAzureSubscription
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AccountType { get; set; }
        public DateTime ? ExpirationDate { get; set; }
        public string CertificateBlob { get; set; }
        public string CertificateThumbprint { get; set; }
        public string AccountID { get; set; }
        public string AccountPassword { get; set; }
        public bool ? Active { get; set; }
        public string TenantID { get; set; }
        public string ClientID { get; set; }
        public string ClientKey { get; set; }
        public string PlanId { get; set; }
    }
}