//*****************************************************************************
// File: DomainsController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: 
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System.Diagnostics;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{

    public class DomainsController : ApiController
    {
        public static EventLog EventLog;
        public static List<Domain> DomainList = new List<Domain>();
        private static bool _havePendingStatus = false;

        private void RemoveVmFromList(string domainName)
        {
            foreach(Domain cVm in DomainList)
                if (cVm.Name.Equals(domainName))
                    DomainList.Remove(cVm);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method adds domain to a list
        /// </summary>
        /// <param name="dom">Domain</param>
        /// 
        //*********************************************************************

        private void AddVmToList(Models.AdDomainMap dom)
        {
            DomainList.Add(item: new Domain
            {
                Id=dom.Id,
                Name = dom.DomainFullName,
                DisplayName = dom.DomainShortName
            });
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method adds domain to a list.
        /// </summary>
        /// <param name="dom"></param>
        /// 
        //*********************************************************************

        private void AddVmToList(Domain dom)
        {
            DomainList.Add(new Domain
            {
                DisplayName = dom.DisplayName,
                Name = dom.Name,
                Id=dom.Id
            });
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches domain list from WAP DB
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchVmListFromDb()
        {
            var cwdb = new CmpWapDb();
            var domainList = cwdb.FetchDomainInfoList();

            _havePendingStatus = false;
            DomainList.Clear();

            foreach (var dom in domainList)
                AddVmToList(dom);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method list all the domains valid for a VM
        /// </summary>
        /// <returns> a list of domains</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Domain> ListDomains()
        {
            try
            {
                lock (DomainList)
                {
                    if (0 == DomainList.Count)
                        FetchVmListFromDb();
                }

                var domains = from domain in DomainList
                              select domain;

                return domains.ToList();
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.DomainsController.ListDomains()", 100, 1);
                throw;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method list all the domains valid for a VM based on WAP subscription
        /// </summary>
        /// <param name="subscriptionId">WAP subscription Id</param>
        /// <returns>list of domains</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<Domain> ListDomains(string subscriptionId)
        {
            try
            {
                lock (DomainList)
                {
                    if (0 == DomainList.Count)
                        FetchVmListFromDb();
                }

                if (string.IsNullOrWhiteSpace(subscriptionId))
                {
                    throw new ArgumentNullException(subscriptionId);
                }

                var domains = from domain in DomainList
                             select domain;

                return domains.ToList();
            }
            catch(Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.DomainsController.ListDomains()", 100, 1);
                throw;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="type"></param>
        /// <param name="prefix"></param>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// 
        //*********************************************************************

        private void LogThis(Exception ex, EventLogEntryType type, string prefix,
            int id, short category)
        {
            try
            {
                if (null != EventLog)
                    EventLog.WriteEntry(prefix + " : " +
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex), type, id, category);
            }
            catch(Exception ex2)
            { string x = ex2.Message; }
        }
    }
}
