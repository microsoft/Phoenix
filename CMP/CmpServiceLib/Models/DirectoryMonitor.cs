using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class DirectoryMonitor
    {
        public int MonitorID { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string StorageAccountKey { get; set; }
        public string TransferType { get; set; }
        public string SourceType { get; set; }
        public string DestinationType { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string Config { get; set; }
        public string WhoRequested { get; set; }
        public System.DateTime WhenRequested { get; set; }
        public Nullable<bool> Enabled { get; set; }
        public string ResultStatusCode { get; set; }
        public string ResultDescription { get; set; }
    }
}
