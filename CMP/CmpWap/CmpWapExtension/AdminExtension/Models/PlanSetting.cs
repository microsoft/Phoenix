using Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models.Interfaces;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models
{
    public class PlanSetting : IPlanSetting
    {
        public int Id { get; set; }

        public bool IsSelected { get; set; }

        public string Name { get; set; }
    }
}