using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class vw_CMPDailyMailCompletedBuilds
    {
        public int RowOrder { get; set; }
        public Nullable<System.DateTime> StartDatePST { get; set; }
        public string RoleName { get; set; }
        public Nullable<int> CompletedBuilds { get; set; }
        public string AvgCompleteDuration { get; set; }
        public string MaxCompleteDuration { get; set; }
        public string MinCompleteDuration { get; set; }
    }
}
