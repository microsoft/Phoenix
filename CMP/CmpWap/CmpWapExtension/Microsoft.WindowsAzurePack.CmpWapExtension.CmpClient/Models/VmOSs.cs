using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class VmOSs
    {
        public int VMOsId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OsFamily { get; set; }
        public string AzureImageName { get; set; }
        public bool IsCustomImage { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string AzureImagePublisher { get; set; }
        public string AzureImageOffer { get; set; }
        public string AzureWindowsOSVersion { get; set; }
    }
}
