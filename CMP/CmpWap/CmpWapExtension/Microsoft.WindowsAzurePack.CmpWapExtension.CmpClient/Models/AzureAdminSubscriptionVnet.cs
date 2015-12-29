using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class AzureAdminSubscriptionVnet
    {
        public int Id { get; set; }
        public string SubId { get; set; }
        public string Subnet { get; set; }
        public string Gateway { get; set; }
        public string CircuitName { get; set; }
        public string VNetType { get; set; }
        public string VNetName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
