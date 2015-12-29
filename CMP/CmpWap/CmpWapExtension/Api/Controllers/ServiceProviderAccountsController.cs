//*****************************************************************************
// File: ServiceProviderAccountsController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This method provides Service Provider Account related information.
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
    /// <remarks>
    ///     This method provides Service Provider Account related information.
    /// </remarks>
    public class ServiceProviderAccountsController : ApiController
    {
        public static EventLog EventLog;
        public static List<ServiceProviderAccount> SpaList = new List<ServiceProviderAccount>();
        private static bool _havePendingStatus = false;

        /// <summary>
        ///     This method removes Service Providr Account from Service Provider Account list based
        ///     on domain name.
        /// </summary>
        /// <param name="domainName"></param>
        private void RemoveSpaFromList(string domainName)
        {
            foreach (ServiceProviderAccount spa in SpaList)
                if (spa.Name.Equals(domainName))
                    SpaList.Remove(spa);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method adds Service Provider Account to Service Provider 
        ///     Account list using domain.
        /// </summary>
        /// <param name="dom"></param>
        /// 
        //*********************************************************************

        private void AddSpaToList(Models.AdDomainMap dom)
        {
            SpaList.Add(item: new ServiceProviderAccount
            {
                Name = dom.DomainFullName,
                //DisplayName = dom.DomainShortName
            });
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method adds Service Provider Account to Service Provider 
        ///     Account list.
        /// </summary>
        /// <param name="spa"></param>
        /// 
        //*********************************************************************

        private void AddSpaToList(ServiceProviderAccount spa)
        {
            SpaList.Add(spa);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches Service Provider Account from WAP DB.
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchSpaListFromDb(string groupName)
        {
            var cmp = new VMServiceRepository(EventLog);
            var spaList = cmp.FetchServiceProviderAccountList(groupName);

            SpaList.Clear();

            foreach (var spa in spaList)
                AddSpaToList(spa);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPa"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private List<ServiceProviderAccount> AddSpaToDb(ServiceProviderAccount sPa)
        {
            var cmp = new VMServiceRepository(EventLog);
            var spaList = cmp.InsertServiceProviderAccount(sPa);

            SpaList.Clear();

            foreach (var spa in spaList)
            {
                AddSpaToList(spa);
            }          

            return spaList;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPa"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private List<ServiceProviderAccount> UpdateSpaInDb(ServiceProviderAccount sPa)
        {
            var cmp = new VMServiceRepository(EventLog);
            var spaList = cmp.UpdateServiceProviderAccount(sPa);

            SpaList.Clear();

            foreach (var spa in spaList)
                AddSpaToList(spa);

            return spaList;
        }


        //*********************************************************************
        ///
        /// <summary>
        ///     This GET method lists Service Provider Account information.
        /// </summary>
        /// <returns> list of service provider account</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<ServiceProviderAccount> ListServiceProviderAccounts()
        {
            try
            {
                lock (SpaList)
                {
                    //if (0 == SpaList.Count)
                        FetchSpaListFromDb(null);
                }

                var domains = from domain in SpaList
                              select domain;

                return domains.ToList();
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, 
                    "CmpWapExtension.DomainsController.ListServiceProviderAccounts()", 100, 1);
                throw;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This GET method lists Service Provider Account information based
        ///     on subscription Id.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns>list of service provider account</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        public List<ServiceProviderAccount> ListServiceProviderAccounts(string subscriptionId)
        {
            try
            {
                lock (SpaList)
                {
                    if (0 == SpaList.Count)
                        FetchSpaListFromDb(null);
                }

                if (string.IsNullOrWhiteSpace(subscriptionId))
                {
                    throw new ArgumentNullException(subscriptionId);
                }

                var domains = from domain in SpaList
                             select domain;

                return domains.ToList();
            }
            catch(Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.DomainsController.ListServiceProviderAccounts()", 100, 1);
                throw;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPa"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        [HttpPost]
        public List<ServiceProviderAccount> UpdateServiceProviderAccount(ServiceProviderAccount sPa)
        {
            try
            {
                if (0 == sPa.ID)
                {

                    sPa.ExpirationDate = new DateTime(2100, 1, 1);
                    sPa.Active = true;
                    return AddSpaToDb(sPa);
                }

                return UpdateSpaInDb(sPa);
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error,
                    "CmpWapExtension.DomainsController.UpdateServiceProviderAccount()", 100, 1);
                throw;
            }
        }

        private void performResourceGroupOps(string resGroup)
        {
            var cmpi = new VMServiceRepository(EventLog);
            cmpi.PerformResourceGroupOps(resGroup);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used for logging.
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
