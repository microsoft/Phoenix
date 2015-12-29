//*****************************************************************************
// File: ServerRoleDriveMapping.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for ServerRoleDriveMapping
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class ServerRoleDriveMapping
    {
        public int Id { get; set; }
        public int ServerRoleId { get; set; }
        public string Drive { get; set; }
        public int MemoryInGB { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
