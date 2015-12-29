namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Interfaces
{
    interface IAzureSubscription : IPlanOption
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string AccountType { get; set; }
        System.DateTime ? ExpirationDate { get; set; }
        string CertificateBlob { get; set; }
        string CertificateThumbprint { get; set; }
        string AccountID { get; set; }
        string AccountPassword { get; set; }
        bool ? Active { get; set; }
        string TenantID { get; set; }
        string ClientID { get; set; }
        string ClientKey { get; set; }
        string PlanId { get; set; }
    }
}
