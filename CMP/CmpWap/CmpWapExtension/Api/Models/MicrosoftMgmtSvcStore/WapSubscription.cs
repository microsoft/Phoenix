//*****************************************************************************
// File: WapSubscription.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for WapSubscription
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.MicrosoftMgmtSvcStore
{
    public partial class WapSubscription
    {
        public Int64 Id { get; set; }
        public string SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public Int64 UserId { get; set; }
        public Int64 PlanId { get; set; }
    }
}
