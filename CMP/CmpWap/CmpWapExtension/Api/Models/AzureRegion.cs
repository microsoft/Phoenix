//*****************************************************************************
// File: AzureRegion.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for Azure region
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Interfaces;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class AzureRegion : IPlanOption
    {
        public int AzureRegionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OsImageContainer { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }

        public int Id
        {
            get { return AzureRegionId; }
        }
    }
}
