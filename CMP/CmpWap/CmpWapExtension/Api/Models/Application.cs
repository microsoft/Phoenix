//*****************************************************************************
// File: Application.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for the Application associated with a VM.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class Application
    {
        public int ApplicationId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool HasService { get; set; }
        public string CIOwner { get; set; }
        public bool IsActive { get; set; }
        public string SubscriptionId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
