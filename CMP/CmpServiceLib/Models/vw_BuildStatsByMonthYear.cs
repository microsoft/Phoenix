using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class vw_BuildStatsByMonthYear
    {
        public string ExecutionStatus { get; set; }
        public string MonthYear { get; set; }
        public Nullable<int> CountOfSvrs { get; set; }
    }
}
