//***********************************************************************************
// File: ServerRoleDriveMappingsController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class is used to provide Drive Mapping information for a particular
//          server role.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//***********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Diagnostics;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    /// <remarks>
    /// This class is used to provide Drive Mapping information for a particular
    /// server role.
    /// </remarks>
    public class ServerRoleDriveMappingsController : BaseApiController
    {
        public static EventLog EventLog;
        public static List<Models.ServerRoleDriveMapping> DriveMappingList = new List<Models.ServerRoleDriveMapping>();


        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches drive mapping infomration from WAP DB.
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchServerRoleDriveMappingsFromDb()
        {
            var cwdb = new CmpWapDb();
            var list = cwdb.FetchServerRoleDriveInfoList();
            DriveMappingList.Clear();
            DriveMappingList.AddRange(list);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This GET method lists drive mapping information.
        /// </summary>
        /// <returns>list of drive mapping information</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.ServerRoleDriveMapping> ListServerRoleDriveMappings()
        {
            try
            {
                lock (DriveMappingList)
                {
                    if (0 == DriveMappingList.Count)
                        FetchServerRoleDriveMappingsFromDb();
                }
                var result = from map in DriveMappingList                           
                             select map;
                return result.ToList();
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
        ///     This GET method lists drivde mapping information based on 
        ///     subscription Id.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.ServerRoleDriveMapping> ListServerRoleDriveMappings(string subscriptionId)
        {
             
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                var ex = new ArgumentNullException(subscriptionId);
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw ex;
            }
            return ListServerRoleDriveMappings();

        }

       
    }
}
