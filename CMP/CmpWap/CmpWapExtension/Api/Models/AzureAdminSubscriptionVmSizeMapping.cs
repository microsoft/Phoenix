//*****************************************************************************
// File: AzureAdminSubscriptionVmSizeMapping.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for VM Size mapping class to a Plan ID
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class AzureAdminSubscriptionVmSizeMapping
    {
        public int Id { get; set; }
        public int VmSizeId { get; set; }
        public string PlanId { get; set; }
        public bool IsActive { get; set; }
    }
}
