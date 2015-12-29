using System;
using System.Collections.Generic;

namespace CmpServiceLib.Models
{
    public partial class vw_AzureLocalComputerDetails
    {
        public int ID { get; set; }
        public string ServerName { get; set; }
        public string LocalAdminUsername { get; set; }
        public string LocalAdminPassword { get; set; }
    }
}
