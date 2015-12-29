// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.Azure.Portal.Configuration;
using Microsoft.WindowsAzurePack.Samples.DataContracts;
using Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using Microsoft.WindowsAzurePack.CmpWapExtension.Common;
using Microsoft.WindowsAzurePack.Samples;
using System.Collections.Generic;
using PlanConfiguration = Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.PlanConfiguration;


namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Controllers
{
    //*********************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*********************************************************************

    [RequireHttps]
    [OutputCache(Location = OutputCacheLocation.None)]
    [PortalExceptionHandler]
    public sealed class CmpWapExtensionAdminController : ExtensionController
    {   
        private static readonly string AdminApiUri = OnPremPortalConfiguration.Instance.RdfeAdminUri;
        //This model is used to show registered resource provider information
        public EndpointModel CmpWapExtensionServiceEndPoint { get; set; }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets the admin settings.
        /// </summary>
        /// 
        //*********************************************************************

        [HttpPost]
        [ActionName("AdminSettings")]
        public async Task<JsonResult> GetAdminSettings()
        {
            try
            {
                var resourceProvider = 
                    await ClientFactory.AdminManagementClient.GetResourceProviderAsync
                        (CmpWapExtensionClient.RegisteredServiceName, Guid.Empty.ToString());

                this.CmpWapExtensionServiceEndPoint = 
                    EndpointModel.FromResourceProviderEndpoint(resourceProvider.AdminEndpoint);
                
                return this.JsonDataSet(this.CmpWapExtensionServiceEndPoint);
            }
            catch (ManagementClientException managementException)
            {
                // 404 means the CmpWapExtension resource provider is not yet configured, return an empty record.
                if (managementException.StatusCode == HttpStatusCode.NotFound)
                    return this.JsonDataSet(new EndpointModel());

                //Just throw if there is any other type of exception is encountered
                throw;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Update admin settings => Register Resource Provider
        /// </summary>
        /// <param name="newSettings">The new settings.</param>
        /// 
        //*********************************************************************

        [HttpPost]
        [ActionName("UpdateAdminSettings")]
        public async Task<JsonResult> UpdateAdminSettings(EndpointModel newSettings)
        {
            this.ValidateInput(newSettings);

            ResourceProvider cmpWapExtensionResourceProvider;
            var errorMessage = string.Empty;

            try
            {
                //***********************************************************

                const string rsn = CmpWapExtensionClient.RegisteredServiceName;
                var es = Guid.Empty.ToString();

                var amc = ClientFactory.AdminManagementClient;

                cmpWapExtensionResourceProvider = 
                    await amc.GetResourceProviderAsync(rsn, es);

                //***********************************************************

                //Check if resource provider is already registered or not
                /*CmpWapExtensionResourceProvider =
                    await
                        ClientFactory.AdminManagementClient.GetResourceProviderAsync(
                            CmpWapExtensionClient.RegisteredServiceName, Guid.Empty.ToString());*/
            }
            catch (ManagementClientException exception)
            {
                // 404 means the CmpWapExtension resource provider is not yet configured, return an empty record.
                if (exception.StatusCode == HttpStatusCode.NotFound)
                {
                    cmpWapExtensionResourceProvider = null;
                }
                else
                {
                    //Just throw if there is any other type of exception is encountered
                    throw;
                }
            }
            catch (Exception ex)
            {
                var XX = ex.Message;
                throw;
            }

            if (cmpWapExtensionResourceProvider != null)
            {
                //Resource provider already registered so lets update endpoint
                cmpWapExtensionResourceProvider.AdminEndpoint = newSettings.ToAdminEndpoint();
                cmpWapExtensionResourceProvider.TenantEndpoint = newSettings.ToTenantEndpoint();
                cmpWapExtensionResourceProvider.NotificationEndpoint = newSettings.ToNotificationEndpoint();
                cmpWapExtensionResourceProvider.UsageEndpoint = newSettings.ToUsageEndpoint();
            }
            else
            {
                //Resource provider not registered yet so lets register new one now
                cmpWapExtensionResourceProvider = new ResourceProvider()
                {
                    Name = CmpWapExtensionClient.RegisteredServiceName,
                    DisplayName = "Cmp Wap Extension",
                    InstanceDisplayName = CmpWapExtensionClient.RegisteredServiceName + " Instance",
                    Enabled = true,
                    PassThroughEnabled = true,
                    AllowAnonymousAccess = false,
                    AdminEndpoint = newSettings.ToAdminEndpoint(),
                    TenantEndpoint = newSettings.ToTenantEndpoint(),
                    NotificationEndpoint = newSettings.ToNotificationEndpoint(),
                    UsageEndpoint = newSettings.ToUsageEndpoint()
                };
            }

            var testList = new ResourceProviderVerificationTestList()
            {
                new ResourceProviderVerificationTest()
                {
                    TestUri = new Uri(CmpWapExtensionAdminController.AdminApiUri + 
                        CmpWapExtensionClient.AdminSettings),
                    IsAdmin = true
                }
            };
            try
            {
                // Resource Provider Verification to ensure given endpoint and username/password is correct
                // Only validate the admin RP since we don't have a tenant subscription to do it.
                var result = await ClientFactory.AdminManagementClient.VerifyResourceProviderAsync(
                    cmpWapExtensionResourceProvider, testList);

                if (result.HasFailures)
                    throw new HttpException("Invalid endpoint or bad username/password");
            }
            catch (ManagementClientException ex)
            {
                throw new HttpException("Invalid endpoint or bad username/password " + ex.Message.ToString());
            }

            //Finally Create Or Update resource provider
            var rpTask = (string.IsNullOrEmpty(cmpWapExtensionResourceProvider.Name) || 
                String.IsNullOrEmpty(cmpWapExtensionResourceProvider.InstanceId))
                    ? ClientFactory.AdminManagementClient.CreateResourceProviderAsync(cmpWapExtensionResourceProvider)
                        : ClientFactory.AdminManagementClient.UpdateResourceProviderAsync(cmpWapExtensionResourceProvider.Name, 
                        cmpWapExtensionResourceProvider.InstanceId, cmpWapExtensionResourceProvider);

            try
            {
                await rpTask;
            }
            catch (ManagementClientException e)
            {
                throw;
            }

            var adminSettings = new AdminSettings()
            {
                EndpointAddress = newSettings.EndpointAddress,
                Username = newSettings.Username,
                Password = newSettings.Password
            };

            await ClientFactory.CmpWapExtensionClient.UpdateAdminSettingsAsync(adminSettings);

            return this.Json(newSettings);
        }


        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spam"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        [HttpPost]
        [ActionName("UpdateServiceProviderAccount")]
        public async Task<JsonResult> UpdateServiceProviderAccount(ServiceProviderAccountModel spam)
        {
            var sPAs = new List<ServiceProviderAccountModel>();

            var domainsFromApi = await ClientFactory.CmpWapExtensionClient.UpdateServProvAcctAsync(spam.ToApiObject());
            sPAs.AddRange(domainsFromApi.Select(d => new ServiceProviderAccountModel(d)));

            return this.JsonDataSet(sPAs, namePropertyName: "Name");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newSettings"></param>
        /// 
        //*********************************************************************

        private void ValidateInput(EndpointModel newSettings)
        {
            if (newSettings == null)
            {
                throw new ArgumentNullException("newSettings");
            }

            if (String.IsNullOrEmpty(newSettings.EndpointAddress))
            {
                throw new ArgumentNullException("EndpointAddress");
            }

            if (String.IsNullOrEmpty(newSettings.Username))
            {
                throw new ArgumentNullException("Username");
            }

            if (String.IsNullOrEmpty(newSettings.Password))
            {
                throw new ArgumentNullException("Password");
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// List VM Requests that belong to the subscription
        /// </summary>
        /// <param name="subscriptionIds"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        [HttpPost]
        [ActionName("Vms")]
        public async Task<JsonResult> ListVMs(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            //var vMs = new List<Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models.CreateVmModel>();
            var vMs = new List<Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.CreateVmModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                //throw new HttpException("Subscription Id not found");
                subscriptionIds = new string[] {null};
            }

            try
            {
                foreach (var subId in subscriptionIds)
                {
                    var vMsFromApi = await ClientFactory.CmpWapExtensionClient.ListVmsAsync(subId);
                    vMs.AddRange(vMsFromApi.Select(d => 
                        new Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.CreateVmModel(d)));
                }
            }
            catch (Exception ex)
            {
                var g = ex.Message;
                throw;
            }

            return this.JsonDataSet(vMs, namePropertyName: "Name");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionIds"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        [HttpPost]
        [ActionName("Domains")]
        public async Task<JsonResult> ListDomains(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            //var domains = new List<Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models.CreateDomainModel>();
            var domains = new List<Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.CreateDomainModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                //throw new HttpException("Subscription Id not found");
                subscriptionIds = new string[] { null };
            }

            foreach (var subId in subscriptionIds)
            {
                var domainsFromApi = await ClientFactory.CmpWapExtensionClient.ListDomainsAsync(subId);
                domains.AddRange(domainsFromApi.Select(d => 
                    new Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.CreateDomainModel(d)));
            }

            return this.JsonDataSet(domains, namePropertyName: "Name");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionIds"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        [HttpPost]
        [ActionName("Oss")]
        public async Task<JsonResult> ListOSs(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            //var domains = new List<Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models.CreateOsModel>();
            var domains = new List<Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.CreateOsModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                //throw new HttpException("Subscription Id not found");
                subscriptionIds = new string[] { null };
            }

            foreach (var subId in subscriptionIds)
            {
                var domainsFromApi = await ClientFactory.CmpWapExtensionClient.ListOSsAsync(subId);
                domains.AddRange(domainsFromApi.Select(d => new Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.CreateOsModel(d)));
            }

            return this.JsonDataSet(domains, namePropertyName: "Name");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionIds"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        [HttpPost]
        [ActionName("Vmsizes")]
        public async Task<JsonResult> ListVmSizes(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            //var domains = new List<Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models.CreateSizeModel>();
            var domains = new List<Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.CreateSizeModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                //throw new HttpException("Subscription Id not found");
                subscriptionIds = new string[] { null };
            }

            foreach (var subId in subscriptionIds)
            {
                var domainsFromApi = await ClientFactory.CmpWapExtensionClient.ListVmSizesAsync(subId);
                domains.AddRange(domainsFromApi.Select(d => new Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.CreateSizeModel(d)));
            }

            return this.JsonDataSet(domains, namePropertyName: "Name");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionIds"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        [HttpPost]
        [ActionName("Regions")]
        public async Task<JsonResult> ListTargetRegions(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            //var domains = new List<Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models.CreateRegionModel>();
            var domains = new List<Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.CreateRegionModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                //throw new HttpException("Subscription Id not found");
                subscriptionIds = new string[] { null };
            }

            foreach (var subId in subscriptionIds)
            {
                var domainsFromApi = await ClientFactory.CmpWapExtensionClient.ListTargetRegionsAsync(subId);
                domains.AddRange(domainsFromApi.Select(d => new Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.CreateRegionModel(d)));
            }

            return this.JsonDataSet(domains, namePropertyName: "Name");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionIds"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        [HttpPost]
        [ActionName("Apps")]
        public async Task<JsonResult> ListApps(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            //var domains = new List<Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models.CreateRegionModel>();
            var domains = new List<Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.AppModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                //throw new HttpException("Subscription Id not found");
                subscriptionIds = new string[] { null };
            }

            foreach (var subId in subscriptionIds)
            {
                var domainsFromApi = await ClientFactory.CmpWapExtensionClient.ListAppsAsync(subId);
                domains.AddRange(domainsFromApi.Select(d => new Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.AppModel(d)));
            }

            return this.JsonDataSet(domains, namePropertyName: "Name");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionIds"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        [HttpPost]
        [ActionName("ServProvAccts")]
        public async Task<JsonResult> ListServProdAccts(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var domains = new List<Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.ServiceProviderAccountModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                //throw new HttpException("Subscription Id not found");
                subscriptionIds = new string[] { null };
            }

            foreach (var subId in subscriptionIds)
            {
                var domainsFromApi = await ClientFactory.CmpWapExtensionClient.ListServProvAcctsAsync(subId);
                domains.AddRange(domainsFromApi.Select(d => new Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.ServiceProviderAccountModel(d)));
            }

            return this.JsonDataSet(domains, namePropertyName: "Name");
        }

        [HttpPost]
        [ActionName("ResourceGroups")]
        public async Task<JsonResult> ListResourceGroups(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var domains = new List<Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.ResourceGroupModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                //throw new HttpException("Subscription Id not found");
                subscriptionIds = new string[] { null };
            }

            foreach (var subId in subscriptionIds)
            {
                var domainsFromApi = await ClientFactory.CmpWapExtensionClient.ListResourceGroupsAsync(subId);
                domains.AddRange(domainsFromApi.Select(d => new Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.ResourceGroupModel(d)));
            }

            return this.JsonDataSet(domains, namePropertyName: "Name");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="vmToCreate"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        [HttpPost]
        [ActionName("CreateVm")]
        public async Task<JsonResult> CreateVm(string subscriptionId,
            Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.CreateVmModel vmToCreate)
        {
            await ClientFactory.CmpWapExtensionClient.CreateVmAsync(subscriptionId, vmToCreate.ToApiObject());
            return this.Json(vmToCreate);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="vmOp"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        [HttpPost]
        [ActionName("Vmop")]
        public async Task<JsonResult> VmOp(string subscriptionId, 
            Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.VmOpModel vmOp)
        {
            await ClientFactory.CmpWapExtensionClient.VmOpAsync(subscriptionId, vmOp.ToApiObject());
            return this.Json(vmOp);
        }

        [HttpPost]
        [ActionName("PlanConfiguration")]
        public async Task<JsonResult> GetPlanConfiguration(string planId)
        {
            var configuration = await ClientFactory.CmpWapExtensionClient.GetPlanConfigurationAsync(planId);
            return Json(configuration);
        }

        [HttpPost]
        [ActionName("SetPlanConfiguration")]
        public async Task<JsonResult> SetPlanConfiguration(string planId, PlanConfiguration configuration)
        {
            await ClientFactory.CmpWapExtensionClient.SetPlanConfigurationAsync(planId, configuration.ToApiObject());
            return Json(configuration);
        }
    }
}