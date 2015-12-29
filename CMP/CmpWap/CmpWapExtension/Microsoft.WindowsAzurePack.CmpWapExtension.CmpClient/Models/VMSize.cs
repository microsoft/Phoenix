using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class VMSize
    {
        public int VMSizeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cores { get; set; }
        public int Memory { get; set; }
        public int MaxDataDiskCount { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
