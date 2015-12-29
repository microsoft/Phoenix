using System.Linq;
using System.Web.Http;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    public class PlansController : ApiController
    {
        private ICmpWapDbAdminRepository AdminRepository { get; set; }
        private ICmpWapDb Repository { get; set; }

        public PlansController()
        {
            AdminRepository = new CmpWapDb();
            Repository = new CmpWapDb();
        }

        [HttpGet]
        public PlanConfiguration GetPlanConfiguration(string planId)
        {
            var oses = AdminRepository.FetchOsInfoList(planId);
            var sizes = AdminRepository.FetchVmSizeInfoList(planId);
            var regions = AdminRepository.FetchAzureRegionList(planId);
            var azureSubscriptions = AdminRepository.FetchAzureSubToPlanMapping(planId);
            
            if (oses == null)
            {
                AdminRepository.SetVmOsByBatch(Repository.FetchOsInfoList(onlyActiveOnes: true), planId);
                oses = AdminRepository.FetchOsInfoList(planId);
            }

            if (sizes == null)
            {
                AdminRepository.SetVmSizeByBatch(Repository.FetchVmSizeInfoList(onlyActiveOnes: true), planId);
                sizes = AdminRepository.FetchVmSizeInfoList(planId);
            }

            if (regions == null)
            {
                AdminRepository.SetAzureRegionByBatch(Repository.FetchAzureRegionList(onlyActiveOnes: true), planId);
                regions = AdminRepository.FetchAzureRegionList(planId);
            }

            return new PlanConfiguration
            {
                OperatingSystems = oses == null ? Enumerable.Empty<PlanSetting<VmOs>>() : oses.Select(kvp => new PlanSetting<VmOs>
                {
                    Option = kvp.Key,
                    IsSelected = kvp.Value,
                }).OrderBy(p => p.Option.AzureImageOffer),

                VmSizes = sizes == null ? Enumerable.Empty<PlanSetting<VmSize>>() : sizes.Select(kvp => new PlanSetting<VmSize>
                {
                    Option = kvp.Key,
                    IsSelected = kvp.Value,
                }).OrderBy(p => p.Option.Name),

                AzureRegions = regions == null ? Enumerable.Empty<PlanSetting<AzureRegion>>() : regions.Select(kvp => new PlanSetting<AzureRegion>
                {
                    Option = kvp.Key,
                    IsSelected = kvp.Value,
                }),
                AzureSubscriptions = azureSubscriptions == null ? Enumerable.Empty<PlanSetting<AzureSubscription>>() : azureSubscriptions.Select(kvp => new PlanSetting<AzureSubscription>
                {
                    Option = kvp.Key,
                    IsSelected = kvp.Value,
                })
            };
        }

        [HttpPost]
        public void SetPlanConfiguration(string planId, ApiClient.DataContracts.PlanConfiguration configuration)
        {
            //Using the IsActive of the new objects not as to activate or deactivate an object's availability,
            //but rather to indicate the status of its mapping.
            var oses = configuration.OperatingSystems.Select(s => new VmOs
            {
                VmOsId = s.Id,
                IsActive = s.IsSelected,
            });
            var sizes = configuration.VmSizes.Select(s => new VmSize
            {
                VmSizeId = s.Id,
                IsActive = s.IsSelected,
            });
            var regions = configuration.AzureRegions.Select(s => new AzureRegion
            {
                AzureRegionId = s.Id,
                IsActive = s.IsSelected,
            });
            var azureSubscriptions = configuration.AzureSubscriptions.Select(s => new AzureSubscription
            {
                Id = s.Id,
                Active = s.IsSelected,
            });

            AdminRepository.SetVmOsByBatch(oses, planId);
            AdminRepository.SetVmSizeByBatch(sizes, planId);
            AdminRepository.SetAzureRegionByBatch(regions, planId);
            AdminRepository.UpdateAzureSubToPlanMapping(azureSubscriptions, planId);
        }
    }
}
