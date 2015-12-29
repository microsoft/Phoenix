using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class OpRequest
    {
        public int Id { get; set; }
        public string RequestName { get; set; }
        public string RequestDescription { get; set; }
        public string RequestType { get; set; }
        public string Config { get; set; }
        public string TargetTypeCode { get; set; }
        public string TargetName { get; set; }
        public string WhoRequested { get; set; }
        public Nullable<System.DateTime> WhenRequested { get; set; }
        public string ExceptionMessage { get; set; }
        public Nullable<System.DateTime> LastStatusUpdate { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string Warnings { get; set; }
        public string ServiceProviderStatusCheckTag { get; set; }
        public Nullable<System.DateTime> CurrentStateStartTime { get; set; }
        public Nullable<int> CurrentStateTryCount { get; set; }
        public string TagData { get; set; }
        public Nullable<int> TagID { get; set; }
        public Nullable<bool> Active { get; set; }
    }
}
