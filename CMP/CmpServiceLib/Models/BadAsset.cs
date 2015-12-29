using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class BadAsset
    {
        public int Id { get; set; }
        public string AssetName { get; set; }
        public string AssetTypeCode { get; set; }
        public string ProblemDescription { get; set; }
        public string WhoReported { get; set; }
        public Nullable<System.DateTime> WhenReported { get; set; }
        public string Config { get; set; }
        public string TagData { get; set; }
        public bool Active { get; set; }
    }
}
