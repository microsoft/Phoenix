using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    public class VmRegionSizeMappingValidationController : BaseApiController
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

        private bool GetVmRegionSizeMappingValidationFromDb(int regionId, int sizeId)
        {
            var cwdb = new CmpWapDb();
            bool result = cwdb.GetRegionVmSizeMappings(regionId, sizeId);
            return result;
        }

        [HttpPost]
        public bool GetVmRegionSizeMappingValidation([FromBody]int[] RegionSizeIds)
        {
            if ((RegionSizeIds == null) || (RegionSizeIds.Length == 0))
            {
                var ex = new ArgumentException("RegionId or SizeId is empty/invalid");
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw ex;
            }
            return GetVmRegionSizeMappingValidationFromDb(RegionSizeIds[0], RegionSizeIds[1]);

        }
    }
}
