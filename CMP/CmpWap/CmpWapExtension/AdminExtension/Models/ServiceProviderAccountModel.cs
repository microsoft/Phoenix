using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models
{
    public class ServiceProviderAccountModel
    {

        //*********************************************************************
        ///
        /// <summary>
        /// Initializes a new instance of the <see cref="AppModel" /> class.
        /// </summary>
        /// 
        //*********************************************************************

        public ServiceProviderAccountModel()
        {
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Initializes a new instance of the <see cref="AppModel" /> class.
        /// </summary>
        /// <param name="createSPAM">The domain name from API.</param>
        /// 
        //*********************************************************************

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
            this.TenantID = TenantID;
            this.ClientID = ClientID;
            this.ClientKey = ClientKey;
            this.PlanId = PlanId;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Covert to the API object.
        /// </summary>
        /// <returns>The API DomainName data contract.</returns>
        /// 
        //*********************************************************************

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
                TagID = this.TagID,
                TenantID = this.TenantID,
                ClientID = this.ClientID,
                ClientKey = this.ClientKey,
                PlanId  = this.PlanId
            };
        }

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
        public string TenantID { get; set; }
        public string ClientID { get; set; }
        public string ClientKey { get; set; }
        public string PlanId { get; set; }
    }
}
