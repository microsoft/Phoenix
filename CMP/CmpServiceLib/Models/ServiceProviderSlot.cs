using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class ServiceProviderSlot
    {
        public int Id { get; set; }
        public Nullable<int> ServiceProviderAccountId { get; set; }
        public string TypeCode { get; set; }
        public string ServiceProviderSlotName { get; set; }
        public string Description { get; set; }
        public string TagData { get; set; }
        public Nullable<int> TagInt { get; set; }
        public Nullable<bool> Active { get; set; }
    }
}
