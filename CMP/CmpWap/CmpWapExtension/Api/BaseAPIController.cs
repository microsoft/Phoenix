using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api
{
    public class BaseApiController:ApiController
    {
        public static ILogger Logger = new Logger();
    }
}
