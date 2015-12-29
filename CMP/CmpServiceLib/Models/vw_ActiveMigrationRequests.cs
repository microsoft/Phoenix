using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class vw_ActiveMigrationRequests
    {
        public int ID { get; set; }
        public int VmDeploymentRequestID { get; set; }
        public string VmSize { get; set; }
        public string TagData { get; set; }
        public Nullable<int> TagID { get; set; }
        public string Config { get; set; }
        public string TargetVmName { get; set; }
        public string SourceServerName { get; set; }
        public string SourceVhdFilesCSV { get; set; }
        public string ExceptionMessage { get; set; }
        public Nullable<System.DateTime> LastStatusUpdate { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string AgentRegion { get; set; }
        public string AgentName { get; set; }
        public Nullable<System.DateTime> CurrentStateStartTime { get; set; }
        public Nullable<int> CurrentStateTryCount { get; set; }
        public string Warnings { get; set; }
        public Nullable<bool> Active { get; set; }
    }
}
