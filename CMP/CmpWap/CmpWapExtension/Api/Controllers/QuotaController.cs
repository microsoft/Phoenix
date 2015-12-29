//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    public class QuotaController : ApiController
    {
        [HttpGet]
        public List<string> GetDefaultQuota()
        {
            return new List<string>();
        }
    }
}