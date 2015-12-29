//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Data model for service provider accounts
    /// </summary>    
    public class ServiceProviderAccountModel
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderAccount" /> class.
        /// </summary>
        public ServiceProviderAccountModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderAccount" /> class.
        /// </summary>
        /// <param name="createSPAM">The account from API.</param>
        public ServiceProviderAccountModel(ServiceProviderAccount createSPAM)
        {
            this.ID = createSPAM.ID;
            this.AccountID = createSPAM.AccountID;
            this.AccountPassword = createSPAM.AccountPassword;
            this.AccountType = createSPAM.AccountType;
            this.Active = createSPAM.Active;
            this.AzAffinityGroup = createSPAM.AzAffinityGroup;
            this.AzRegion = createSPAM.AzRegion;
            this.AzStorageContainerUrl = createSPAM.AzStorageContainerUrl;
            this.AzSubnet = createSPAM.AzSubnet;
            this.AzVNet = createSPAM.AzVNet;
            this.CertificateBlob = createSPAM.CertificateBlob;
            this.CertificateThumbprint = createSPAM.CertificateThumbprint;
            this.Config = createSPAM.Config;
            this.CoreCountCurrent = createSPAM.CoreCountCurrent;
            this.CoreCountMax = createSPAM.CoreCountMax;
            this.Description = createSPAM.Description;
            this.ExpirationDate = createSPAM.ExpirationDate;
            this.Name = createSPAM.Name;
            this.OwnerNamesCSV = createSPAM.OwnerNamesCSV;
            this.ResourceGroup = createSPAM.ResourceGroup;
            this.TagData = createSPAM.TagData;
            this.TagID = createSPAM.TagID;
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API account data contract.</returns>
        public ServiceProviderAccount ToApiObject()
        {
            return new ServiceProviderAccount()
            {
                ID = this.ID, 
                AccountID = this.AccountID, 
                AccountPassword = this.AccountPassword, 
                AccountType = this.AccountType, 
                Active = this.Active, 
                AzAffinityGroup = this.AzAffinityGroup, 
                AzRegion = this.AzRegion, 
                AzStorageContainerUrl = this.AzStorageContainerUrl, 
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
                TagData = this.TagData, 
                TagID = this.TagID
            };
        }

        /// <summary>
        /// The ID of the account
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The name of the account
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the account
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The owner names in CSV format
        /// </summary>
        public string OwnerNamesCSV { get; set; }

        /// <summary>
        /// The configuration of the account
        /// </summary>
        public string Config { get; set; }

        /// <summary>
        /// Tag data associated with the account
        /// </summary>
        public string TagData { get; set; }

        /// <summary>
        /// The tag ID of the account
        /// </summary>
        public int TagID { get; set; }

        /// <summary>
        /// The type of the account
        /// todo: Rename this
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        /// Expiration date of the account
        /// </summary>
        public System.DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Blob representing the account's certificate
        /// </summary>
        public string CertificateBlob { get; set; }

        /// <summary>
        /// Thumbprint of the account's certificate
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        /// ID of the account
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// Password of the account
        /// </summary>
        public string AccountPassword { get; set; }

        /// <summary>
        /// Whether or not the account is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// The Azure region of the account
        /// todo: Rename
        /// </summary>
        public string AzRegion { get; set; }

        /// <summary>
        /// The Azure affinity group for the account
        /// todo: Rename
        /// </summary>
        public string AzAffinityGroup { get; set; }

        /// <summary>
        /// The virtual network of the account in Azure
        /// todo: Rename
        /// </summary>
        public string AzVNet { get; set; }

        /// <summary>
        /// The subnet of the account in Azure
        /// todo: Rename
        /// </summary>
        public string AzSubnet { get; set; }

        /// <summary>
        /// The URL for the account's storage container in Azure
        /// todo: Rename
        /// </summary>
        public string AzStorageContainerUrl { get; set; }

        /// <summary>
        /// The resource group for the account in Azure
        /// </summary>
        public string ResourceGroup { get; set; }

        /// <summary>
        /// Maximum number of cores supported in the account
        /// </summary>
        public int CoreCountMax { get; set; }

        /// <summary>
        /// Current number of cores in the account
        /// </summary>
        public int CoreCountCurrent { get; set; }
    }
}