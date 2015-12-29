//*****************************************************************************
// File: EnvironmentType.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for EnvironmentType
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class EnvironmentType
    {
        public EnvironmentType()
        {
            //this.ResourceProviderAcctGroups = new List<ResourceProviderAcctGroup>();
        }

        public int EnvironmentTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        //public virtual ICollection<ResourceProviderAcctGroup> ResourceProviderAcctGroups { get; set; }
    }
}
