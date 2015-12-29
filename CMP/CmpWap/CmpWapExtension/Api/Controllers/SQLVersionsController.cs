//*****************************************************************************
// File: SQLVersionsController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class is used to proivide SQL versions related information.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    /// <remarks>
    ///     This class is used to proivide SQL versions related information.
    /// </remarks>
    public class SQLVersionsController : BaseApiController
    {
        public static EventLog EventLog;
        public static List<Models.SQLVersion> VersionList = new List<Models.SQLVersion>();


        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to fetch SQL version infromation from WAP DB.
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchSQLVersionFromDb()
        {
            var cwdb = new CmpWapDb();
            var list = cwdb.FetchSqlVersionInfoList();
            VersionList.Clear();
            VersionList.AddRange(list);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This GET method lists SQL version information
        /// </summary>
        /// <returns>list of SQL version</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.SQLVersion> ListSQLVersions()
        {
            try
            {
                lock (VersionList)
                {
                    if (0 == VersionList.Count)
                        FetchSQLVersionFromDb();
                }
                return VersionList;
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
        ///     This GET method lists SQL version information based on WAP
        ///     subscription Id.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns>list of SQL version</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.SQLVersion> ListSQLVersions(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                var ex = new ArgumentNullException(subscriptionId);
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw ex;
            }
            return ListSQLVersions();
        }

       
    }
}
