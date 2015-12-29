//*****************************************************************************
// File: NetworkNIC.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for NetworkNIC
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************
using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class NetworkNIC
    {
        public NetworkNIC()
        {
            this.ResourceProviderAcctGroups = new List<ResourceProviderAcctGroup>();
        }

        public int NetworkNICId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public string ADDomain { get; set; } // returns ad domain info based on joining between NetworkNIC and AdDomainMap table
        public int ADDomainId { get; set; }
        public virtual ICollection<ResourceProviderAcctGroup> ResourceProviderAcctGroups { get; set; }
    }
}
