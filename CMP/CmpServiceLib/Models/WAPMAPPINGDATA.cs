using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class WAPMAPPINGDATA
    {
        public int Id { get; set; }
        public string WapSubscriptionID { get; set; }
        public string TargetVmName { get; set; }
        public string AdminUser { get; set; }
    }
}
