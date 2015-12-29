//*****************************************************************************
// File: AzureAdminSubscriptionVmOsMapping.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for Operating Systems
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class AzureAdminSubscriptionVmOsMapping
    {
        public int Id { get; set; }
        public int VmOsId { get; set; }
        public string PlanId { get; set; }
        public bool IsActive { get; set; }
    }
}
