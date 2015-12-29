using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class AzureAdminSubscriptionVnetMapping
    {
        public int Id { get; set; }
        public string SubId { get; set; }
        public int VnetId { get; set; }
    }
}
