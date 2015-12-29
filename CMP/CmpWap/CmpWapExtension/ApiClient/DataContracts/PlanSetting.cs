using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts.Interfaces;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
    public class PlanSetting : IPlanSetting
    {
        public int Id { get; set; }

        public bool IsSelected { get; set; }

        public string Name { get; set; }
    }
}