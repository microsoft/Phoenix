//*****************************************************************************
// File: SequenceRequest.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for SequenceRequest
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************
using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class SequenceRequest
    {
        public int Id { get; set; }
        public string WapSubscriptionID { get; set; }
        public Nullable<int> CmpRequestID { get; set; }
        public string ServiceProviderName { get; set; }
        public string ServiceProviderTypeCode { get; set; }
        public string ServiceProviderJobId { get; set; }
        public string TargetName { get; set; }
        public string TargetTypeCode { get; set; }
        public string StatusCode { get; set; }
        public string Config { get; set; }
        public string WhoRequested { get; set; }
        public Nullable<System.DateTime> WhenRequested { get; set; }
        public string StatusMessage { get; set; }
        public string ExceptionMessage { get; set; }
        public string Warnings { get; set; }
        public Nullable<System.DateTime> LastStatusUpdate { get; set; }
        public Nullable<bool> Active { get; set; }
        public string TagOpName { get; set; }
        public string TagData { get; set; }
        public Nullable<int> TagID { get; set; }
    }
}
