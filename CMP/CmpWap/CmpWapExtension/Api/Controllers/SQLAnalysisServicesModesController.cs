//*****************************************************************************
// File: SQLAnalysisServicesModesController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class is used to provide various modes of SQL Server Analysis Services.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Diagnostics;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    /// <remarks>
    ///     This class is used to provide various modes of SQL Server Analysis Services.
    /// </remarks>
    public class SQLAnalysisServicesModesController : BaseApiController
    {

        public static List<Models.SQLAnalysisServicesMode> AppList = new List<Models.SQLAnalysisServicesMode>();


        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to fetch SQL Server Analysis services modes
        ///     from WAP DB.
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchSQLAnalysisServicesModeFromDb()
        {
            var cwdb = new CmpWapDb();
            var list = cwdb.FetchSqlAnalysisServicesModeInfoList();
            AppList.Clear();
            AppList.AddRange(list);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This GET method is used to list SQL Server Analysis Services modes.
        /// </summary>
        /// <returns>list of SQL server analysis services modes</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.SQLAnalysisServicesMode> ListApps()
        {
            try
            {
                lock (AppList)
                {
                    if (0 == AppList.Count)
                        FetchSQLAnalysisServicesModeFromDb();
                }
                return AppList;
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
        ///     This GET method is used to list SQL Server Analysis Services modes
        ///     based on WAP subscription id.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns>list of SQL server analysis services modes</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.SQLAnalysisServicesMode> ListApps(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                var ex = new ArgumentNullException(subscriptionId);
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw ex;
            }
            return ListApps();
        }
    }
}
