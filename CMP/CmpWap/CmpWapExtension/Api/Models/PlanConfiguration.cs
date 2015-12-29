using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public class PlanConfiguration
    {
        public IEnumerable<PlanSetting<VmOs>> OperatingSystems { get; set; }
        public IEnumerable<PlanSetting<VmSize>> VmSizes { get; set; }
        public IEnumerable<PlanSetting<AzureRegion>> AzureRegions { get; set; }
        public IEnumerable<PlanSetting<AzureSubscription>> AzureSubscriptions { get; set; }    
    }
}
