//-----------------------------------------------------------------------
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.WindowsAzurePack.CmpWapExtension.Common;
using Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models;
using System;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Controllers
{
    //*********************************************************************
    /// 
    /// <summary>
    /// Entry point to the tenant extension API
    /// </summary>
    /// 
    //*********************************************************************
    [RequireHttps]
    [OutputCache(Location = OutputCacheLocation.None)]
    [PortalExceptionHandler]
    public sealed class CmpWapExtensionTenantController : ExtensionController
    {
        //*********************************************************************
        ///
        /// <summary>
        /// List file shares belong to subscription
        /// NOTE: For this sample dummy entries will be displayed
        /// </summary>
        /// <param name="subscriptionIds">WAP subscription IDs to check against</param>
        /// <returns>A JSON object containing the file shares</returns>
        /// 
        //*********************************************************************

        [HttpPost]
        public async Task<JsonResult> ListFileShares(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var fileShares = new List<FileShareModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var fileSharesFromApi = await ClientFactory.CmpWapExtensionClient.ListFileSharesAsync(subId);
                fileShares.AddRange(fileSharesFromApi.Select(d => new FileShareModel(d)));
            }

            return this.JsonDataSet(fileShares, namePropertyName: "Name");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// List VM Requests that belong to the subscription
        /// </summary>
        /// <param name="subscriptionIds">WAP subscription IDs to check against</param>
        /// <returns>A JSON object containing a list of associated VMs</returns>
        /// 
        //*********************************************************************

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public async Task<JsonResult> ListVMs(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var vMs = new List<CreateVmModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var vMsFromApi = await ClientFactory.CmpWapExtensionClient.ListVmsAsync(subId);
                vMs.AddRange(vMsFromApi.Select(d => new CreateVmModel(d)));
            }

            return this.JsonDataSet(vMs, namePropertyName: "Name");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Lists domains associated with a WAP subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscription to check against</param>
        /// <returns>A JSON object containing a list of the associated domains</returns>
        /// 
        //*********************************************************************

        [HttpPost]
        public async Task<JsonResult> ListDomains(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var domains = new List<CreateDomainModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var domainsFromApi = await ClientFactory.CmpWapExtensionClient.ListDomainsAsync(subId);
                domains.AddRange(domainsFromApi.Select(d => new CreateDomainModel(d)));
            }

            return this.JsonDataSet(domains, namePropertyName: "Name");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Returns a list of operating systems associated with a WAP subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscriptions to check against</param>
        /// <returns>A JSON object containing the operating systems</returns>
        /// 
        //*********************************************************************

        [HttpPost]
        public async Task<JsonResult> ListOSs(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var domains = new List<CreateOsModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var domainsFromApi = await ClientFactory.CmpWapExtensionClient.ListOSsAsync(subId);
                domains.AddRange(domainsFromApi.Select(d => new CreateOsModel(d)));
            }
            domains = domains.Distinct(new CreateOsComparer()).ToList();

            return this.JsonDataSet(domains, namePropertyName: "Name");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets the possible VM sizes associated with a WAP subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscriptions to check against</param>
        /// <returns>A JSON object containing the available sizes</returns>
        /// 
        //*********************************************************************

        [HttpPost]
        public async Task<JsonResult> ListVmSizes(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var domains = new List<CreateSizeModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var domainsFromApi = await ClientFactory.CmpWapExtensionClient.ListVmSizesAsync(subId);
                domains.AddRange(domainsFromApi.Select(d => new CreateSizeModel(d)));
            }
            domains = domains.Distinct(new CreateSizeComparer()).ToList();

            return this.JsonDataSet(domains, namePropertyName: "Name");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Lists the target regions associated with a WAP subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscription to check against</param>
        /// <returns>A JSON object containing the regions</returns>
        /// 
        //*********************************************************************

        [HttpPost]
        public async Task<JsonResult> ListTargetRegions(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var domains = new List<CreateRegionModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var domainsFromApi = await ClientFactory.CmpWapExtensionClient.ListTargetRegionsAsync(subId);
                domains.AddRange(domainsFromApi.Select(d => new CreateRegionModel(d)));
            }
            domains = domains.Distinct(new CreateRegionComparer()).ToList();

            return this.JsonDataSet(domains, namePropertyName: "Name");
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Lists the possible application names for a given WAP subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscription to check against</param>
        /// <returns>A JSON object representing the possible names</returns>
        /// 
        //*********************************************************************
        [HttpPost]
        public async Task<JsonResult> ListApps(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var domains = new List<CreateAppModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            /*
             * Dummied this out since at the moment, we are displaying all App names, regardless to
             * which subscription they belong to. If in the future there is a method that returns an
             * App object specific to a WAP subscription, then the below commented code would be applicable
             * to query the app objects for each subscription. Right now, I just use the first one and get all
             * the Application objects available
             */
            //foreach (var subId in subscriptionIds)
            //{
            //    var domainsFromApi = await ClientFactory.CmpWapExtensionClient.ListAppsAsync(subId);
            //    domains.AddRange(domainsFromApi.Select(d => new CreateAppModel(d)));
            //}

            var domainsFromApi = await ClientFactory.CmpWapExtensionClient.ListAppsAsync(subscriptionIds[0]);
            domains.AddRange(domainsFromApi.Select(d => new CreateAppModel(d)));

            return this.JsonDataSet(domains, namePropertyName: "Name");
        }
        //New actions

        //*********************************************************************
        /// 
        /// <summary>
        /// Lists the available server roles for a given WAP subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscription to check against</param>
        /// <returns>A JSON object representing the possible roles</returns>
        /// 
        //*********************************************************************
        [HttpPost]
        public async Task<JsonResult> ListServerRoles(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var serverroles = new List<CreateServerRoleModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var serverrolesFromApi = 
                    await ClientFactory.CmpWapExtensionClient.ListServerRolesAsync(subId);
                serverroles.AddRange(serverrolesFromApi.Select(
                    d => new CreateServerRoleModel(d)));
            }

            return this.JsonDataSet(serverroles, namePropertyName: "Name");
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Lists the categories for a given WAP subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscription to check against</param>
        /// <returns>A JSON object representing the possible categories</returns>
        /// 
        //*********************************************************************
        [HttpPost]
        public async Task<JsonResult> ListCategories(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var categories = new List<CreateServiceCategoryModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var categoriesFromApi = 
                    await ClientFactory.CmpWapExtensionClient.ListCategoriesAsync(subId);
                categories.AddRange(categoriesFromApi.Select(
                    d => new CreateServiceCategoryModel(d)));
            }

            return this.JsonDataSet(categories, namePropertyName: "Name");
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Lists the possible SQL Server analysis service modes for a given WAP subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscription to check against</param>
        /// <returns>A JSON object representing the possible modes</returns>
        /// 
        //*********************************************************************
        [HttpPost]
        public async Task<JsonResult> ListSQLAnalysisServiceModes(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var categories = new List<SQLAnalysisServiceModesModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var sqlanalysisservicesFromApi = 
                    await ClientFactory.CmpWapExtensionClient.ListSQLAnalysisServicesAsync(subId);
                categories.AddRange(sqlanalysisservicesFromApi.Select(
                    d => new SQLAnalysisServiceModesModel(d)));
            }

            return this.JsonDataSet(categories, namePropertyName: "Name");
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Lists the possible IIS role services for a given WAP subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscription to check against</param>
        /// <returns>A JSON object representing the possible services</returns>
        /// 
        //*********************************************************************
        [HttpPost]
        public async Task<JsonResult> ListIISRoleServices(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var categories = new List<IISRoleServicesModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var iisroleservicesFromApi = 
                    await ClientFactory.CmpWapExtensionClient.ListIISRolesAsync(subId);
                categories.AddRange(iisroleservicesFromApi.Select(
                    d => new IISRoleServicesModel(d)));
            }

            return this.JsonDataSet(categories, namePropertyName: "Name");
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Lists the available SQL Server collations for a given WAP subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscription to check against</param>
        /// <returns>A JSON object representing the possible collations</returns>
        /// 
        //*********************************************************************
        [HttpPost]
        public async Task<JsonResult> ListSqlCollations(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var sqlcollations = new List<SQLCollationModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var sqlcollationsFromApi = 
                    await ClientFactory.CmpWapExtensionClient.ListSqlCollationsAsync(subId);
                sqlcollations.AddRange(sqlcollationsFromApi.Select(
                    d => new SQLCollationModel(d)));
            }

            return this.JsonDataSet(sqlcollations, namePropertyName: "Name");
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Lists the possible SQL Server versions for a given WAP subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscription to check against</param>
        /// <returns>A JSON object representing the possible versions</returns>
        /// 
        //*********************************************************************
        [HttpPost]
        public async Task<JsonResult> ListSqlVersions(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var domains = new List<SQLVersionModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var domainsFromApi = 
                    await ClientFactory.CmpWapExtensionClient.ListSqlVersionsAsync(subId);
                domains.AddRange(domainsFromApi.Select(d => new SQLVersionModel(d)));
            }

            return this.JsonDataSet(domains, namePropertyName: "Name");
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Lists the available VM environments for a given WAP subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscription to check against</param>
        /// <returns>A JSON object representing the possible environments</returns>
        /// 
        //*********************************************************************
        [HttpPost]
        public async Task<JsonResult> ListEnvironments(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var environments = new List<CreateEnvironmentTypeModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var environmentsFromApi = 
                    await ClientFactory.CmpWapExtensionClient.ListEnvironmentTypesAsync(subId);
                environments.AddRange(environmentsFromApi.Select(
                    d => new CreateEnvironmentTypeModel(d)));
            }

            return this.JsonDataSet(environments, namePropertyName: "Name");
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Lists the available network NICs for a given WAP subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscription to check against</param>
        /// <returns>A JSON object representing the possible NICs</returns>
        /// 
        //*********************************************************************
        [HttpPost]
        public async Task<JsonResult> LisNetworkNICs(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var networknics = new List<NetworkNICModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var networknicsFromApi = 
                    await ClientFactory.CmpWapExtensionClient.ListNetworkNicsAsync(subId);
                networknics.AddRange(networknicsFromApi.Select(
                    d => new NetworkNICModel(d)));
            }

            return this.JsonDataSet(networknics, namePropertyName: "Name");
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Lists the available resource groups for a given WAP subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscription to check against</param>
        /// <returns>A JSON object representing the possible resource groups</returns>
        /// 
        //*********************************************************************
        [HttpPost]
        public async Task<JsonResult> LisResourceGroups(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var networknics = new List<ResourceGroupModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var networknicsFromApi = 
                    await ClientFactory.CmpWapExtensionClient.ListResourceGroupsAsync(subId);
                networknics.AddRange(networknicsFromApi.Select(
                    d => new ResourceGroupModel(d)));
            }

            return this.JsonDataSet(networknics, namePropertyName: "Name");
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Lists the available drive mappings for each server role for a given WAP subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscription to check against</param>
        /// <returns>A JSON object representing the mappings</returns>
        /// 
        //*********************************************************************
        [HttpPost]
        public async Task<JsonResult> LisServerRoleDriveMappings(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var serverroledrivemappings = new List<ServerRoleDriveMappingModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var serverroledrivemappingsFromApi = 
                    await ClientFactory.CmpWapExtensionClient.ListServerRoleDriverMappingsAsync(subId);
                serverroledrivemappings.AddRange(
                    serverroledrivemappingsFromApi.Select(
                    d => new ServerRoleDriveMappingModel(d)));
            }

            return this.JsonDataSet(serverroledrivemappings, namePropertyName: "Name");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Create new file share for subscription
        /// </summary>
        /// <param name="subscriptionId">The WAP subscription to add the share to</param>
        /// <param name="fileShareToCreate">The file share to add</param>
        /// <returns>The file share represented as JSON</returns>
        /// 
        //*********************************************************************

        [HttpPost]
        public async Task<JsonResult> CreateFileShare(string subscriptionId, 
            FileShareModel fileShareToCreate)
        {
            await ClientFactory.CmpWapExtensionClient.CreateFileShareAsync(
                subscriptionId, fileShareToCreate.ToApiObject());
            return this.Json(fileShareToCreate);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Creates a new VM in a WAP subscription
        /// </summary>
        /// <param name="subscriptionId">The WAP subscription to use when adding the VM</param>
        /// <param name="vmToCreate">The VM to create</param>
        /// <returns>The new VM as a JSON object</returns>
        /// 
        //*********************************************************************

        [HttpPost]
        public async Task<JsonResult> CreateVm(string subscriptionId, 
            CreateVmModel vmToCreate)
        {
            try
            {
                await ClientFactory.CmpWapExtensionClient.CreateVmAsync(
                    subscriptionId, vmToCreate.ToApiObject());
            }
            catch (Exception ex)
            {
                throw new PortalException(ex.Message);
            }
            return this.Json(vmToCreate);
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Resolves a security group
        /// </summary>
        /// <param name="subscriptionId">The WAP subscription to check against</param>
        /// <param name="securitygroups">The security groups to resolve</param>
        /// <returns>The resolved security groups as JSON</returns>
        /// 
        //*********************************************************************
        [HttpPost]
        public async Task<JsonResult> NameResolution(string subscriptionId,
            string securitygroups)
        {
      
          //  var result = new List<SecurityGroupResultModel>();
            var resolvedsecuritygroups = 
                await ClientFactory.CmpWapExtensionClient.NameResolutionAsync(
                securitygroups, subscriptionId);

            return this.JsonDataSet(resolvedsecuritygroups);
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Returns a list of detached disks in a WAP subscription
        /// </summary>
        /// <param name="subscriptionId">The WAP subscription to check against</param>
        /// <param name="vmId">The CMP request ID to filter on for available disks</param>
        /// <returns>A JSON object containing a list of detached disks available to a VM</returns>
        /// 
        //*********************************************************************
        [HttpPost]
        public async Task<JsonResult> GetDetachedDisks(string subscriptionId, int vmId)
        {
            var disks = 
                await ClientFactory.CmpWapExtensionClient.DetachedDisksGetAsync(
                subscriptionId, vmId);
            return this.JsonDataSet(disks);
        }

        

        //*********************************************************************
        /// <summary>
        /// Gets details on a VM
        /// </summary>
        /// <param name="subscriptionId">The WAP subscription in which the VM resides</param>
        /// <param name="vmId">The CMP request ID to check for</param>
        /// <returns>A JSON object representing the VM</returns>
        /// 
        //*********************************************************************

        [HttpPost]
        public async Task<JsonResult> GetVm(string subscriptionId, int vmId)
        {
    
            var result = new VmDashboardModel();
            var vmInfo = 
                await ClientFactory.CmpWapExtensionClient.VmgetAsync(
                subscriptionId, vmId);

            return this.JsonDataSet(vmInfo);
        }

        [HttpPost]
        public async Task<JsonResult> GetVmOpsQueueTask(string subscriptionId, int vmId)
        {
            try
            {
                if (string.IsNullOrEmpty(subscriptionId))
                {
                    throw new HttpException("Subscription Id not found");
                }
                var vmOpsInfo = await ClientFactory.CmpWapExtensionClient.GetVmOpsQueueResponseAsync(subscriptionId, vmId);
                return this.JsonDataSet(vmOpsInfo);
            }
            catch (Exception ex)
            {
                throw new PortalException(ex.Message);   // Throw an exception that is to be handled by the portal
            }
        }
        
        //*********************************************************************
        ///
        /// <summary>
        /// Performs an operation on a VM
        /// </summary>
        /// <param name="subscriptionId">The WAP subscription in which the 
        /// VM resides</param>
        /// <param name="vmOp">Details of the operation to perform</param>
        /// <returns>The operation as JSON</returns>
        /// 
        //*********************************************************************
        [HttpPost]
        public async Task<JsonResult> VmOp(string[] subscriptionId, VmOpModel vmOp)
        {
            try
            {
                if (subscriptionId == null || subscriptionId.Length == 0)
                {
                    throw new HttpException("Subscription Id not found");
                }

                foreach(var subId in subscriptionId)
                {
                    await ClientFactory.CmpWapExtensionClient.VmOpAsync(subId, vmOp.ToApiObject());                
                }
            }
            catch(Exception ex)
            {
                throw new PortalException(ex.Message);   // Throw an exception that is to be handled by the portal
            }
            return this.Json(vmOp);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Returns the mappings between the WAP subscription sent and their
        /// respective Azure subscription
        /// </summary>
        /// <param name="subscriptionIds">The WAP subscription a user belongs
        /// to</param>
        /// <returns>The operation as JSON</returns>
        /// 
        //*********************************************************************
        [HttpPost]
        public async Task<JsonResult> ListSubscriptionMappings(string[] subscriptionIds)
        {
            try
            {
                if (!subscriptionIds.Any())
                {
                    throw new HttpException("Subscription Id not found");
                }

                var mappings = await ClientFactory.CmpWapExtensionClient.ListSubscriptionMappings(subscriptionIds);
                return this.Json(mappings);
            }
            catch (Exception ex)
            {
                throw new PortalException(ex.Message);   // Throw an exception that is to be handled by the portal
            }
            
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Creates an RDP connection
        /// </summary>
        /// <returns>The view for this controller</returns>
        /// 
        //*********************************************************************
        [HttpGet]
        public ActionResult OpenRDP()
        {
            string address = Request.QueryString["vmIp"];
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}.rdp", address));
            Response.Output.Write(@"
            screen mode id:i:2
            session bpp:i:32
            compression:i:1
            keyboardhook:i:2
            displayconnectionbar:i:1
            disable wallpaper:i:1
            disable full window drag:i:1
            allow desktop composition:i:0
            allow font smoothing:i:0
            disable menu anims:i:1
            disable themes:i:0
            disable cursor setting:i:0
            bitmapcachepersistenable:i:1
            full address:s:{0}
            audiomode:i:0
            redirectprinters:i:1
            redirectcomports:i:0
            redirectsmartcards:i:1
            redirectclipboard:i:1
            redirectposdevices:i:0
            redirectdrives:i:0
            autoreconnection enabled:i:1
            authentication level:i:2
            prompt for credentials:i:0
            negotiate security layer:i:1
            remoteapplicationmode:i:0
            alternate shell:s:
            shell working directory:s:
            gatewayhostname:s:
            gatewayusagemethod:i:4
            gatewaycredentialssource:i:4
            gatewayprofileusagemethod:i:0
            promptcredentialonce:i:1
            use multimon:i:0
            audiocapturemode:i:0
            videoplaybackmode:i:1
            connection type:i:2
            redirectdirectx:i:1
            use redirection server name:i:0", address);
            Response.End();
            return View();
        }
    }
}
