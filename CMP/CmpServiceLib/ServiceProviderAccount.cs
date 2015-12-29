using System;
using System.Collections.Generic;
using AzureAdminClientLib;

namespace CmpServiceLib.Models
{
    public partial class ServiceProviderAccount : IComparable<ServiceProviderAccount>
    {
        public ServiceProviderAccount()
        { }

        public ServiceProviderAccount(CmpInterfaceModel.Models.ServiceProviderAccountSpec sPa)
        {
            ID = sPa.ID;
            AccountID = sPa.AccountID;
            AccountPassword = sPa.AccountPassword;
            AccountType = sPa.AccountType;
            Active = sPa.Active;
            AzAffinityGroup = sPa.AzAffinityGroup;
            AzRegion = sPa.AzRegion;
            AzStorageContainerUrl = sPa.AzStorageContainerUrl;
            AzSubnet = sPa.AzSubnet;
            AzVNet = sPa.AzVNet;
            CertificateBlob = sPa.CertificateBlob;
            CertificateThumbprint = sPa.CertificateThumbprint;
            Config = sPa.Config;
            CoreCountCurrent = sPa.CoreCountCurrent;
            CoreCountMax = sPa.CoreCountMax;
            Description = sPa.Description;
            ExpirationDate = sPa.ExpirationDate;
            Name = sPa.Name;
            OwnerNamesCSV = sPa.OwnerNamesCSV;
            ResourceGroup = sPa.ResourceGroup;
            StorageAccountCountCurrent = sPa.StorageAccountCountCurrent;
            StorageAccountCountMax = sPa.StorageAccountCountMax;
            TagData = sPa.TagData;
            TagID = sPa.TagID;
            VmsPerServiceCountMax = sPa.VmsPerServiceCountMax;
            VmsPerVnetCountMax = sPa.VmsPerVnetCountMax;
            VnetCountCurrent = sPa.VnetCountCurrent;
            VnetCountMax = sPa.VnetCountMax;
            AzureADTenantId = sPa.AzureADTenantId;
            AzureADClientId = sPa.AzureADClientId;
            AzureADClientKey = sPa.AzureADClientKey;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// This instance method will call the AzureClientLibrary and load properties from Azure to
        /// the ServiceProviderAccount class. Only the top level subscription properties
        /// are available via this method.
        /// </summary>
        /// 
        //*********************************************************************

        public void LoadAzureProperties()
        {
            var currentSubscription = new AzureSubscription();
            currentSubscription.LoadOnlyStaticValues(this.AccountID, this.CertificateThumbprint);

            this.CoreCountCurrent = (int) currentSubscription.CurrentCoreCount;
            this.CoreCountMax = (int) currentSubscription.MaxCoreCount;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProviderAccountParameter"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public int CompareTo(ServiceProviderAccount serviceProviderAccountParameter)
        {
            // The comparison is the percentage of available cores between the two objects
            double percentageAvailableForThisObject = 0;
            double percentageAvailableForTheParameterObject = 0;

            if (this.CoreCountMax != null && this.CoreCountMax != 0 && this.CoreCountCurrent != null)
            {
                percentageAvailableForThisObject = 1.0 - ((int)this.CoreCountCurrent / (double)this.CoreCountMax);
            }

            if (serviceProviderAccountParameter.CoreCountMax != null && serviceProviderAccountParameter.CoreCountMax != 0 && serviceProviderAccountParameter.CoreCountCurrent != null)
            {
                percentageAvailableForTheParameterObject = 1.0 - ((int)serviceProviderAccountParameter.CoreCountCurrent / (double)serviceProviderAccountParameter.CoreCountMax);
            }

            return percentageAvailableForThisObject.CompareTo(percentageAvailableForTheParameterObject);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public List<AzureStorageAccount> FetchStorageAccountList()
        {
            var currentSubscription = new AzureSubscription();
            currentSubscription.LoadStorageAccounts(this.AccountID, this.CertificateThumbprint);

            return currentSubscription.StorageAccounts;
        }
    }
}
