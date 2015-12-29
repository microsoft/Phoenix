//*****************************************************************************
// File: SQLCollationsController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class is use to provide SQL collation related information.
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
    ///     This class is use to provide SQL collation related information.
    /// </remarks>
    public class SQLCollationsController : BaseApiController
    {
        public static EventLog EventLog;
        public static List<Models.SQLCollation> Collations = new List<Models.SQLCollation>();


        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to fetch SQL collation information from WAP DB.
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchSQLCollationFromDb()
        {
            var cwdb = new CmpWapDb();
            var list = cwdb.FetchSqlCollationInfoList();
            Collations.Clear();
            Collations.AddRange(list);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This GET method is used to list SQL collation information.
        /// </summary>
        /// <returns>list of SQL collation</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.SQLCollation> ListSQLCollations()
        {
            try
            {
                lock (Collations)
                {
                    if (0 == Collations.Count)
                        FetchSQLCollationFromDb();
                }
                return Collations;
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
        ///     This GET method is used to list SQL collation information based
        ///     on subscription Id.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns>list of SQL collation</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.SQLCollation> ListSQLCollations(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                var ex = new ArgumentNullException(subscriptionId);
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw ex;
            }
            return ListSQLCollations();
        }
    }
}
