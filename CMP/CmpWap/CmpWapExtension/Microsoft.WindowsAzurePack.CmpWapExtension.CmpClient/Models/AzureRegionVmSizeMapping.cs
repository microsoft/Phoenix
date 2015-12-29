using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class AzureRegionVmSizeMapping
    {
        public int Id { get; set; }
        public int VmSizeId { get; set; }
        public int AzureRegionId { get; set; }
        public bool IsActive { get; set; }
    }
}
