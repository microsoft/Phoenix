//*****************************************************************************
// File: CmpRequest.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for CmpRequest
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class CmpRequest
    {
        public int Id { get; set; }
        public string WapSubscriptionID { get; set; }
        public Nullable<int> CmpRequestID { get; set; }
        public string ParentAppName { get; set; }
        public string TargetVmName { get; set; }
        public string Domain { get; set; }
        public string VmSize { get; set; }
        public string TargetLocation { get; set; }
        public string StatusCode { get; set; }
        public string SourceImageName { get; set; }
        public string SourceServerName { get; set; }
        public string UserSpec { get; set; }
        public string StorageSpec { get; set; }
        public string FeatureSpec { get; set; }
        public string Config { get; set; }
        public string RequestType { get; set; }
        public string WhoRequested { get; set; }
        public Nullable<System.DateTime> WhenRequested { get; set; }
        public string StatusMessage { get; set; }
        public string ExceptionMessage { get; set; }
        public string Warnings { get; set; }
        public Nullable<System.DateTime> LastStatusUpdate { get; set; }
        public Nullable<bool> Active { get; set; }
        public string TagData { get; set; }
        public Nullable<int> TagID { get; set; }
        public string AddressFromVm { get; set; }
        public Nullable<int> AccessGroupId { get; set; }
    }
}
