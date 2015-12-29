using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class MonitoredFileInfo
    {
        public int FileInfoID { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public System.DateTime LastWriteTime { get; set; }
        public int Size { get; set; }
    }
}
