using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class TodoItem
    {
        public string id { get; set; }
        public System.DateTimeOffset C__createdAt { get; set; }
        public Nullable<System.DateTimeOffset> C__updatedAt { get; set; }
        public byte[] C__version { get; set; }
        public string text { get; set; }
        public Nullable<bool> complete { get; set; }
    }
}
