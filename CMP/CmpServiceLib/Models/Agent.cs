using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class Agent
    {
        public int ID { get; set; }
        public string HostName { get; set; }
        public string Region { get; set; }
        public Nullable<System.DateTime> CheckInTime { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public Nullable<int> TransferLoad { get; set; }
        public Nullable<int> MaxSupportedLoad { get; set; }
        public string AgentConfig { get; set; }
        public string SystemConfig { get; set; }
        public Nullable<bool> Enabled { get; set; }
    }
}
