using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class Config
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Region { get; set; }
        public string Instance { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}
