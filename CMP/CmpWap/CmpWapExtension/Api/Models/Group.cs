//*****************************************************************************
// File: Group.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for Group
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class Group
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public string Config { get; set; }
        public string TagData { get; set; }
        public Nullable<int> TagId { get; set; }
        public bool IsActive { get; set; }
    }
}
