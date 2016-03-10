// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient
{
    //*********************************************************************
    ///
    /// <summary>
    /// This is client of CmpWapExtension Resource Provider 
    /// This client is used by admin and tenant extensions to make call to 
    /// CmpWapExtension Resource Provider
    /// </summary>
    /// 
    //*********************************************************************
    
    public class CmpWapExtensionClient
    {        
        public const string RegisteredServiceName = "CmpWapExtension";
        public const string RegisteredPath = "services/" + RegisteredServiceName;
        public const string AdminSettings = RegisteredPath + "/settings";
        public const string AdminProducts = RegisteredPath + "/products";
        public const string AdminFileServers = RegisteredPath + "/fileservers";
        private const string PlanConfigurations = RegisteredPath + "/{0}/configuration";
        
        public const string FileShares = "{0}/" + RegisteredPath + "/fileshares";

        public const string Vms = "{0}/" + RegisteredPath + "/vms";
        public const string Vmget = "{0}/" + RegisteredPath + "/vms/{1}";
        public const string DetachedDisksGet = "{0}/" + RegisteredPath + "/detachedDisks/{1}";
        public const string NameResolution = "{0}/" + RegisteredPath + "/nameresolution/{1}";
     
        public const string VmOps = "{0}/" + RegisteredPath + "/vmops";
        public const string VmOpGet = "{0}/" + RegisteredPath + "/vmops/{1}";
        public const string Domains = "{0}/" + RegisteredPath + "/domains";
        public const string VmSizes = "{0}/" + RegisteredPath + "/vmsizes";
        public const string Regions = "{0}/" + RegisteredPath + "/regions";
        public const string OSs = "{0}/" + RegisteredPath + "/oss";
        public const string Apps = "{0}/" + RegisteredPath + "/apps";
        public const string Subs = "{0}/" + RegisteredPath + "/wapsubscriptions";
        public const string RegionOSMappingValidation = "{0}/" + RegisteredPath + "/regionosmappingvalidation";
        public const string RegionSizeMappingValidation = "{0}/" + RegisteredPath + "/regionsizemappingvalidation";

        public const string ServicePrividerAccts = "{0}/" + RegisteredPath + "/servprovaccts";
        public const string ServiceCategories = "{0}/" + RegisteredPath + "/servicecategories";
        public const string ServerRoles = "{0}/" + RegisteredPath + "/serverroles";
        public const string SQLCollations = "{0}/" + RegisteredPath + "/sqlcollations";
        public const string SQLVersions = "{0}/" + RegisteredPath + "/sqlversions";
        public const string ServerRoleDriveMappings = "{0}/" + RegisteredPath + "/serverroledrivemappings";
        public const string NetworkNICs = "{0}/" + RegisteredPath + "/networknics";
        public const string ResourceGroups = "{0}/" + RegisteredPath + "/resourcegroups";
        public const string EnvironmentTypes = "{0}/" + RegisteredPath + "/environmenttypes";
        public const string IISRoleServices = "{0}/" + RegisteredPath + "/iissroleservices";
        public const string SQLAnalysisServiceModes = "{0}/" + RegisteredPath + "/sqlanalysisservicesmodes"; 

        public const string AllVms = RegisteredPath + "/vms";
        public const string AllVmOps = RegisteredPath + "/vmops";
        public const string AllDomains = RegisteredPath + "/domains";
        public const string AllVmSizes = RegisteredPath + "/vmsizes";
        public const string AllRegions = RegisteredPath + "/regions";
        public const string AllOSs = RegisteredPath + "/oss";
        public const string AllApps = RegisteredPath + "/apps";
        public const string AllServicePrividerAccts = RegisteredPath + "/servprovaccts";
        public const string AllServiceCategories = RegisteredPath + "/servicecategories";
        public const string AllServerRoles = RegisteredPath + "/serverroles";
        public const string AllSQLCollations = RegisteredPath + "/sqlcollations";
        public const string AllSQLVersions = RegisteredPath + "/sqlversions";
        public const string AllServerRoleDriveMappings = RegisteredPath + "/serverroledrivemappings";
        public const string AllNetworkNICs = RegisteredPath + "/networknics";
        public const string AllResourceGroups = RegisteredPath + "/resourcegroups";
        public const string AllEnvironmentTypes = RegisteredPath + "/environmenttypes";
        public const string AllIIsRoleServices = RegisteredPath + "/iissroleservices";
        public const string AllSQLAnalysisServiceModes = RegisteredPath + "/sqlanalysisservicesmodes";

        public Uri BaseEndpoint { get; set; }
        public HttpClient HttpClient;

        //*********************************************************************
        ///
        /// <summary>
        /// This constructor takes BearerMessageProcessingHandler which reads 
        /// token as attach to each request
        /// </summary>
        /// <param name="baseEndpoint"></param>
        /// <param name="handler"></param>
        /// 
        //*********************************************************************

        public CmpWapExtensionClient(Uri baseEndpoint, MessageProcessingHandler handler)
        {
            if (baseEndpoint == null) 
            {
                throw new ArgumentNullException("baseEndpoint"); 
            }

            this.BaseEndpoint = baseEndpoint;

            this.HttpClient = new HttpClient(handler) 
            {Timeout = TimeSpan.FromMinutes(2)};
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseEndpoint"></param>
        /// <param name="bearerToken"></param>
        /// <param name="timeout"></param>
        /// 
        //*********************************************************************

        public CmpWapExtensionClient(Uri baseEndpoint, string bearerToken, 
            TimeSpan? timeout = null)
        {
            if (baseEndpoint == null) 
            { 
                throw new ArgumentNullException("baseEndpoint"); 
            }

            this.BaseEndpoint = baseEndpoint;

            this.HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", bearerToken);

            if (timeout.HasValue)
            {
                this.HttpClient.Timeout = timeout.Value;
            }
        }
       
        #region --- Admin APIs ------------------------------------------------

        //*********************************************************************
        ///
        /// <summary>
        /// GetAdminSettings returns CmpWapExtension Resource Provider 
        /// endpoint information if its registered with Admin API
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<AdminSettings> GetAdminSettingsAsync()
        {
            var requestUrl = this.CreateRequestUri(CmpWapExtensionClient.AdminSettings);

            // For simplicity, we make a request synchronously.
            var response = await this.HttpClient.GetAsync(requestUrl, 
                HttpCompletionOption.ResponseContentRead);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AdminSettings>();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// UpdateAdminSettings registers CmpWapExtension Resource Provider 
        /// endpoint information with Admin API
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task UpdateAdminSettingsAsync(AdminSettings newSettings)
        {
            var requestUrl = this.CreateRequestUri(CmpWapExtensionClient.AdminSettings);
            var response = await this.HttpClient.PutAsJsonAsync<AdminSettings>(
                requestUrl.ToString(), newSettings);

            response.EnsureSuccessStatusCode();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// GetFileServerList return list of file servers hosted in 
        /// CmpWapExtension Resource Provider
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<FileServer>> GetFileServerListAsync()
        {
            var requestUrl = this.CreateRequestUri(string.Format(
                CultureInfo.InvariantCulture, CmpWapExtensionClient.AdminFileServers));

            var response = await this.HttpClient.GetAsync(requestUrl, 
                HttpCompletionOption.ResponseContentRead);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<List<FileServer>>();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// GetVMOSMappings return list of OS mapped to the subscription hosted in 
        /// CmpWapExtension Resource Provider
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public  Task<bool> GetVMRegionOSMappings(string subscriptionId, int[] Ids)
        {
            var requestUrl = this.CreateRequestUri(CmpWapExtensionClient.CreateVmRegionOSMappingValidationUri(subscriptionId));

            return this.PostAsyncWithReturnValue<int[], bool>(requestUrl, Ids);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// GetVMOSMappings return list of OS mapped to the subscription hosted in 
        /// CmpWapExtension Resource Provider
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public Task<bool> GetVMRegionSizeMappings(string subscriptionId, int[] Ids)
        {
            var requestUrl = this.CreateRequestUri(CmpWapExtensionClient.CreateVmRegionSizeMappingValidationUri(subscriptionId));

            return this.PostAsyncWithReturnValue<int[], bool>(requestUrl, Ids);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// UpdateFileServer updates existing file server information in 
        /// CmpWapExtension Resource Provider
        /// </summary>        
        /// 
        //*********************************************************************

        public async Task UpdateFileServerAsync(FileServer fileServer)
        {
            var requestUrl = this.CreateRequestUri(
                CmpWapExtensionClient.AdminFileServers);
            var response = await this.HttpClient.PutAsJsonAsync<FileServer>(
                requestUrl.ToString(), fileServer);

            response.EnsureSuccessStatusCode();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// AddFileServer adds new file server in CmpWapExtension Resource Provider
        /// </summary>        
        /// 
        //*********************************************************************

        public async Task AddFileServerAsync(FileServer fileServer)
        {
            var requestUrl = this.CreateRequestUri(CmpWapExtensionClient.AdminFileServers);
            var response = await this.HttpClient.PutAsJsonAsync<FileServer>(
                requestUrl.ToString(), fileServer);

            response.EnsureSuccessStatusCode();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// GetProductList return list of products stored in CmpWapExtension 
        /// Resource Provider
        /// </summary>        
        /// 
        //*********************************************************************

        public async Task<List<Product>> GetProductListAsync()
        {
            var requestUrl = this.CreateRequestUri(string.Format(
                CultureInfo.InvariantCulture, CmpWapExtensionClient.AdminProducts));

            var response = await this.HttpClient.GetAsync(requestUrl, 
                HttpCompletionOption.ResponseContentRead);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<List<Product>>();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// UpdateProduct updates existing product information in 
        /// CmpWapExtension Resource Provider
        /// </summary>        
        /// 
        //*********************************************************************

        public async Task UpdateProductAsync(Product product)
        {
            var requestUrl = this.CreateRequestUri(CmpWapExtensionClient.AdminProducts);
            var response = await this.HttpClient.PutAsJsonAsync<Product>(
                requestUrl.ToString(), product);

            response.EnsureSuccessStatusCode();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// AddProduct adds new product in CmpWapExtension Resource Provider
        /// </summary>        
        /// 
        //*********************************************************************

        public async Task AddProductAsync(Product product)
        {
            var requestUrl = this.CreateRequestUri(CmpWapExtensionClient.AdminProducts);
            var response = await this.HttpClient.PostAsXmlAsync<Product>(
                requestUrl.ToString(), product);

            response.EnsureSuccessStatusCode();
        }

        public async Task<PlanConfiguration> GetPlanConfigurationAsync(string planId)
        {
            var requestUrl = CreateRequestUri(string.Format(PlanConfigurations, planId));
            return await GetAsync<PlanConfiguration>(requestUrl);
        }

        public async Task SetPlanConfigurationAsync(string planId, PlanConfiguration configuration)
        {
            var requestUrl = CreateRequestUri(string.Format(PlanConfigurations, planId));
            await PostAsync<PlanConfiguration>(requestUrl, configuration);
        }

        #endregion

        #region --- Tenant APIs -----------------------------------------------

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<FileShare>> ListFileSharesAsync(string subscriptionId = null)
        {
            var requestUrl = this.CreateRequestUri(CmpWapExtensionClient.CreateUri(subscriptionId));
            return await this.GetAsync<List<FileShare>>(requestUrl);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<CreateVm>> ListVmsAsync(string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, 
                    CmpWapExtensionClient.AllVms));

                var response = await this.HttpClient.GetAsync(requestUrl, 
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<CreateVm>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateVmsUri(subscriptionId));
                return await this.GetAsync<List<CreateVm>>(requestUrl);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<Domain>> ListDomainsAsync(string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, 
                    CmpWapExtensionClient.AllDomains));

                var response = await this.HttpClient.GetAsync(requestUrl, 
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<Domain>>();

            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateDomainsUri(subscriptionId));
                return await this.GetAsync<List<Domain>>(requestUrl);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<OS>> ListOSsAsync(string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, 
                    CmpWapExtensionClient.AllOSs));

                var response = await this.HttpClient.GetAsync(requestUrl, 
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<OS>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateOSsUri(subscriptionId));
                return await this.GetAsync<List<OS>>(requestUrl);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<VmSize>> ListVmSizesAsync(string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, 
                    CmpWapExtensionClient.AllVmSizes));

                var response = await this.HttpClient.GetAsync(requestUrl, 
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<VmSize>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateVmSizesUri(subscriptionId));
                return await this.GetAsync<List<VmSize>>(requestUrl);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<Region>> ListTargetRegionsAsync(string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, 
                    CmpWapExtensionClient.AllRegions));

                var response = await this.HttpClient.GetAsync(requestUrl, 
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<Region>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateRegionsUri(subscriptionId));
                return await this.GetAsync<List<Region>>(requestUrl);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<App>> ListAppsAsync(string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, 
                    CmpWapExtensionClient.AllApps));

                var response = await this.HttpClient.GetAsync(requestUrl, 
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<App>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateAppsUri(subscriptionId));
                return await this.GetAsync<List<App>>(requestUrl);
            }
        }

        //New methods
        #region newmethods

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<ServerRole>> ListServerRolesAsync(string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture,
                    CmpWapExtensionClient.AllServerRoles));

                var response = await this.HttpClient.GetAsync(requestUrl,
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<ServerRole>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateServerRolesUri(subscriptionId));
                return await this.GetAsync<List<ServerRole>>(requestUrl);
            }
        }


        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<IISRoleService>> ListIISRolesAsync(string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture,
                    CmpWapExtensionClient.AllIIsRoleServices));

                var response = await this.HttpClient.GetAsync(requestUrl,
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<IISRoleService>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateIISRoleServicesUri(subscriptionId));
                return await this.GetAsync<List<IISRoleService>>(requestUrl);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<SQLAnalysisServiceModes>> 
            ListSQLAnalysisServicesAsync(string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture,
                    CmpWapExtensionClient.AllSQLAnalysisServiceModes));

                var response = await this.HttpClient.GetAsync(requestUrl,
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<SQLAnalysisServiceModes>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateSQLAnalysisServiceModesUri(subscriptionId));
                return await this.GetAsync<List<SQLAnalysisServiceModes>>(requestUrl);
            }
        }
        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<ServiceCategory>> ListCategoriesAsync(
            string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture,
                    CmpWapExtensionClient.AllServiceCategories));

                var response = await this.HttpClient.GetAsync(requestUrl,
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<ServiceCategory>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateCategoriesUri(subscriptionId));
                return await this.GetAsync<List<ServiceCategory>>(requestUrl);
            }
        }
        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<SQLCollation>> ListSqlCollationsAsync(
            string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture,
                    CmpWapExtensionClient.AllSQLCollations));

                var response = await this.HttpClient.GetAsync(requestUrl,
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<SQLCollation>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateSqlCollationsUri(subscriptionId));
                return await this.GetAsync<List<SQLCollation>>(requestUrl);
            }
        }
        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<SQLVersion>> ListSqlVersionsAsync(
            string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture,
                    CmpWapExtensionClient.AllSQLVersions));

                var response = await this.HttpClient.GetAsync(requestUrl,
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<SQLVersion>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateSqlVersionsUri(subscriptionId));
                return await this.GetAsync<List<SQLVersion>>(requestUrl);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<ServerRoleDriveMapping>> 
            ListServerRoleDriverMappingsAsync(string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture,
                    CmpWapExtensionClient.AllServerRoleDriveMappings));

                var response = await this.HttpClient.GetAsync(requestUrl,
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<ServerRoleDriveMapping>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateServerRoleDriveMappingsUri(subscriptionId));
                return await this.GetAsync<List<ServerRoleDriveMapping>>(requestUrl);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<NetworkNIC>> ListNetworkNicsAsync(string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture,
                    CmpWapExtensionClient.AllNetworkNICs));

                var response = await this.HttpClient.GetAsync(requestUrl,
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<NetworkNIC>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateNetworkNICsUri(subscriptionId));
                return await this.GetAsync<List<NetworkNIC>>(requestUrl);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<ResourceGroup>> ListResourceGroupsAsync(
            string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture,
                    CmpWapExtensionClient.AllResourceGroups));

                var response = await this.HttpClient.GetAsync(requestUrl,
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<ResourceGroup>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateResourceGroupsUri(subscriptionId));
                return await this.GetAsync<List<ResourceGroup>>(requestUrl);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<EnvironmentType>> ListEnvironmentTypesAsync(
            string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture,
                    CmpWapExtensionClient.AllEnvironmentTypes));

                var response = await this.HttpClient.GetAsync(requestUrl,
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<EnvironmentType>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateEnvironmentTypesUri(subscriptionId));
                return await this.GetAsync<List<EnvironmentType>>(requestUrl);
            }
        }

        public async Task<List<Subscription>> ListSubscriptionMappings(string [] subscriptionIds)
        {
            var requestUrl = this.CreateRequestUri(CmpWapExtensionClient.CreateSubsUri(subscriptionIds[0]));

            return await this.PostAsyncWithReturnValue<string[], List<Subscription>>(requestUrl, subscriptionIds);
        }

#endregion 

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<ServiceProviderAccount>> ListServProvAcctsAsync(
            string subscriptionId = null)
        {
            if (null == subscriptionId)
            {
                var requestUrl =
                    this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, 
                    CmpWapExtensionClient.AllServicePrividerAccts));

                var response = await this.HttpClient.GetAsync(requestUrl, 
                    HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<ServiceProviderAccount>>();
            }
            else
            {
                var requestUrl = this.CreateRequestUri(
                    CmpWapExtensionClient.CreateServicePrividerAcctsUri(subscriptionId));
                return await this.GetAsync<List<ServiceProviderAccount>>(requestUrl);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPa"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<List<ServiceProviderAccount>> UpdateServProvAcctAsync(
            ServiceProviderAccount sPa)
        {
            var requestUrl =
                this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture,
                CmpWapExtensionClient.AllServicePrividerAccts));

            var response = await this.HttpClient.PostAsXmlAsync<ServiceProviderAccount>(
                requestUrl.ToString(), sPa);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<List<ServiceProviderAccount>>();

            //*************************

            //var requestUrl = this.CreateRequestUri(CmpWapExtensionClient.CreateVmsUri(subscriptionId));
            //await this.PostAsync<ServiceProviderAccount>(requestUrl, sPa);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="fileShareNameToCreate"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task CreateFileShareAsync(string subscriptionId, 
            FileShare fileShareNameToCreate)
        {
            var requestUrl = this.CreateRequestUri(CmpWapExtensionClient.CreateUri(subscriptionId));
            await this.PostAsync<FileShare>(requestUrl, fileShareNameToCreate);            
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="fileShareNameToUpdate"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************
   
        public async Task UpdateFileShareAsync(string subscriptionId, 
            FileShare fileShareNameToUpdate)
        {
            var requestUrl = this.CreateRequestUri(CmpWapExtensionClient.CreateUri(subscriptionId));
            await this.PutAsync<FileShare>(requestUrl, fileShareNameToUpdate);            
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

        public async Task CreateVmAsync(string subscriptionId, CreateVm vmToCreate)
        {
            var requestUrl = this.CreateRequestUri(CmpWapExtensionClient.CreateVmsUri(subscriptionId));
            await this.PostAsync<CreateVm>(requestUrl, vmToCreate);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="vmId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<VmDashboardInfo> VmgetAsync(string subscriptionId, int vmId)
        {
            var requestUrl = this.CreateRequestUri(CmpWapExtensionClient.CreateVmgetUri(subscriptionId, vmId));
           return await this.GetAsync<VmDashboardInfo>(requestUrl);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="vmId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<VmOp> GetVmOpsQueueResponseAsync(string subscriptionId, int vmId)
        {
            var requestUrl = this.CreateRequestUri(CmpWapExtensionClient.CreateVmOpUri(subscriptionId, vmId));
            return await this.GetAsync<VmOp>(requestUrl);
        }

        private static string CreateVmOpUri(string subscriptionId, int vmId)
        {
            return string.Format(CultureInfo.InvariantCulture,
                CmpWapExtensionClient.VmOpGet, subscriptionId, vmId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="vmId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<IEnumerable<DataVirtualHardDisk>> DetachedDisksGetAsync(
            string subscriptionId, int vmId)
        {
            var requestUrl = this.CreateRequestUri(
                CmpWapExtensionClient.CreateDetachedDisksUri(subscriptionId, vmId));

            return await this.GetAsync<IEnumerable<DataVirtualHardDisk>>(requestUrl);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="securitygroups"></param>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public async Task<SecurityGroupResult> NameResolutionAsync(
            string securitygroups, string subscriptionId)
        {
            var requestUrl = this.CreateRequestUri(
                CmpWapExtensionClient.NameResolutionUri(securitygroups, subscriptionId));

            return await this.GetAsync<SecurityGroupResult>(requestUrl);
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

        public async Task VmOpAsync(string subscriptionId, VmOp vmOp)
        {
            var requestUrl = this.CreateRequestUri(
                CmpWapExtensionClient.CreateVmOpUri(subscriptionId));

            await this.PostAsync<VmOp>(requestUrl, vmOp);
        }

        #endregion

        #region Private Methods

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private async Task<T> GetAsync<T>(Uri requestUrl)
        {         
            var response = await this.HttpClient.GetAsync(requestUrl, 
                HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************
        private async Task PostAsync<T>(Uri requestUrl, T content)
        {
            //var response = await this.HttpClient.PostAsXmlAsync<T>(requestUrl.ToString(), content);
            var response = await this.HttpClient.PostAsJsonAsync<T>(requestUrl.ToString(), content);

            if (!response.IsSuccessStatusCode) // handle all non-success errors here
            {
                string responseBody = response.ReasonPhrase;
                throw new Exception(responseBody);
            }

            response.EnsureSuccessStatusCode();
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <typeparam name="T"></typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <param name="requestUrl"></param>
        ///  <param name="content"></param>
        ///  <returns></returns>
        ///  
        //*********************************************************************
        private async Task<TU> PostAsyncWithReturnValue<T,TU>(Uri requestUrl, T content)
        {
            //var response = await this.HttpClient.PostAsXmlAsync<T>(requestUrl.ToString(), content);
            var response = await this.HttpClient.PostAsJsonAsync<T>(requestUrl.ToString(), content);

            if (!response.IsSuccessStatusCode) // handle all non-success errors here
            {
                string responseBody = response.ReasonPhrase;
                throw new Exception(responseBody);
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<TU>();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private async Task PutAsync<T>(Uri requestUrl, T content)
        {            
            var response = await this.HttpClient.PutAsJsonAsync<T>(
                requestUrl.ToString(), content);

            response.EnsureSuccessStatusCode();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************
       
        private Uri CreateRequestUri(string relativePath, string queryString = "")
        {
            var endpoint = new Uri(this.BaseEndpoint, relativePath);
            var uriBuilder = new UriBuilder(endpoint) 
            {Query = queryString};

            return uriBuilder.Uri;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.FileShares, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateVmsUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.Vms, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="vmId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateVmgetUri(string subscriptionId,int vmId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.Vmget, subscriptionId, vmId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="vmId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateDetachedDisksUri(string subscriptionId, int vmId)
        {
            return String.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.DetachedDisksGet, subscriptionId, vmId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="securitygroups"></param>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string NameResolutionUri(string securitygroups,string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.NameResolution,subscriptionId, securitygroups);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateVmOpUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.VmOps, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateDomainsUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.Domains, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateVmSizesUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.VmSizes, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateRegionsUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.Regions, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateAppsUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.Apps, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateServerRolesUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.ServerRoles, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateIISRoleServicesUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.IISRoleServices, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateSQLAnalysisServiceModesUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.SQLAnalysisServiceModes, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateCategoriesUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.ServiceCategories, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateSqlCollationsUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.SQLCollations, subscriptionId);
        }
 
        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateSqlVersionsUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.SQLVersions, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateServerRoleDriveMappingsUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.ServerRoleDriveMappings, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateNetworkNICsUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.NetworkNICs, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateResourceGroupsUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.ResourceGroups, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateEnvironmentTypesUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.EnvironmentTypes, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateServicePrividerAcctsUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.ServicePrividerAccts, subscriptionId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string CreateOSsUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, 
                CmpWapExtensionClient.OSs, subscriptionId);
        }

        private static string CreateSubsUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture,
                CmpWapExtensionClient.Subs, subscriptionId);
        }

        private static string CreateVmRegionOSMappingValidationUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture,
                CmpWapExtensionClient.RegionOSMappingValidation, subscriptionId);
        }

        private static string CreateVmRegionSizeMappingValidationUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture,
                CmpWapExtensionClient.RegionSizeMappingValidation, subscriptionId);
        }
        #endregion
    }
}
