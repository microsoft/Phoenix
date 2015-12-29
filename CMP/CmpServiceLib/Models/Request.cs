using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class Request
    {
        public int RequestID { get; set; }
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
        public Nullable<System.DateTime> TransferStartTime { get; set; }
        public Nullable<int> ElapsedTimeMinutes { get; set; }
        public Nullable<int> MBytesTransferred { get; set; }
        public string ResultStatusCode { get; set; }
        public string ResultDescription { get; set; }
        public Nullable<System.DateTime> ResultTime { get; set; }
        public Nullable<bool> WillTryAgain { get; set; }
        public Nullable<int> SourceMBytes { get; set; }
        public Nullable<int> RateBytesSec { get; set; }
        public Nullable<int> TagID { get; set; }
        public string TagData { get; set; }
        public string AgentRegion { get; set; }
        public string AgentName { get; set; }
        public Nullable<bool> DeleteSourceAfterTransfer { get; set; }
        public Nullable<bool> OverwriteDestinationBlob { get; set; }
        public Nullable<bool> Active { get; set; }
    }
}
