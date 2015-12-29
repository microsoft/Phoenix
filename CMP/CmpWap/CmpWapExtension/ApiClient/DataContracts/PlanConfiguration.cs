using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
    public class PlanConfiguration
    {
        public IEnumerable<PlanSetting> OperatingSystems { get; set; }
        public IEnumerable<PlanSetting> VmSizes { get; set; }
        public IEnumerable<PlanSetting> AzureRegions { get; set; }
        public IEnumerable<PlanSetting> AzureSubscriptions { get; set; }
    }
}
