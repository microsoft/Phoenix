using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class AzureAdminSubscriptionVmOsMapping
    {
        public int Id { get; set; }
        public int VmOsId { get; set; }
        public string SubscriptionId { get; set; }
        public bool IsActive { get; set; }
    }
}
