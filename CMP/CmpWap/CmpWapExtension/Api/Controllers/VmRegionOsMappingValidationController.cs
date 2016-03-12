using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    public class VmRegionOsMappingValidationController : BaseApiController
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

        private bool GetRegionOSSubscriptionMappingValidationFromDb(int regionId, int osId)
        {
            var cwdb = new CmpWapDb();
            bool result = cwdb.GetRegionVmOSMappings(regionId, osId);
            return result;
        }


        [HttpPost]
        public bool GetRegionOSSubscriptionMappingValidation([FromBody]int[] RegionOSIds)
        {
            if ((RegionOSIds == null) || (RegionOSIds.Length == 0))
            {
                var ex = new ArgumentException("RegionId or OsId is empty/invalid");
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw ex;
            }
            return GetRegionOSSubscriptionMappingValidationFromDb(RegionOSIds[0], RegionOSIds[1]);

        }

    }
}
