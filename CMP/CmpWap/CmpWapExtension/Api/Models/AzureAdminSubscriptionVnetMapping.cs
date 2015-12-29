//*****************************************************************************
// File: AzureAdminSubscriptionVnetMapping.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for AzureAdminSubscription Vnet Mapping
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class AzureAdminSubscriptionVnetMapping
    {
        public int Id { get; set; }
        public string PlanId { get; set; }
        public int VnetId { get; set; }
    }
}
