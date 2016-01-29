using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class Container
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public Nullable<bool> HasService { get; set; }
        public string CIOwner { get; set; }
        public bool IsActive { get; set; }
        public string SubscriptionId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Region { get; set; }
        public string Path { get; set; }
        public string Config { get; set; }
    }
}
