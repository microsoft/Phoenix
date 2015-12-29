//*****************************************************************************
// File: ServerRolesController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class is used to provide server role information for a
//          particular virtual machine.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Diagnostics;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    /// <remarks>
    /// his class is used to provide server role information for a
    /// particular virtual machine.
    /// </remarks>
    public class ServerRolesController : BaseApiController
    {
        public static EventLog EventLog;
        public static List<Models.ServerRole> RoleList = new List<Models.ServerRole>();


        //*********************************************************************
        ///
        /// <summary>
        /// This method fetches srever role information from WAP DB.
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchServerRoleFromDb()
        {
            var cwdb = new CmpWapDb();
            var list = cwdb.FetchServerRoleInfoList();
            RoleList.Clear();
            RoleList.AddRange(list);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This GET method lists server role information.
        /// </summary>
        /// <returns>list of server roles</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.ServerRole> ListServerRoles()
        {
            try
            {
                lock (RoleList)
                {
                    if (0 == RoleList.Count)
                        FetchServerRoleFromDb();
                }
                return RoleList;
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
        ///     This GET method lists server role information based on
        ///     subscription Id.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns>list of server roles</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Models.ServerRole> ListServerRoles(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                var ex = new ArgumentNullException(subscriptionId);
                Logger.Log(ex, EventLogEntryType.Error, 100, 1);
                throw ex;
            }
            return ListServerRoles();
        }

        
    }
}
