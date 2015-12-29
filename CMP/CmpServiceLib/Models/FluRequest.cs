using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class FluRequest
    {
        public int ID { get; set; }
        public string RequestName { get; set; }
        public string RequestDescription { get; set; }
        public string ParentAppName { get; set; }
        public string TargetVmName { get; set; }
        public string SourceServerName { get; set; }
        public string SourceVhdFilesCSV { get; set; }
        public string TargetLocation { get; set; }
        public string WhoRequested { get; set; }
        public Nullable<System.DateTime> WhenRequested { get; set; }
        public string ExceptionMessage { get; set; }
        public Nullable<System.DateTime> LastStatusUpdate { get; set; }
        public string TagData { get; set; }
        public string Status { get; set; }
        public string VmSize { get; set; }
        public string TargetLocationType { get; set; }
        public Nullable<bool> Active { get; set; }
    }
}
