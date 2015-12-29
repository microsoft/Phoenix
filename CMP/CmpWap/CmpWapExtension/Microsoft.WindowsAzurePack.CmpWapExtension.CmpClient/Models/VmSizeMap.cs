using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class VmSizeMap
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string AzureSizeName { get; set; }
        public string TagData { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Config { get; set; }
        public string Description { get; set; }
        public Nullable<int> CpuCoreCount { get; set; }
        public Nullable<int> RamMB { get; set; }
        public Nullable<int> DiskSizeOS { get; set; }
        public Nullable<int> DiskSizeTemp { get; set; }
        public Nullable<int> DataDiskCount { get; set; }
    }
}
