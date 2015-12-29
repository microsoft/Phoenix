using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class TargetRegionMap
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string AzureReqionName { get; set; }
        public string TagData { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Config { get; set; }
        public string Description { get; set; }
    }
}
