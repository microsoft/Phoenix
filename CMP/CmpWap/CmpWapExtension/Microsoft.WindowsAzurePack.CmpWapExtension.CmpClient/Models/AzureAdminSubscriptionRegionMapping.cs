using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class AzureAdminSubscriptionRegionMapping
    {
        public int Id { get; set; }
        public string SubId { get; set; }
        public int AzureRegionId { get; set; }
    }
}
