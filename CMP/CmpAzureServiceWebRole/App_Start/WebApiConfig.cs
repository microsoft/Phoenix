using System.Web.Http;
using System.Web.Http.OData.Builder;
using CmpAzureServiceWebRole.Models;
using VmDeploymentRequest = CmpInterfaceModel.Models.VmDeploymentRequest;
using VmMigrationRequest = CmpInterfaceModel.Models.VmMigrationRequest;

namespace CmpAzureServiceWebRole
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            /*config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );*/

            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<VmDeploymentRequest>("VmDeployments");
            builder.EntitySet<VmMigrationRequest>("VmMigrations");
            builder.EntitySet<Reboot>("Reboots");

            // New code: Add an action to the EDM, and define the parameter and return type.
            var reboot = builder.Entity<Reboot>().Action("Reboot");
            reboot.Parameter<int>("Param");
            reboot.Returns<int>();

            config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}
