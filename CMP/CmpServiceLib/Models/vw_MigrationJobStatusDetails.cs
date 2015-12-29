using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class vw_MigrationJobStatusDetails
    {
        public int CMPRequestID { get; set; }
        public string VMName { get; set; }
        public string StatusCode { get; set; }
        public string VMSourceName { get; set; }
        public string AD_Domain { get; set; }
        public string Org_Division { get; set; }
        public string Status { get; set; }
        public string Application_Name { get; set; }
        public string Application_ID { get; set; }
        public string Requestor { get; set; }
        public string Status_Message { get; set; }
        public string ValidationResult { get; set; }
        public string Azure_IP_Address { get; set; }
        public Nullable<System.DateTime> StartTimePST { get; set; }
        public Nullable<int> Submitted_Elapsed_Time { get; set; }
        public Nullable<System.DateTime> Last_Update { get; set; }
        public Nullable<int> Last_Update_Elapsed_Time { get; set; }
        public string Exception { get; set; }
        public string Warnings { get; set; }
    }
}
