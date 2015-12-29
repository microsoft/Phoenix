using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class vw_DeploymentRequestExecutionTimes
    {
        public Nullable<int> RequestID { get; set; }
        public string RequestName { get; set; }
        public string RoleName { get; set; }
        public string ExecutionDuration { get; set; }
        public Nullable<System.DateTime> StartTimePST { get; set; }
        public Nullable<System.DateTime> EndTimePST { get; set; }
        public string ExecutionStatus { get; set; }
        public string LastExecutionStep { get; set; }
        public string Message { get; set; }
    }
}
