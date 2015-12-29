using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class AzureRoleSize
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CoreCount { get; set; }
        public int DiskCount { get; set; }
        public double RamMb { get; set; }
        public int DiskSizeRoleOs { get; set; }
        public double DiskSizeRoleApps { get; set; }
        public int DiskSizeVmOs { get; set; }
        public int DiskSizeVmTemp { get; set; }
        public bool CanBeService { get; set; }
        public bool CanBeVm { get; set; }
    }
}
