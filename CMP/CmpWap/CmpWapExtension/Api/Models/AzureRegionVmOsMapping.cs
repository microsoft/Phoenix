//*****************************************************************************
// File: AzureRegionVmOsMapping.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for Azure Region Mapping to a VM Size
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class AzureRegionVmOsMapping
    {
        public int Id { get; set; }
        public int AzureRegionId { get; set; }
        public int VmOsId { get; set; }
        public string AzureSubscriptionId { get; set; }
        public bool IsActive { get; set; }
    }
}
