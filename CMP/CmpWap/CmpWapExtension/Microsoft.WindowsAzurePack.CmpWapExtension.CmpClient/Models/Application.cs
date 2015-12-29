using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class Application
    {
        public int ApplicationId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool HasService { get; set; }
        public string CIOwner { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string SubscriptionId { get; set; }
    }
}
