//*****************************************************************************
// File: AdDomainMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for domain mapping
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class AdDomainMap
    {
        public AdDomainMap()
        {
            this.ResourceProviderAcctGroups = new List<ResourceProviderAcctGroup>();
        }

        public int Id { get; set; }
        public string DomainShortName { get; set; }
        public string DomainFullName { get; set; }
        public string JoinCredsUserName { get; set; }
        public string JoinCredsPasword { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Config { get; set; }
        public string ServerOU { get; set; }
        public string WorkstationOU { get; set; }
        public string DefaultVmAdminMember { get; set; }
        public virtual ICollection<ResourceProviderAcctGroup> ResourceProviderAcctGroups { get; set; }
    }
}
