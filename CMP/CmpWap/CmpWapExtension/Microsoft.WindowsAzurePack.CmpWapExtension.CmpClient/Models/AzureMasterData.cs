using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class AzureMasterData
    {
        public int Id { get; set; }
        public string OsName { get; set; }
        public string RegionName { get; set; }
        public string VnetName { get; set; }
        public string VmSize { get; set; }
        public string StorageType { get; set; }
        public string Description { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
