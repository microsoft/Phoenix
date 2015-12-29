using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class ChangeLog
    {
        public int ID { get; set; }
        public Nullable<int> RequestID { get; set; }
        public Nullable<System.DateTime> When { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public string TagData { get; set; }
        public string ConfigFrom { get; set; }
        public string ConfigTo { get; set; }
        public string Who { get; set; }
    }
}
