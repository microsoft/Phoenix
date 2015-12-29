//*****************************************************************************
// File: VmOs.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for VmOs
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Interfaces;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    using System;

    public partial class VmOs : IPlanOption
    {
        public int VmOsId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OsFamily { get; set; }
        public string AzureImageName { get; set; }
        public bool IsCustomImage { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string AzureImagePublisher { get; set; }
        public string AzureImageOffer { get; set; }
        public string AzureWindowsOSVersion { get; set; }

        public int Id
        {
            get { return VmOsId; }
        }
    }
}
