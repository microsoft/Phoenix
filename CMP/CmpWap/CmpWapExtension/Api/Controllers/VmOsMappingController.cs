using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    public class VmOsMappingController : BaseApiController
    {

        public static EventLog EventLog;
        public static List<Models.AzureRegionVmOsMapping> osMapping = new List<Models.AzureRegionVmOsMapping>();

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches os mapping to subscription information from WAP DB.
        /// </summary>
        /// 
        //*********************************************************************

        private bool GetOSSubscriptionMappingsFromDb(int regionId, int osId)
        {
            var cwdb = new CmpWapDb();
            bool result = cwdb.GetOSMappedToWapSubscription(regionId, osId);
            return result;
        }

        [HttpPost]
        public bool GetOSSubscriptionMappings([FromBody]int[] regionId)
        {
            if (regionId == null)
            {
                var ex = new ArgumentException("RegionId is invalid");
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw ex;
            }
            return GetOSSubscriptionMappingsFromDb(regionId[0], regionId[1]);

        }

    }
}
