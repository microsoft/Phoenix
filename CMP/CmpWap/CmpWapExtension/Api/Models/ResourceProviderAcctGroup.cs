//*****************************************************************************
// File: ResourceProviderAcctGroup.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for ResourceProviderAcctGroup
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************
using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class ResourceProviderAcctGroup
    {
        public int ResourceProviderAcctGroupId { get; set; }
        public string Name { get; set; }
        public int DomainId { get; set; }
        public int NetworkNICId { get; set; }
        public int EnvironmentTypeId { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public virtual AdDomainMap AdDomainMap { get; set; }
        public virtual EnvironmentType EnvironmentType { get; set; }
        public virtual NetworkNIC NetworkNIC { get; set; }
    }
}
