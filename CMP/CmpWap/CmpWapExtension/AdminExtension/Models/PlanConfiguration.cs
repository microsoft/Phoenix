using System.Collections.Generic;
using Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.Interfaces;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models
{
    public class PlanConfiguration : IApiMapper<ApiClient.DataContracts.PlanConfiguration>
    {
        public IEnumerable<PlanSetting> OperatingSystems { get; set; }

        public IEnumerable<PlanSetting> VmSizes { get; set; }

        public IEnumerable<PlanSetting> AzureRegions { get; set; }

        public IEnumerable<PlanSetting> AzureSubscriptions { get; set; }

        public ApiClient.DataContracts.PlanConfiguration ToApiObject()
        {
            var apiObject = new ApiClient.DataContracts.PlanConfiguration();
            foreach (var property in apiObject.GetType().GetProperties())
            {
                var list = GetType().GetProperty(property.Name).GetValue(this) as IEnumerable<PlanSetting>;
                if (list == null) continue;

                var apiList = new List<ApiClient.DataContracts.PlanSetting>();
                foreach (var value in list)
                {
                    apiList.Add(new ApiClient.DataContracts.PlanSetting
                    {
                        Id = value.Id,
                        IsSelected = value.IsSelected,
                        Name = value.Name,
                    });
                }
                property.SetValue(apiObject, apiList);
            }

            return apiObject;
        }
    }
}