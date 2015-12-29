using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class vw_MigrationStatusByDay
    {
        public Nullable<System.DateTime> StartDatePST { get; set; }
        public string Status { get; set; }
        public Nullable<int> NumberOfBuilds { get; set; }
        public string AvgCompleteDuration { get; set; }
        public string MaxCompleteDuration { get; set; }
        public string MinCompleteDuration { get; set; }
    }
}
