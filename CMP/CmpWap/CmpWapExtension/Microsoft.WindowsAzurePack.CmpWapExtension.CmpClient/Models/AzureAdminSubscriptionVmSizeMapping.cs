using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class AzureAdminSubscriptionVmSizeMapping
    {
        public int Id { get; set; }
        public int VmSizeId { get; set; }
        public string SubscriptionId { get; set; }
        public bool IsActive { get; set; }
    }
}
