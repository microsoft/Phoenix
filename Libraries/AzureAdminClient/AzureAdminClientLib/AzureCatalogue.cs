//*****************************************************************************
// File: AzureCatalogue.cs
// Project: AzureAdminClientLib.AzureCatalogue
// Purpose: Encapsulating model to show which VM Sizes and OSes are available
// for a particular region.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************
using System.Collections.Generic;

namespace AzureAdminClientLib
{
    public class AzureCatalogue
    {
        public string RegionId { get; set; }      
        public string RegionName { get; set; }
        public string RegionDisplayName { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public IEnumerable<AzureVmSizeArmData> VmSizes { get; set; }
        public IEnumerable<AzureVmOsArmData> VmOses { get; set; }

        public AzureCatalogue(string regionId, string regionName, string regionDisplayName, string longitude, string latitude)
        {
            RegionId = regionId;
            RegionName = regionName;
            RegionDisplayName = regionDisplayName;
            Longitude = longitude;
            Latitude = latitude;
            VmSizes = new List<AzureVmSizeArmData>();
            VmOses = new List<AzureVmOsArmData>();
        }
    }
}
