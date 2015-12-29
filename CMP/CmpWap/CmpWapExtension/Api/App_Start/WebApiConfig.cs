// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Web.Http;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api
{
    public static class WebApiConfig
    {
        /// <summary>
        /// This method is used to register routes to the controller
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
               name: "AdminSettings",
               routeTemplate: "admin/settings",
               defaults: new { controller = "AdminSettings" });

            config.Routes.MapHttpRoute(
                name: "AdminProducts",
                routeTemplate: "admin/products",
                defaults: new { controller = "Products" });

            config.Routes.MapHttpRoute(
                name: "AdminFileServers",
                routeTemplate: "admin/fileservers",
                defaults: new { controller = "FileServers" });

            config.Routes.MapHttpRoute(
               name: "CmpWapExtensionQuota",
               routeTemplate: "admin/quota",
               defaults: new { controller = "Quota" });

            config.Routes.MapHttpRoute(
               name: "CmpWapExtensionDefaultQuota",
               routeTemplate: "admin/defaultquota",
               defaults: new { controller = "Quota" });

            config.Routes.MapHttpRoute(
               name: "Subscriptions",
               routeTemplate: "admin/subscriptions",
               defaults: new { controller = "Subscriptions" });

            config.Routes.MapHttpRoute(
               name: "FileShares",
               routeTemplate: "subscriptions/{subscriptionId}/fileshares",
               defaults: new { controller = "FileShare" });

            config.Routes.MapHttpRoute(
                name: "DetachedDisks",
                routeTemplate: "subscriptions/{subscriptionId}/detachedDisks/{vmId}",
                defaults: new { controller = "Disks" });

            config.Routes.MapHttpRoute(
              name: "Vmget",
              routeTemplate: "subscriptions/{subscriptionId}/vms/{Id}",
              defaults: new { controller = "Vms" });
            config.Routes.MapHttpRoute(
               name: "Vms",
               routeTemplate: "subscriptions/{subscriptionId}/vms",
               defaults: new { controller = "Vms" });

            config.Routes.MapHttpRoute(
                name: "AdminVms",
                routeTemplate: "admin/vms",
                defaults: new { controller = "Vms" });


            config.Routes.MapHttpRoute(
              name: "wapsubs",
              routeTemplate: "admin/vms/{action}",
              defaults: new { controller = "Vms" });

           
            config.Routes.MapHttpRoute(
               name: "VmOps",
               routeTemplate: "subscriptions/{subscriptionId}/vmops",
               defaults: new { controller = "VmOps" });

            config.Routes.MapHttpRoute(
               name: "VmOpGet",
               routeTemplate: "subscriptions/{subscriptionId}/vmops/{Id}",
               defaults: new { controller = "VmOps" });

            config.Routes.MapHttpRoute(
                name: "AdminResourceGroup",
                routeTemplate: "admin/resourcegroups",
                defaults: new { controller = "ResourceGroup" });


            config.Routes.MapHttpRoute(
              name: "Resourcegroup",
              routeTemplate: "subscriptions/{subscriptionId}/resourcegroups",
              defaults: new { controller = "ResourceGroup" });

            config.Routes.MapHttpRoute(
                name: "AdminVmOps",
                routeTemplate: "admin/vmops",
                defaults: new { controller = "VmOps" });

            config.Routes.MapHttpRoute(
               name: "Domains",
               routeTemplate: "subscriptions/{subscriptionId}/domains",
               defaults: new { controller = "Domains" });

            config.Routes.MapHttpRoute(
                name: "AdminDomains",
                routeTemplate: "admin/domains",
                defaults: new { controller = "Domains" });

            config.Routes.MapHttpRoute(
               name: "OSs",
               routeTemplate: "subscriptions/{subscriptionId}/oss",
               defaults: new { controller = "OSs" });

            config.Routes.MapHttpRoute(
                name: "AdminOSs",
                routeTemplate: "admin/oss",
                defaults: new { controller = "OSs" });

            config.Routes.MapHttpRoute(
               name: "VmSizes",
               routeTemplate: "subscriptions/{subscriptionId}/vmsizes",
               defaults: new { controller = "VmSizes" });

            config.Routes.MapHttpRoute(
                name: "AdminVmSizes",
                routeTemplate: "admin/vmsizes",
                defaults: new { controller = "VmSizes" });

            config.Routes.MapHttpRoute(
               name: "Regions",
               routeTemplate: "subscriptions/{subscriptionId}/regions",
               defaults: new { controller = "Regions" });

            config.Routes.MapHttpRoute(
                name: "AdminRegions",
                routeTemplate: "admin/regions",
                defaults: new { controller = "Regions" });

            config.Routes.MapHttpRoute(
               name: "Apps",
               routeTemplate: "subscriptions/{subscriptionId}/apps",
               defaults: new { controller = "Apps" });

            config.Routes.MapHttpRoute(
                name: "AdminApps",
                routeTemplate: "admin/apps",
                defaults: new { controller = "Apps" });

            config.Routes.MapHttpRoute(
               name: "ServProvAccts",
               routeTemplate: "subscriptions/{subscriptionId}/servprovaccts",
               defaults: new { controller = "ServiceProviderAccounts" });

            config.Routes.MapHttpRoute(
                name: "AdminServProvAccts",
                routeTemplate: "admin/servprovaccts",
                defaults: new { controller = "ServiceProviderAccounts" });

            config.Routes.MapHttpRoute(
                name: "PlanConfiguration",
                routeTemplate: "admin/{planId}/configuration",
                defaults: new { controller = "Plans" });

            config.Routes.MapHttpRoute(
               name: "ServiceCategories",
               routeTemplate: "subscriptions/{subscriptionId}/servicecategories",
               defaults: new { controller = "ServiceCategories" });

            config.Routes.MapHttpRoute(
                name: "AdminServiceCategories",
                routeTemplate: "admin/servicecategories",
                defaults: new { controller = "ServiceCategories" });

            config.Routes.MapHttpRoute(
               name: "EnvironmentTypes",
               routeTemplate: "subscriptions/{subscriptionId}/environmenttypes",
               defaults: new { controller = "EnvironmentTypes" });

            config.Routes.MapHttpRoute(
                name: "AdminEnvironmentTypes",
                routeTemplate: "admin/environmenttypes",
                defaults: new { controller = "EnvironmentTypes" });

            config.Routes.MapHttpRoute(
               name: "ServerRoleDriveMappings",
               routeTemplate: "subscriptions/{subscriptionId}/serverroledrivemappings",
               defaults: new { controller = "ServerRoleDriveMappings" });

            config.Routes.MapHttpRoute(
                name: "AdminServerRoleDriveMappings",
                routeTemplate: "admin/serverroledrivemappings",
                defaults: new { controller = "ServerRoleDriveMappings" });

            config.Routes.MapHttpRoute(
               name: "ServerRoles",
               routeTemplate: "subscriptions/{subscriptionId}/serverroles",
               defaults: new { controller = "ServerRoles" });

            config.Routes.MapHttpRoute(
                name: "AdminServerRoles",
                routeTemplate: "admin/serverroles",
                defaults: new { controller = "ServerRoles" });
            config.Routes.MapHttpRoute(
               name: "SQLCollations",
               routeTemplate: "subscriptions/{subscriptionId}/sqlcollations",
               defaults: new { controller = "SQLCollations" });

            config.Routes.MapHttpRoute(
                name: "AdminSQLCollations",
                routeTemplate: "admin/sqlcollations",
                defaults: new { controller = "SQLCollations" });

            config.Routes.MapHttpRoute(
              name: "SQLVersions",
              routeTemplate: "subscriptions/{subscriptionId}/sqlversions",
              defaults: new { controller = "SQLVersions" });

            config.Routes.MapHttpRoute(
                name: "AdminSQLVersions",
                routeTemplate: "admin/sqlversions",
                defaults: new { controller = "SQLVersions" });

            config.Routes.MapHttpRoute(
              name: "NetworkNICs",
              routeTemplate: "subscriptions/{subscriptionId}/networknics",
              defaults: new { controller = "NetworkNICs" });

            config.Routes.MapHttpRoute(
                name: "AdminNetworkNICs",
                routeTemplate: "admin/networknics",
                defaults: new { controller = "NetworkNICs" });

            config.Routes.MapHttpRoute(
              name: "IISRoleServices",
              routeTemplate: "subscriptions/{subscriptionId}/iissroleservices",
              defaults: new { controller = "IISRoleServices" });

            config.Routes.MapHttpRoute(
                name: "AdminIISRoleServices",
                routeTemplate: "admin/iissroleservices",
                defaults: new { controller = "IISRoleServices" });

            config.Routes.MapHttpRoute(
              name: "SQLAnalysisServicesModes",
              routeTemplate: "subscriptions/{subscriptionId}/sqlanalysisservicesmodes",
              defaults: new { controller = "SQLAnalysisServicesModes" });

            config.Routes.MapHttpRoute(
                name: "AdminSQLAnalysisServicesModes",
                routeTemplate: "admin/sqlanalysisservicesmodes",
                defaults: new { controller = "SQLAnalysisServicesModes" });

            config.Routes.MapHttpRoute(
                name: "NameResolution",
                //  routeTemplate: "nameresolution/{securitygroups}",
                routeTemplate: "subscriptions/{subscriptionId}/nameresolution/{securitygroups}",
                defaults: new { controller = "NameResolution" });

            config.Routes.MapHttpRoute(
                name: "SubmitScriptJob",
                routeTemplate: "subscriptions/{subscriptionId}/ScriptJobs",
                defaults: new { controller = "ScriptJobs" });

            config.Routes.MapHttpRoute(
                name: "ScriptJob",
                routeTemplate: "subscriptions/{subscriptionId}/ScriptJobs/{smaJobId}",
                defaults: new { controller = "ScriptJobs" });

            config.Routes.MapHttpRoute(
               name: "ScriptJobs",
               routeTemplate: "subscriptions/{subscriptionId}/ScriptJobs",
               defaults: new { controller = "ScriptJobs" });

            config.Filters.Add(new Microsoft.WindowsAzurePack.CmpWapExtension.Common.PortalAPIExceptionHandlerAttribute());
        }
    }
}
