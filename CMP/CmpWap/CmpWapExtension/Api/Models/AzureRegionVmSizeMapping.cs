//*****************************************************************************
// File: AzureRegionVmSizeMapping.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for Azure Region Mapping to a VM Size
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class AzureRegionVmSizeMapping
    {
        public int Id { get; set; }
        public int VmSizeId { get; set; }
        public int AzureRegionId { get; set; }
        public bool IsActive { get; set; }
    }
}
