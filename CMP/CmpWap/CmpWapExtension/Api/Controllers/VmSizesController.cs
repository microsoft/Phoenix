//*****************************************************************************
// File: VmSizesController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class contains methods that is use dto provide VM size related
//          information.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Diagnostics;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    /// <remarks>
    /// This class contains methods that is use dto provide VM size related
    /// information.
    /// </remarks>
    public class VmSizesController : BaseApiController
    {
        public static EventLog EventLog;
        public static List<Models.VmSize> SizeList = new List<Models.VmSize>();


        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches VM size information from WAP DB.
        /// </summary>
        /// 
        /// All Azure sizes -> https://msdn.microsoft.com/en-us/library/azure/dn197896.aspx
        /// 
        //*********************************************************************

        private void FetchVmSizeFromDb()
        {
            var cwdb = new CmpWapDb();
            var list = cwdb.FetchVmSizeInfoList(onlyActiveOnes: true);
            SizeList.Clear();
            SizeList.AddRange(list);
        }


        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches VM size information from WAP DB for the corresponding subscriptionid.
        /// </summary>
        /// 
        /// All Azure sizes -> https://msdn.microsoft.com/en-us/library/azure/dn197896.aspx
        /// 
        //*********************************************************************

        private void FetchVmSizeFromDb(string subscriptionId)
        {
            var cwdb = new CmpWapDb();

            if (!string.IsNullOrEmpty(subscriptionId))
            {
                var repository = cwdb as ICmpWapDbTenantRepository;
                var list = repository.FetchVmSizeInfoList(subscriptionId);
                SizeList.Clear();
                SizeList.AddRange(list);
            }

        }
        //*********************************************************************
        ///
        /// <summary>
        ///     This GET method lists VM sizes.
        /// </summary>
        /// <returns>list of VM size</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.VmSize> ListVmSizes()
        {
            try
            {
                lock (SizeList)
                {
                    if (0 == SizeList.Count)
                        FetchVmSizeFromDb();
                }
                return SizeList;
            }
            catch (Exception ex)
            {
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This GET method lists VM sizes based on subscription Id.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns>list of VM sizes</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.VmSize> ListVmSizes(string subscriptionId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subscriptionId))
                {
                    var ex = new ArgumentNullException(subscriptionId);
                    Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                    throw ex;
                }

                lock (SizeList)
                {
                    FetchVmSizeFromDb(subscriptionId);
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw;
            }

            return SizeList;
        }

        
    }
}
