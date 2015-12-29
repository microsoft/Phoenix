using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class IpakVersionMap
    {
        public int Id { get; set; }
        public string VersionCode { get; set; }
        public string VersionName { get; set; }
        public string AzureRegion { get; set; }
        public string AdDomain { get; set; }
        public string Config { get; set; }
        public string TagData { get; set; }
        public string IpakDirLocation { get; set; }
        public string IpakFullFileLocation { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string AdminName { get; set; }
        public string QfeVersion { get; set; }
    }
}
