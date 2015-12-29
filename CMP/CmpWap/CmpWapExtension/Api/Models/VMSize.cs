//*****************************************************************************
// File: VmSize.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for VmSize
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************
using System;
using System.Collections.Generic;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Interfaces;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class VmSize : IPlanOption
    {
        public int VmSizeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cores { get; set; }
        public int Memory { get; set; }
        public int MaxDataDiskCount { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }

        public int Id
        {
            get { return VmSizeId; }
        }
    }
}
