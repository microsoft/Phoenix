using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class AzureAdminSubscriptionMapping
    {
        public int Id { get; set; }
        public string SubId { get; set; }
        public string PlanId { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}
