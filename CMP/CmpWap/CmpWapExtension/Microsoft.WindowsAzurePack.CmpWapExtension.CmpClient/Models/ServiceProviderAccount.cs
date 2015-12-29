using System;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    //*********************************************************************
    ///
    /// <summary>
    /// Class for a Service Provider Account
    /// </summary>
    /// 
    //*********************************************************************
    public class ServiceProviderAccount
    {
        public ServiceProviderAccount()
        {

        }
        /// <summary>
        /// Initializes a new Service Provider Account
        /// </summary>
        /// <param name="spa">The service account to initialize</param>
        public ServiceProviderAccount(CmpServiceLib.Models.ServiceProviderAccount spa)
        {
            if (null == spa.Active)
                spa.Active = false;

            if (null == spa.CoreCountCurrent)
                spa.CoreCountCurrent = 0;

            if (null == spa.CoreCountMax)
                spa.CoreCountMax = 0;

            if (null == spa.ExpirationDate)
                spa.ExpirationDate = DateTime.Now.AddYears(1);

            ID = spa.ID;
            AccountID = spa.AccountID;
            AccountPassword = spa.AccountPassword;
            AccountType = spa.AccountType;
            Active = (bool)spa.Active;
            AzAffinityGroup = spa.AzAffinityGroup;
            AzRegion = spa.AzRegion;
            AzStorageContainerUrl = spa.AzStorageContainerUrl;
            AzSubnet = spa.AzSubnet;
            AzVNet = spa.AzVNet;
            CertificateBlob = spa.CertificateBlob;
            CertificateThumbprint = spa.CertificateThumbprint;
            Config = spa.Config;
            CoreCountCurrent = (int)spa.CoreCountCurrent;
            CoreCountMax = (int)spa.CoreCountMax;
            Description = spa.Description;
            ExpirationDate = (DateTime)spa.ExpirationDate;
            Name = spa.Name;
            OwnerNamesCSV = spa.OwnerNamesCSV;
            ResourceGroup = spa.ResourceGroup;
            TagData = spa.TagData;
            AzureADTenantId = spa.AzureADTenantId;
            AzureADClientId = spa.AzureADClientId;
            AzureADClientKey = spa.AzureADClientKey;
        }

        public CmpServiceLib.Models.ServiceProviderAccount GetModel()
        {
            return new CmpServiceLib.Models.ServiceProviderAccount()
            {
                AccountID = AccountID, 
                ID = this.ID, 
                AccountPassword = this.AccountPassword, 
                AccountType = this.AccountType, 
                Active = this.Active, 
                AzAffinityGroup = this.AzAffinityGroup, 
                AzRegion = this.AzRegion, 
                AzStorageContainerUrl = AzStorageContainerUrl, 
                AzSubnet = this.AzSubnet, 
                AzVNet = this.AzVNet, 
                CertificateBlob = this.CertificateBlob, 
                CertificateThumbprint = this.CertificateThumbprint, 
                Config = this.Config, 
                CoreCountCurrent = this.CoreCountCurrent, 
                CoreCountMax = this.CoreCountMax, 
                Description = this.Description, 
                ExpirationDate = this.ExpirationDate, 
                Name = this.Name, 
                OwnerNamesCSV = this.OwnerNamesCSV, 
                ResourceGroup = this.ResourceGroup, 
                StorageAccountCountCurrent = 0, 
                StorageAccountCountMax = 0, 
                TagData = this.TagData, 
                TagID = this.TagID, 
                VmsPerServiceCountMax = 0, 
                VmsPerVnetCountMax = 0, 
                VnetCountCurrent = 0, 
                VnetCountMax = 0,
                AzureADTenantId = this.AzureADTenantId,
                AzureADClientId = this.AzureADClientId,
                AzureADClientKey = this.AzureADClientKey
            };
        }

        /// <summary>
        /// Service Account ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Name of the Service Account
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the Service Account
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// CSV containing the Service Account owners
        /// </summary>
        public string OwnerNamesCSV { get; set; }

        /// <summary>
        /// XML configuration for the Service Account
        /// </summary>
        public string Config { get; set; }

        /// <summary>
        /// Tag data for the Service Account
        /// </summary>
        public string TagData { get; set; }

        /// <summary>
        /// ID for the Tag data of the Service Account
        /// </summary>
        public int TagID { get; set; }

        /// <summary>
        /// Type of the Service Account
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        /// Expiration date of the Service Account
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Certificate blob for the Service Account
        /// </summary>
        public string CertificateBlob { get; set; }

        /// <summary>
        /// Thumbprint for the certificate blob of the Service Account
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        /// ID for the Service Account
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// Password for the Service Account
        /// </summary>
        public string AccountPassword { get; set; }

        /// <summary>
        /// Flag on whether the Service Account is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Azure region of the Service Account
        /// </summary>
        public string AzRegion { get; set; }

        /// <summary>
        /// Azure affinity group of the Service Account
        /// </summary>
        public string AzAffinityGroup { get; set; }

        /// <summary>
        /// Azure virtual network of the Service Account
        /// </summary>
        public string AzVNet { get; set; }

        /// <summary>
        /// Azure subnet of the Service Account
        /// </summary>
        public string AzSubnet { get; set; }
        /// <summary>
        /// Url of the Azure storage container for the Service Account
        /// </summary>
        public string AzStorageContainerUrl { get; set; }

        /// <summary>
        /// Resource Group of the Service Account
        /// </summary>
        public string ResourceGroup { get; set; }

        /// <summary>
        /// Maximum core count for the Service Account
        /// </summary>
        public int CoreCountMax { get; set; }

        /// <summary>
        /// Current core count for the Service Account
        /// </summary>
        public int CoreCountCurrent { get; set; }

        public string AzureADClientId { get; set; }
        public string AzureADTenantId { get; set; }
        public string AzureADClientKey { get; set; }

    }
}
