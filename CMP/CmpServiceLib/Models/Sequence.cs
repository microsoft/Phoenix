using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class Sequence
    {
        public int Id { get; set; }
        public string SequenceName { get; set; }
        public Nullable<int> SequenceOrder { get; set; }
        public string Config { get; set; }
        public string TagData { get; set; }
    }
}
