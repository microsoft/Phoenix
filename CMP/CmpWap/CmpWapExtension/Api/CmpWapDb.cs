//*****************************************************************************
// File: CmpWapDb.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class contains methods that interact with WAP DB.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api
{
    /// <remarks>
    /// This class contains methods that interact with WAP DB.
    /// </remarks>
    public class CmpWapDb : ICmpWapDb, ICmpWapDbAdminRepository, ICmpWapDbTenantRepository
    {
        //*********************************************************************
        /// 
        ///  <summary>
        ///      This method is used for logging
        ///  </summary>
        ///  <param name="message"></param>
        ///  <param name="source"></param>
        ///  <param name="ex"></param>
        ///  <param name="type"></param>
        ///  
        //*********************************************************************
        private void LogThis(string message, string source, Exception ex, EventLogEntryType type)
        {
            EventLog.WriteEntry(source, null == ex ? message : Utilities.UnwindExceptionMessages(ex), type);
        }

        #region --- App Region ---

        //*********************************************************************
        ///
        /// <summary>
        ///     This method checks if Application data exists
        /// </summary>
        /// <returns>boolean</returns>
        /// 
        //*********************************************************************

        public bool CheckAppDataRecord(CreateVm vm)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    return db.Applications.Any(app => app.Name == vm.VmAppName || app.Code == vm.VmAppId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CheckAppDataRecord() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method inserts Application data
        /// </summary>
        /// <returns>void</returns>
        /// 
        //*********************************************************************

        public void InsertAppDataRecord(CreateVm vm)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    if (db.Applications.Any(app => app.Name == vm.VmAppName || app.Code == vm.VmAppId))
                    {
                        throw new Exception("Exception in InsertAppDataRecord() : " +
                                            vm.VmAppName + "already exists.");
                    }
                    else
                    {
                        var maxAppId = db.Applications.Max(app => app.ApplicationId);

                        var appn = new Application
                        {
                            ApplicationId = maxAppId + 1,
                            Code = vm.VmAppId,
                            Name = vm.VmAppName,
                            HasService = true,
                            IsActive = true,
                            SubscriptionId = vm.SubscriptionId,
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = vm.AccountAdminLiveEmailId,
                            LastUpdatedOn = DateTime.UtcNow,
                            LastUpdatedBy = vm.AccountAdminLiveEmailId,
                            CIOwner = vm.AccountAdminLiveEmailId
                        };

                        db.Applications.Add(appn);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogThis("Exception in InsertAppDataRecord() : Unable to locate VM request record: ID: ", "InsertAppDataRecord()", ex, EventLogEntryType.Error);

                throw new Exception("Exception in InsertAppDataRecord() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method lists application data
        /// </summary>
        /// <returns>list of application data</returns>
        /// 
        //*********************************************************************

        public List<Application> FetchAppList()
        {
            var appList = new List<Application>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var vmrQ = from rb in db.Applications
                               orderby rb.Name
                               where rb.IsActive
                               select rb;

                    appList.AddRange(vmrQ);
                    return appList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchAppList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- VmSizeInfo ---

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches VM size info
        /// </summary>
        /// <param name="onlyActiveOnes"></param>
        /// <returns>VM Size IEnumerable</returns>
        /// 
        //*********************************************************************

        public IEnumerable<Models.VmSize> FetchVmSizeInfoList(bool onlyActiveOnes)
        {
            var vmSizeResultList = new List<Models.VmSize>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    if (onlyActiveOnes)
                    {
                        var vmSizeQuery = (from rb in db.VmSizes
                                           orderby rb.Name
                                           where rb.IsActive
                                           select rb);

                        vmSizeResultList.AddRange(vmSizeQuery);
                        return vmSizeResultList;
                    }

                    var altVmSizeQuery = from rb in db.VmSizes
                                     orderby rb.Name
                                     select rb;

                    vmSizeResultList.AddRange(altVmSizeQuery);
                    return vmSizeResultList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVmSizeInfoList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches VM size info by subscription id
        /// </summary>
        /// <returns>VM Size IEnumerable</returns>
        /// 
        //*********************************************************************
        IEnumerable<Models.VmSize> ICmpWapDbTenantRepository.FetchVmSizeInfoList(string wapSubscriptionId)
        {
            var vmrList = new List<Models.VmSize>();
            string planId = GetPlanMappedToWapSubscription(wapSubscriptionId); //Name is the PlanId

            try
            {
                if (planId == null)
                    throw new InvalidDataException("No plan was found mapped to the provided WapSubscription");

                //Now using the resolved planId as the clause to search the correct AzureAdminSubscriptionMapping
                //This block is the same as the admin portion, since it relies on a PlanId.
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var vmrQ = (db.VmSizes.Join(db.AzureAdminSubscriptionVmSizeMappings,
                                            vmSize => vmSize.VmSizeId,
                                            subsizeMap => subsizeMap.VmSizeId,
                                            (vmSize, subsizeMap) => new { VmSize = vmSize, SubMap = subsizeMap })
                                        .Where(map => map.SubMap.PlanId == planId
                                               && map.SubMap.IsActive
                                               && map.VmSize.IsActive)
                                        .Select(vmo => vmo.VmSize));

                    vmrList.AddRange(vmrQ);
                    return vmrList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVmSizeInfoList() : "
                    + Utilities.UnwindExceptionMessages(ex) + ex);
            }
        }      

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches VM size info based on a plan ID
        /// </summary>
        /// <param name="planId"></param>
        /// <returns>VM Size IDictionary</returns>
        /// 
        //*********************************************************************
        public IDictionary<Models.VmSize, bool> FetchVmSizeInfoList(string planId)
        {
            var vmSizesResult = new Dictionary<Models.VmSize, bool>();

            if (string.IsNullOrEmpty(planId))
                return null;

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var vmSizesQuery = from vms in db.VmSizes
                                       join mapTable in db.AzureAdminSubscriptionVmSizeMappings
                                       on vms.VmSizeId equals mapTable.VmSizeId into joinedEntities
                                       from mapTable in joinedEntities.DefaultIfEmpty()
                                       where vms.IsActive && (mapTable.PlanId == planId || mapTable == null || mapTable.PlanId == planId)
                                       select new { Key = vms, Value = mapTable == null ? false : mapTable.IsActive };

                    if (!vmSizesQuery.Any())
                        return null;

                    foreach (var item in vmSizesQuery.Distinct())
                    {
                        vmSizesResult.Add(item.Key, item.Value);
                    }

                    return vmSizesResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVmSizeInfo() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to write an association between a plan (or
        ///     its related subscriptions and a VmSize object in the DB
        /// </summary>
        /// <param name="vmSizeList"></param>
        /// <param name="planId"></param>
        /// 
        //*********************************************************************

        public void SetVmSizeByBatch(IEnumerable<Models.VmSize> vmSizeList, string planId)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    //Get all subscriptions tied to plan, and get all the mappings to VmSize objects that contain said subscriptions.

                    var vmSizePlanMappingList = db.AzureAdminSubscriptionVmSizeMappings.Where(x => x.PlanId == planId);

                    foreach (var vmSizeParameter in vmSizeList)
                    {
                        if (vmSizePlanMappingList.Any(x => x.VmSizeId == vmSizeParameter.VmSizeId))
                        {
                            //Mapping exists on table. Just toggle the IsActive value for every subscription which implements that VmSize Object
                            var parameter = vmSizeParameter;
                            var targetSubscriptions = vmSizePlanMappingList.Where(x => x.VmSizeId == parameter.VmSizeId);

                            foreach (var item in targetSubscriptions)
                            {
                                var mapping = db.AzureAdminSubscriptionVmSizeMappings.FirstOrDefault(x => x.Id == item.Id);
                                if (mapping != null) mapping.IsActive = parameter.IsActive;
                            }
                        }
                        else
                        {
                            //Mapping doesn't exist on table. It's a new association. Add mapping record to table for each of the subscriptions associated to the plan.
                                db.AzureAdminSubscriptionVmSizeMappings.Add(new AzureAdminSubscriptionVmSizeMapping
                                {
                                    VmSizeId = vmSizeParameter.VmSizeId,
                                    PlanId = planId,
                                    IsActive = vmSizeParameter.IsActive
                                });
                            }
                        }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in SetVmSizeByBatch() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches VM size info based on size name
        /// </summary>
        /// <param name="roleSizeName"></param>
        /// <returns>A single VM Size object</returns>
        /// 
        //*********************************************************************

        public Models.VmSize FetchVmSizeInfo(string roleSizeName)
        {
            var vmrList = new List<Models.VmSize>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var vmrQ = (from rb in db.VmSizes
                                where rb.IsActive && 
                                rb.Name.Equals(roleSizeName,StringComparison.InvariantCultureIgnoreCase)
                                select rb);

                    if (!vmrQ.Any())
                        return null;

                    vmrList.AddRange(vmrQ);
                    return vmrList.First();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVmSizeInfo() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie method updates VM Size information based on an VmSize object
        /// </summary>
        /// <param name="vmSize"></param>
        /// 
        //*********************************************************************
        public void UpdateVmSizeInfo(Models.VmSize vmSize)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var foundVmSizesList = (from vms in db.VmSizes
                                           where vms.VmSizeId == vmSize.VmSizeId
                                           select vms).ToList();

                    if (!foundVmSizesList.Any())
                    {
                        throw new Exception("UpdateVmSizeInfo() : Unable to locate VM Size record: ID: "
                            + vmSize.VmSizeId);
                    }

                    var foundVmSize = foundVmSizesList.First();
                    Type sourceType = typeof(Models.VmSize);
                    foreach (PropertyInfo propInfo in sourceType.GetProperties())
                    {
                        propInfo.SetValue(foundVmSize, propInfo.GetValue(vmSize));
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogThis("Exception in UpdateVmSizeInfo() : ", "UpdateVmSizeInfo()", ex, EventLogEntryType.Error);

                throw new Exception("Exception in UpdateVmSizeInfo() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to insert a list of VM Size objects
        ///     into the DB
        /// </summary>
        /// <param name="vms">An enum of VmSize objects</param>
        /// 
        //*********************************************************************

        public void InsertVmSizeByBatch(IEnumerable<Models.VmSize> vms)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    foreach (var item in vms)
                    {
                        db.VmSizes.Add(item);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogThis("Exception in InsertVmSizeByBatch() ", "InsertVmSizeByBatch() ", ex, EventLogEntryType.Error);

                throw new Exception("Exception in InsertVmSizeByBatch() "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to delete a VM Size.
        /// </summary>
        /// <param name="vmSizeId"></param>
        /// 
        //*********************************************************************

        public void DeleteVmSize(int vmSizeId)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var foundVmSizesList = from vms in db.VmSizes
                                               where vms.VmSizeId == vmSizeId
                                               select vms;

                    foreach (var foundVmSize in foundVmSizesList)
                    {
                        db.VmSizes.Remove(foundVmSize);
                    }                  
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogThis("Exception in DeleteVmSize() : ", "DeleteVmSize()", ex, EventLogEntryType.Error);

                throw new Exception("Exception in DeleteVmSize() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- VmOsInfo ---

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches OS info.
        /// </summary>
        /// <param name="onlyActiveOnes"></param>
        /// <returns>OS list</returns>
        /// 
        //*********************************************************************

        public IEnumerable<VmOs> FetchOsInfoList(bool onlyActiveOnes)
        {
            var osResultList = new List<VmOs>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    if (onlyActiveOnes)
                    {
                        var osQuery = from rb in db.VmOs
                                   where rb.IsActive
                                   orderby rb.Name
                                   select rb;

                        osResultList.AddRange(osQuery);
                        return osResultList;
                    }

                    var altOsQuery = from rb in db.VmOs
                                     orderby rb.Name
                                     select rb;

                    osResultList.AddRange(altOsQuery);
                    return osResultList;

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchOsInfoList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }


        /// <summary>
        ///  This method fetches an OS list by subscription id, only active ones
        /// </summary>
        /// <returns> OS List</returns>
        List<VmOs> ICmpWapDbTenantRepository.FetchOsInfoList(string wapSubscriptionId)
        {
            var vmrList = new List<VmOs>();
            string planId = GetPlanMappedToWapSubscription(wapSubscriptionId); //Name is the PlanId

            try
            {
                if (planId == null)
                    throw new InvalidDataException("No plan was found mapped to the provided WapSubscription");

                //Now using the resolved planId as the clause to search the correct AzureAdminSubscriptionMapping
                //This block is the same as the admin portion, since it relies on a PlanId.
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var vmrQ = db.VmOs.Join(db.AzureAdminSubscriptionVmOsMappings,
                                            os => os.VmOsId,
                                            subosMap => subosMap.VmOsId,
                                            (os, subosMap) => new { Os = os, SubosMap = subosMap })
                                .Where(map => (map.SubosMap.PlanId == planId)
                                               && map.SubosMap.IsActive
                                               && map.Os.IsActive)
                                .Select(vmo => vmo.Os);


                    vmrList.AddRange(vmrQ);
                    return vmrList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchOsInfoList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches VmOs info based on a plan ID, only active
        ///     ones for the Admin Portal
        /// </summary>
        /// <param name="planId"></param>
        /// <returns>Vm Os IDictionary</returns>
        /// 
        //*********************************************************************

        public IDictionary<VmOs, bool> FetchOsInfoList(string planId)
        {
            var vmOsResult = new Dictionary<VmOs, bool>();

            if (string.IsNullOrEmpty(planId))
                return null;

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var vmOsQuery = from vmos in db.VmOs
                                    join mapTable in db.AzureAdminSubscriptionVmOsMappings
                                    on vmos.VmOsId equals mapTable.VmOsId into joinedEntities
                                    from mapTable in joinedEntities.DefaultIfEmpty()
                                    where vmos.IsActive && (mapTable == null || mapTable.PlanId == planId)
                                    select new { Key = vmos, Value = mapTable == null ? false : mapTable.IsActive };

                    if (!vmOsQuery.Any())
                        return null;

                    foreach (var item in vmOsQuery.Distinct())
                    {
                        //The tempVmOs variable helps to change the name property to a friendly display name. We kinda have to do this as a workaround since it implements the IPlanSetting interface, and they share the name property.
                        var tempVmOs = item.Key;
                        tempVmOs.Name = tempVmOs.AzureImageOffer + ": " + tempVmOs.AzureWindowsOSVersion;

                        vmOsResult.Add(tempVmOs, item.Value);
                    }

                    return vmOsResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchOsInfoList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to write an association between a plan (or
        ///     its related subscriptions and a VmOs object in the DB
        /// </summary>
        /// <param name="vmOsList"></param>
        /// <param name="planId"></param>
        /// 
        //*********************************************************************

        public void SetVmOsByBatch(IEnumerable<VmOs> vmOsList, string planId)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    //Get all subscriptions tied to plan, and get all the mappings to VmOs objects that contain said subscriptions.
                    var vmOsPlanMappingList = db.AzureAdminSubscriptionVmOsMappings.Where(x => x.PlanId == planId);

                    foreach (var vmOsParameter in vmOsList)
                    {
                        if (vmOsPlanMappingList.Any(x => x.VmOsId == vmOsParameter.VmOsId))
                        {
                            //Mapping exists on table. Just toggle the IsActive value for every subscription which implements that VmOs Object
                            var parameter = vmOsParameter;
                            var targetSubscriptions = vmOsPlanMappingList.Where(x => x.VmOsId == parameter.VmOsId);

                            foreach (var item in targetSubscriptions)
                            {
                                var mapping = db.AzureAdminSubscriptionVmOsMappings.FirstOrDefault(x => x.Id == item.Id);
                                if (mapping != null) mapping.IsActive = parameter.IsActive; //The IsActive is for TenantPortal
                            }
                        }
                        else
                        {
                            //Mapping doesn't exist on table. It's a new association. Add mapping record to table for each of the subscriptions associated to the plan.
                                db.AzureAdminSubscriptionVmOsMappings.Add(new AzureAdminSubscriptionVmOsMapping
                                {
                                    VmOsId = vmOsParameter.VmOsId,
                                    PlanId = planId,
                                    IsActive = vmOsParameter.IsActive
                                });
                            }
                        }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in SetVmOsByBatch() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie method updates VM OS information based on an VmOs object
        /// </summary>
        /// <param name="vmOsInfo"></param>
        /// 
        //*********************************************************************
        public void UpdateOsInfo(VmOs vmOsInfo)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var foundVmOsInfoList = (from vmo in db.VmOs
                                            where vmo.VmOsId == vmOsInfo.VmOsId
                                            select vmo).ToList();

                    if (!foundVmOsInfoList.Any())
                    {
                        throw new Exception("UpdateOsInfo() : Unable to locate VM OS Info record: ID: "
                            + vmOsInfo.VmOsId);
                    }

                    var foundVmOsInfo = foundVmOsInfoList.First();
                    Type sourceType = typeof(VmOs);
                    foreach (PropertyInfo propInfo in sourceType.GetProperties())
                    {
                        propInfo.SetValue(foundVmOsInfo, propInfo.GetValue(vmOsInfo));
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogThis("Exception in UpdateOsInfo() : ", "UpdateOsInfo()", ex, EventLogEntryType.Error);

                throw new Exception("Exception in UpdateOsInfo() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to insert an enmum VM OS info record into 
        ///     the DB
        /// </summary>
        /// <param name="vmOsInfo">An enum of VmOs objects</param>
        /// 
        //*********************************************************************
        public void InsertOsInfoByBatch(IEnumerable<VmOs> vmOsInfo)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    foreach (var item in vmOsInfo)
                    {
                        item.Name = item.Name.Length >= 100 ? item.Name.Substring(0, 100) : item.Name;
                        if (!string.IsNullOrEmpty(item.Description))
                            item.Description = item.Description.Length >= 500 ? item.Description.Substring(0, 500) : item.Description;
                        if (!string.IsNullOrEmpty(item.OsFamily))
                            item.OsFamily = item.OsFamily.Length >= 50 ? item.OsFamily.Substring(0, 50) : item.OsFamily;

                        db.VmOs.Add(item);
                    }
                    
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in InsertOsInfoByBatch() "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to delete a VM OS Info record.
        /// </summary>
        /// <param name="vmOsInfoId"></param>
        /// 
        //*********************************************************************
        public void DeleteOsInfo(int vmOsInfoId)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var foundVmOsInfoList = from vmo in db.VmOs
                                               where vmo.VmOsId == vmOsInfoId
                                               select vmo;

                    foreach (var foundVmOsInfo in foundVmOsInfoList)
                    {
                        db.VmOs.Remove(foundVmOsInfo);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DeleteOsInfo() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- Regions ---

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie method fetches Azure region information
        /// </summary>
        /// <param name="onlyActiveOnes"></param>
        /// <returns>list of Azure regions</returns>
        /// 
        //*********************************************************************

        public List<AzureRegion> FetchAzureRegionList(bool onlyActiveOnes)
        {
            var regionResultList = new List<AzureRegion>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {

                    if (onlyActiveOnes)
                    {
                        var regionQuery = from rb in db.AzureRegions
                               orderby rb.Name
                               where rb.IsActive
                               select rb;

                        regionResultList.AddRange(regionQuery);
                        return regionResultList;
                    }

                    var altRegionQuery = from rb in db.AzureRegions
                                     orderby rb.Name
                                     select rb;

                    regionResultList.AddRange(altRegionQuery);
                    return regionResultList;
                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchAzureRegionList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches AzureRegion info based on a plan ID
        /// </summary>
        /// <param name="planId"></param>
        /// <returns>Azure Region IDictionary</returns>
        /// 
        //*********************************************************************

        public IDictionary<AzureRegion, bool> FetchAzureRegionList(string planId)
        {
            var regionResult = new Dictionary<AzureRegion, bool>();

            if (string.IsNullOrEmpty(planId))
                return null;

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var regionQuery =   from regions in db.AzureRegions
                                        join mapTable in db.AzureAdminSubscriptionRegionMappings
                                        on regions.AzureRegionId equals mapTable.AzureRegionId into joinedEntities
                                        from mapTable in joinedEntities.DefaultIfEmpty()
                                        where regions.IsActive && (mapTable.PlanId == planId || mapTable == null || mapTable.PlanId == planId)
                                        select new { Key = regions, Value = mapTable == null ? false : mapTable.IsActive };

                    if (!regionQuery.Any())
                        return null;

                    foreach (var item in regionQuery.Distinct())
                    {
                        regionResult.Add(item.Key, item.Value);
                    }

                    return regionResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchAzureRegionList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie method fetches Azure region information by subscription ID
        /// </summary>
        /// <param name="wapSubscriptionId"></param>
        /// <returns>list of Azure regions</returns>
        /// 
        //*********************************************************************

        List<AzureRegion> ICmpWapDbTenantRepository.FetchAzureRegionList(string wapSubscriptionId)
        {
            //*** TODO * remove this short circuit after the admin console successfully adds region data to db.AzureAdminSubscriptionRegionMappings. This essentially means activating the Azure Region objects in the Plan Config section in the admin Portal
            return FetchAzureRegionList(onlyActiveOnes: true);

            var resultList = new List<AzureRegion>();
            string planId = GetPlanMappedToWapSubscription(wapSubscriptionId); //Name is the PlanId

            try
            {
                if (planId == null)
                    throw new InvalidDataException("No plan was found mapped to the provided WapSubscription");

                //Now using the resolved planId as the clause to search the correct AzureAdminSubscriptionMapping
                //This block is the same as the admin portion, since it relies on a PlanId.
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var regions = from ar in db.AzureRegions
                                  join subMapTable in db.AzureAdminSubscriptionRegionMappings
                                  on ar.AzureRegionId equals subMapTable.AzureRegionId 
                                  orderby ar.Name
                                  where ar.IsActive && subMapTable.PlanId == planId
                                  select ar;

                    resultList.AddRange(regions);
                    return resultList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchAzureRegionsList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie method fetches Azure region information by plan ID
        /// </summary>
        /// <param name="planId"></param>
        /// <returns>list of Azure regions</returns>
        /// 
        //*********************************************************************

        public List<AzureRegion> FetchAzureRegionListByPlan(string planId)
        {
            var resultList = new List<AzureRegion>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var regions = from ar in db.AzureRegions
                                  join mapTable in db.AzureAdminSubscriptionRegionMappings
                                  on ar.AzureRegionId equals mapTable.AzureRegionId into joinedEntities
                                  from mapTable in joinedEntities.DefaultIfEmpty()
                                  orderby ar.Name
                                  where ar.IsActive && (mapTable.PlanId == planId || mapTable == null || mapTable.PlanId == planId)
                                  select ar;

                    resultList.AddRange(regions);
                    return resultList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchAzureRegionsListByPlan() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to write an association between a plan (or
        ///     its related subscriptions and an AzureRegion object in the DB
        /// </summary>
        /// <param name="azureRegionList"></param>
        /// <param name="planId"></param>
        /// 
        //*********************************************************************

        public void SetAzureRegionByBatch(IEnumerable<AzureRegion> azureRegionList, string planId)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    //Get all subscriptions tied to plan, and get all the mappings to AzureRegion objects that contain said subscriptions.

                    var regionsPlanMappingList = db.AzureAdminSubscriptionRegionMappings.Where(x => x.PlanId == planId);

                    foreach (var azureRegionParameter in azureRegionList)
                    {
                        if (regionsPlanMappingList.Any(x => x.AzureRegionId == azureRegionParameter.AzureRegionId))
                        {
                            //Mapping exists on table. Just toggle the IsActive value for every subscription which implements that AzureRegion Object
                            var parameter = azureRegionParameter;
                            var targetSubscriptions = regionsPlanMappingList.Where(x => x.AzureRegionId == parameter.AzureRegionId);

                            foreach (var item in targetSubscriptions)
                            {
                                var mapping = db.AzureAdminSubscriptionRegionMappings.FirstOrDefault(x => x.Id == item.Id);
                                if (mapping != null) mapping.IsActive = parameter.IsActive;
                            }
                        }
                        else
                        {
                            //Mapping doesn't exist on table. It's a new association. Add mapping record to table for each of the subscriptions associated to the plan.
                            db.AzureAdminSubscriptionRegionMappings.Add(new AzureAdminSubscriptionRegionMapping()
                            {
                                AzureRegionId = azureRegionParameter.AzureRegionId,
                                PlanId = planId,
                                IsActive = azureRegionParameter.IsActive
                            });
                        }
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in SetAzureRegionByBatch() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to insert an Azure region into the DB
        /// </summary>
        /// <param name="ar">An enum of AzureRegion objects</param>
        /// 
        //*********************************************************************
        public void InsertAzureRegionByBatch(IEnumerable<AzureRegion> ar)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    db.AzureRegions.AddRange(ar);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in InsertAzureRegionByBatch() "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie method updates Azure region information
        /// </summary>
        /// <param name="azureRegion"></param>
        /// 
        //*********************************************************************
        public void UpdateAzureRegion(AzureRegion azureRegion)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var foundRegionsList = from ar in db.AzureRegions
                                           where ar.AzureRegionId == azureRegion.AzureRegionId
                                           select ar;

                    if (!foundRegionsList.Any())
                    {
                        throw new Exception("UpdateAzureRegion() : Unable to locate Azure Region record: ID: "
                            + azureRegion.AzureRegionId);
                    }

                    var foundRegion = foundRegionsList.First();
                    Type sourceType = typeof(Models.VmSize);
                    foreach (PropertyInfo propInfo in sourceType.GetProperties())
                    {
                        propInfo.SetValue(foundRegion, propInfo.GetValue(azureRegion));
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogThis("Exception in UpdateAzureRegion() : ", "UpdateAzureRegion()", ex, EventLogEntryType.Error);

                throw new Exception("Exception in UpdateAzureRegion() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to delete an Azure Region.
        /// </summary>
        /// <param name="azureRegionId"></param>
        /// 
        //*********************************************************************

        public void DeleteAzureRegion(int azureRegionId)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var foundAzureRegionList = from ar in db.AzureRegions
                                       where ar.AzureRegionId == azureRegionId
                                       select ar;

                    foreach (var foundAzureRegion in foundAzureRegionList)
                    {
                        db.AzureRegions.Remove(foundAzureRegion);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in DeleteAzureRegion() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- ResourceGroup ---

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches resource provider account group information
        /// </summary>
        /// <returns>resource provider account group list</returns>
        /// 
        //*********************************************************************

        public IEnumerable<ResourceProviderAcctGroup> FetchResourceGroupList()
        {
            try
            {
                VMServiceRepository vmsRepo = new VMServiceRepository();

                IEnumerable<int> spaIds;
                List<ResourceProviderAcctGroup> result = new List<ResourceProviderAcctGroup>();

                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var spaMappings = (from map in db.AzureAdminSubscriptionMappings
                                       where map.IsActive
                                       select map);

                    /*In order not to mix the two contexts (DBs and avoid model overhead in projects,
                     * we have to extract the scalar values of the FK Id and make Enumerables of 
                     * which ones are within the plan and which ones are active. Otherwise, EF (or rather Linq
                     * to Entity throws an exception)
                     */

                    if (!spaMappings.Any())
                        return result;

                    spaIds = spaMappings.Select(s => s.SubId).ToList();
                }

                var allPlanSpas = vmsRepo.FetchServiceProviderAccountList(spaIds);

                foreach (var item in allPlanSpas)
                {
                    result.Add(new ResourceProviderAcctGroup
                    {
                        AdDomainMap = null,
                        Name = item.ResourceGroup,
                        CreatedBy = "Cmp Wap Extension",
                        CreatedOn = DateTime.Now,
                        DomainId = 1,
                        EnvironmentType = null,
                        EnvironmentTypeId = 1,
                        IsActive = true,
                        LastUpdatedBy = "Cmp Wap Extension",
                        LastUpdatedOn = DateTime.Now,
                        NetworkNIC = null,
                        NetworkNICId = 0,
                        ResourceProviderAcctGroupId = 1
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchResourceGroupList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches resource provider account group information
        /// </summary>
        /// <returns>resource provider account group list</returns>
        /// 
        //*********************************************************************

        public IEnumerable<ResourceProviderAcctGroup> FetchResourceGroupList(string wapSubscriptionId)
        {   
            try
            {
                VMServiceRepository vmsRepo = new VMServiceRepository();

                IEnumerable<int> spaIds;
                List<ResourceProviderAcctGroup> result = new List<ResourceProviderAcctGroup>();
                string planId = GetPlanMappedToWapSubscription(wapSubscriptionId);

                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var spaMappings = (from map in db.AzureAdminSubscriptionMappings
                                       where map.PlanId == planId && map.IsActive
                                       select map);

                    /*In order not to mix the two contexts (DBs and avoid model overhead in projects,
                     * we have to extract the scalar values of the FK Id and make Enumerables of 
                     * which ones are within the plan and which ones are active. Otherwise, EF (or rather Linq
                     * to Entity throws an exception)
                     */

                    if (!spaMappings.Any())
                        return result;

                    spaIds = spaMappings.Select(s => s.SubId).ToList();
                }

                var allPlanSpas = vmsRepo.FetchServiceProviderAccountList(spaIds);

                foreach (var item in allPlanSpas)
                {
                    result.Add(new ResourceProviderAcctGroup
                    {
                        AdDomainMap = null,
                        Name = item.ResourceGroup,
                        CreatedBy = "Cmp Wap Extension",
                        CreatedOn = DateTime.Now,
                        DomainId = 1,
                        EnvironmentType = null,
                        EnvironmentTypeId = 1,
                        IsActive = true,
                        LastUpdatedBy = "Cmp Wap Extension",
                        LastUpdatedOn = DateTime.Now,
                        NetworkNIC = null,
                        NetworkNICId = 0,
                        ResourceProviderAcctGroupId = 1
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchResourceGroupList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }


        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wapSubscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public string FetchDefaultResourceProviderGroupName(string wapSubscriptionId)
        {
            try
            {
                var result = FetchResourceGroupList(wapSubscriptionId).FirstOrDefault();

                if (result == null)
                    throw new Exception("Resource Group not found");

                return result.Name;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchDefaultResourceProviderGroupName() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     Check if Resource Group Exists
        /// </summary>
        /// <returns>boolean</returns>
        /// 
        //*********************************************************************
        public bool CheckResourceGroupExists(string resourceGrp)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    return db.ResourceProviderAcctGroups.Any(r => r.Name == resourceGrp);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CheckResourceGroupExists() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     Insert a new Resource Group
        /// </summary>
        /// <returns>void</returns>
        /// 
        //*********************************************************************
        public void InsertResourceProviderAcctGroup(string resourceGrp)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    if (db.ResourceProviderAcctGroups.Any(r => r.Name == resourceGrp))
                    {
                        throw new Exception("Exception in InsertAppDataRecord() : Unable to locate VM request record: ID: ");
                    }

                    var maxRgrpId = db.ResourceProviderAcctGroups.Max(rgrp => rgrp.ResourceProviderAcctGroupId);

                    var appn = new ResourceProviderAcctGroup
                    {
                        ResourceProviderAcctGroupId = maxRgrpId + 1,
                        Name = resourceGrp,
                        DomainId = 1,
                        NetworkNICId = 1,
                        EnvironmentTypeId = 1,
                        IsActive = true,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = "AdminExtension", // 
                        LastUpdatedOn = DateTime.UtcNow,
                        LastUpdatedBy = "AdminExtension",
                    };

                    db.ResourceProviderAcctGroups.Add(appn);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogThis("InsertResourceProviderAcctGroup() : ", "InsertResourceProviderAcctGroup()", null, EventLogEntryType.Error);

                throw new Exception("Exception in InsertResourceProviderAcctGroup() : "
                                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- DomainInfo ---

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches domain list information
        /// </summary>
        /// <returns> AD Domain list</returns>
        /// 
        //*********************************************************************

        public List<AdDomainMap> FetchDomainInfoList()
        {
            var vmrList = new List<AdDomainMap>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var vmrQ = from rb in db.AdDomainMaps
                               orderby rb.DomainFullName
                               select rb;

                    vmrList.AddRange(vmrQ);
                    return vmrList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchDomainInfoList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- VmDepReqs ---

        //*********************************************************************
        //
        /// <summary>
        ///     This method fetches VM deployment request information
        /// </summary>
        /// <param name="status"></param>
        /// <param name="active"></param>
        /// <param name="subscriptionId"></param>
        /// <returns>VM deployment request list</returns>
        /// 
        //*********************************************************************

        public IEnumerable<CmpRequest> FetchVmDepRequests(string status, bool active, string subscriptionId = null)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    IOrderedQueryable<CmpRequest> vmrQ;

                    if (subscriptionId == null)
                    {
                        if (null == status)
                            vmrQ = from rb in db.CmpRequests
                                   where (rb.Active == active)
                                   orderby rb.Id
                                   select rb;
                        else
                            vmrQ = from rb in db.CmpRequests
                                   where (rb.StatusCode == status & rb.Active == active)
                                   orderby rb.Id
                                   select rb;
                    }
                    else
                    {
                        if (null == status)
                            vmrQ = from rb in db.CmpRequests
                                   where (rb.Active == active)
                                   && rb.WapSubscriptionID == subscriptionId
                                   orderby rb.Id
                                   select rb;
                        else
                            vmrQ = from rb in db.CmpRequests
                                   where (rb.StatusCode == status & rb.Active == active)
                                   && rb.WapSubscriptionID == subscriptionId
                                   orderby rb.Id
                                   select rb;                        
                    }             
                    return vmrQ.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVmDepRequests() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches a particular VM deployment request based on
        ///     deployment request Id.
        /// </summary>
        /// <param name="deploymentRequestId"></param>
        /// <returns>CmpRequest object</returns>
        /// 
        //*********************************************************************
        public CmpRequest FetchVmDepRequest(int deploymentRequestId)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var vmrQ = from rb in db.CmpRequests
                               where rb.Id == deploymentRequestId
                               select rb;

                    return !vmrQ.Any() ? null : vmrQ.First();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVmDepRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///      This method is used for updating VM size.
        ///  </summary>
        ///  <param name="vmDepReqId"></param>
        /// <param name="size"></param>
        ///  
        //*********************************************************************
        public void UpdateVmSize(int vmDepReqId, string size)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {

                    var foundReqList = from vmr in db.CmpRequests
                                       where vmr.Id == vmDepReqId
                                       select vmr;

                    if (!foundReqList.Any())
                    {
                        throw new Exception("Unable to locate VM request record: ID: "
                            + vmDepReqId);
                    }

                    var foundReq = foundReqList.First();
                    foundReq.VmSize = size;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogThis("Exception in UpdateVmSize() : ", "UpdateVmSize()", ex, EventLogEntryType.Error);

                throw new Exception("Exception in UpdateVmSize() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie methid is used to delete VM deployment request
        /// </summary>
        /// <param name="vmDepReqId"></param>
        /// 
        //*********************************************************************

        public void DeleteVmDepRequest(int vmDepReqId)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var foundReqList = from vmr in db.CmpRequests
                                       where vmr.Id == vmDepReqId
                                       select vmr;

                    foreach (var foundReq in foundReqList)
                        db.CmpRequests.Remove(foundReq);

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in DeleteVmDepRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///     This method is used to set VM deployment request status.
        ///  </summary>
        /// <param name="vmRequest"></param>
        /// <param name="warningList"></param>
        ///  
        //*********************************************************************

        public void SetVmDepRequestStatus(CmpRequest vmRequest, List<string> warningList)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var foundReqList = from vmr in db.CmpRequests
                                       where vmr.Id == vmRequest.Id
                                       select vmr;

                    if (!foundReqList.Any())
                    {
                        
                        throw new Exception("Unable to locate VM request record: ID: "
                            + vmRequest.Id);
                    }

                    var foundReq = foundReqList.First();

                    if (null != warningList)
                        foreach (string warning in warningList)
                        {
                            if (null == foundReq.Warnings)
                                foundReq.Warnings = warning;
                            else
                                foundReq.Warnings = foundReq.Warnings + "; " + warning;
                        }

                    //FoundReq.AftsID = vmRequest.AftsID;
                    foundReq.StatusCode = vmRequest.StatusCode;
                    foundReq.StatusMessage = vmRequest.StatusMessage;
                    foundReq.ExceptionMessage = vmRequest.ExceptionMessage;
                    //FoundReq.CurrentStateStartTime = vmRequest.CurrentStateStartTime;
                    foundReq.LastStatusUpdate = DateTime.UtcNow;
                    foundReq.Warnings = vmRequest.Warnings;
                    foundReq.Config = vmRequest.Config;
                    foundReq.AddressFromVm = vmRequest.AddressFromVm;

                    //if (null != vmRequest.SourceVhdFilesCSV)
                    //    FoundReq.SourceVhdFilesCSV = vmRequest.SourceVhdFilesCSV;

                    //if (null != vmRequest.ServiceProviderStatusCheckTag)
                    //    FoundReq.ServiceProviderStatusCheckTag = vmRequest.ServiceProviderStatusCheckTag;

                    db.SaveChanges();

                    //*** Save change record ***

                    /*Models.ChangeLog CL = new Models.ChangeLog
                        {
                            Message = vmRequest.ExceptionMessage,
                            StatusCode = vmRequest.StatusCode,
                            RequestID = vmRequest.ID,
                            When = Now
                        };

                        SaveChangeRecord(db, CL);*/
                }
            }
            catch (Exception ex)
            {
                LogThis("Exception in SetVmRequestStatus() : ", "SetVmRequestStatus()", ex, EventLogEntryType.Error);

                throw new Exception("Exception in SetVmRequestStatus() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to fetch Maximum Id of VM deployment request
        /// </summary>
        /// <param name="db"></param>
        /// <returns>Id</returns>
        /// 
        //*********************************************************************

        private int FetchMaxVmDepId(MicrosoftMgmtSvcCmpContext db)
        {
            int maxId;

            try
            {
                maxId = (from rb in db.CmpRequests
                         select rb.Id).Max();
            }
            catch (InvalidOperationException)
            {
                maxId = 0;
            }
            catch (Exception ex)
            {
                LogThis("Exception in FetchMaxVmDepID() : ", "FetchMaxVmDepID()", ex, EventLogEntryType.Error);
                throw new Exception("Exception in FetchMaxVmDepID() : " + ex.Message);
            }

            return maxId;
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to insert VM deployment request based on 
        ///     CMPRequest object.
        /// </summary>
        /// <param name="vmRequest"></param>
        /// <returns>CMPRequest object</returns>
        /// 
        //*********************************************************************

        public CmpRequest InsertVmDepRequest(CmpRequest vmRequest)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    vmRequest.Id = FetchMaxVmDepId(db) + 1;

                    db.CmpRequests.Add(vmRequest);
                    db.SaveChanges();

                    return vmRequest;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in InsertVmDepRequest() "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This methid is used to insert VM deployment request Object based
        ///     on CreateVm Object.
        /// </summary>
        /// <param name="createVm"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public CreateVm InsertVmDepRequest(
            CreateVm createVm)
        {
            var cmpReq = new CmpRequest
            {
                Id = 0,
                WapSubscriptionID = createVm.SubscriptionId,
                CmpRequestID = createVm.CmpRequestId,
                ParentAppName = createVm.VmAppName,
                TargetVmName = createVm.Name,
                Domain = createVm.VmDomain,
                VmSize = createVm.VmSize,
                TargetLocation = createVm.VmRegion,
                StatusCode = CmpInterfaceModel.Constants.StatusEnum.Submitted.ToString(),
                SourceImageName = createVm.VmSourceImage,
                SourceServerName = "",
                UserSpec = "<UserSpec></UserSpec>",
                StorageSpec = "<StorageSpec></StorageSpec>",
                FeatureSpec = "<FeatureSpec></FeatureSpec>",
                Config = "<Config></Config>",
                RequestType = CmpInterfaceModel.Constants.RequestTypeEnum.NewVM.ToString(),
                WhoRequested = "",
                WhenRequested = DateTime.UtcNow,
                StatusMessage = "Submitted to CMP Service",
                ExceptionMessage = "",
                Warnings = "",
                LastStatusUpdate = DateTime.UtcNow,
                Active = true,
                TagData = createVm.VmTagData,
                TagID = 0, 
                AddressFromVm = ""
            };

            cmpReq = InsertVmDepRequest(cmpReq);

            createVm.Id = cmpReq.Id;
            createVm.StatusCode = cmpReq.StatusCode;
            createVm.StatusMessage = cmpReq.StatusMessage;

            return createVm;
        }

        #endregion

        #region --- EnvironmentType ---

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to fetch EnvironmentType information
        /// </summary>
        /// <returns>EnvironmentType list</returns>
        /// 
        //*********************************************************************

        public List<Models.EnvironmentType> FetchEnvironmentTypeInfoList()
        {
            var envTypeList = new List<Models.EnvironmentType>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var result = (from et in db.EnvironmentTypes
                                  orderby et.Name
                                  where et.IsActive
                                  select et);

                    envTypeList.AddRange(result);
                    return envTypeList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchEnvironmentTypeInfoList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- ServerRole ---

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches Server role information list
        /// </summary>
        /// <returns>ServerRole list</returns>
        /// 
        //*********************************************************************

        public List<Models.ServerRole> FetchServerRoleInfoList()
        {
            var serverRoleList = new List<Models.ServerRole>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var result = (from sr in db.ServerRoles
                                 orderby sr.Name
                                 where sr.IsActive
                                 select sr);

                    serverRoleList.AddRange(result);
                    return serverRoleList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchServerRoleInfoList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches Server role drive information list
        /// </summary>
        /// <returns>ServerRole drive list</returns>
        /// 
        //*********************************************************************
        public List<Models.ServerRoleDriveMapping> FetchServerRoleDriveInfoList()
        {
            var serverRoleDriveList = new List<Models.ServerRoleDriveMapping>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var result = (from srd in db.ServerRoleDriveMappings
                                  orderby srd.Drive
                                  select srd);

                    serverRoleDriveList.AddRange(result);
                    return serverRoleDriveList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchServerRoleInfoList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- ServiceCategory ---

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches ServiceCategory information list
        /// </summary>
        /// <returns>ServiceCategory list</returns>
        /// 
        //*********************************************************************

        public List<Models.ServiceCategory> FetchServiceCategoryInfoList()
        {
            var serviceTypeList = new List<Models.ServiceCategory>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var result = from sg in db.ServiceCategories
                                 orderby sg.Name
                                 where sg.IsActive
                                 select sg;
                    serviceTypeList.AddRange(result);
                    return serviceTypeList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchServiceCategoryInfoList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- NetworkNIC ---

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie method fetches network NIC list
        /// </summary>
        /// <returns>network NIC list</returns>
        /// 
        //*********************************************************************

        public List<Models.NetworkNIC> FetchNetworkNicInfoList()
        {
            var networkNicList = new List<Models.NetworkNIC>();
            
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var query = (from adn in db.AdDomainMaps
                                 join nn in db.NetworkNICs
                                 on adn.Id equals nn.ADDomainId
                                 where nn.IsActive
                                 orderby nn.Name
                                 select new
                                 {
                                     NetworkNICId = nn.NetworkNICId,
                                     Name = nn.Name,
                                     Description = nn.Description,
                                     IsActive = nn.IsActive,
                                     ADDomain = adn.DomainFullName,
                                     ADdomainId = nn.ADDomainId
                                 }).ToList();

                    if (query.Count != 0) // if there is a AD domain and NIC mapping, return join output
                    {
                        var result = query.Select(x => new Models.NetworkNIC
                        {
                            NetworkNICId = x.NetworkNICId,
                            Name = x.Name,
                            Description = x.Description,
                            IsActive = x.IsActive,
                            ADDomain = x.ADDomain
                        });

                        networkNicList.AddRange(result);
                    }
                    else // return all the NICS present in the DB
                    {
                        var result = (from nn in db.NetworkNICs
                                      orderby nn.Name
                                      where nn.IsActive
                                      select nn);

                        networkNicList.AddRange(result);
                    } 
                    return networkNicList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchNetworkNICInfoList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region ---SQL Collation ---

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches SQLCollation information list.
        /// </summary>
        /// <returns>SQLCollation list</returns>
        /// 
        //*********************************************************************

        public List<Models.SQLCollation> FetchSqlCollationInfoList()
        {
            var list = new List<Models.SQLCollation>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var result = from item in db.SQLCollations
                                  orderby item.Name
                                  where item.IsActive
                                  select item;

                    list.AddRange(result);
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchSQLCollationInfoList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region ---SQL Version ---

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches SQL version information list
        /// </summary>
        /// <returns>SQL version list</returns>
        /// 
        //*********************************************************************

        public List<Models.SQLVersion> FetchSqlVersionInfoList()
        {
            var list = new List<Models.SQLVersion>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var result = (from item in db.SQLVersions
                                  orderby item.Name
                                  where item.IsActive
                                  select item);

                    list.AddRange(result);
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchSQLVersionInfoList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region ---SQLAnalysisServices Mode ---

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches SQLAnalysisServicesMode list
        /// </summary>
        /// <returns>SQLAnalysisServicesMode list</returns>
        /// 
        //*********************************************************************

        public List<SQLAnalysisServicesMode> FetchSqlAnalysisServicesModeInfoList()
        {
            var list = new List<SQLAnalysisServicesMode>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var result = from item in db.SQLAnalysisServicesModes
                                 orderby item.Name
                                 where item.IsActive
                                 select item;
                    list.AddRange(result);
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchSQLAnalysisServicesModeInfoList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region ---IISRoleService---

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches IISRoleService list
        /// </summary>
        /// <returns>IISRoleService list</returns>
        /// 
        //*********************************************************************

        public List<Models.IISRoleService> FetchIisRoleServiceInfoList()
        {
            var list = new List<Models.IISRoleService>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var result = from item in db.IISRoleServices
                                 orderby item.Name
                                 where item.IsActive
                                 select item;

                    list.AddRange(result);
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchIISRoleServiceInfoList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- ServiceProviderAccounts ---

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches SequenceRequests list
        /// </summary>
        /// <param name="wapSubscriptionId"></param>
        /// <returns>SequenceRequests list</returns>
        /// 
        //*********************************************************************

        public List<SequenceRequest> FetchSequenceRequests(string wapSubscriptionId)
        {
            var vmrList = new List<SequenceRequest>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    IOrderedQueryable<SequenceRequest> vmrQ;

                    if (0 == wapSubscriptionId.Length)
                        wapSubscriptionId = null;

                    if (null == wapSubscriptionId)
                        vmrQ = from sr in db.SequenceRequests
                               orderby sr.Id
                               select sr;
                    else
                        vmrQ = from sr in db.SequenceRequests
                               where (sr.WapSubscriptionID == wapSubscriptionId)
                               orderby sr.Id
                               select sr;

                    foreach (var vmr in vmrQ)
                        vmrList.Add(vmr);

                    return vmrList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchSequenceRequests() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     Thisnmethod fetches a particular SequenceRequest.
        /// </summary>
        /// <param name="wapSubscriptionId"></param>
        /// <param name="serviveProviderJobId"></param>
        /// <returns>SequenceRequests object</returns>
        /// 
        //*********************************************************************

        public SequenceRequest FetchSequenceRequest(
            string wapSubscriptionId, string serviveProviderJobId)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    IOrderedQueryable<SequenceRequest> vmrQ;

                    if (0 == wapSubscriptionId.Length)
                        wapSubscriptionId = null;

                    if (null == wapSubscriptionId)
                        vmrQ = from sr in db.SequenceRequests
                               where (sr.ServiceProviderJobId == serviveProviderJobId)
                               orderby sr.Id
                               select sr;
                    else
                        vmrQ = from sr in db.SequenceRequests
                               where (sr.WapSubscriptionID == wapSubscriptionId && sr.ServiceProviderJobId == serviveProviderJobId)
                               orderby sr.Id
                               select sr;

                    return vmrQ.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchSequenceRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This methid fetches a particular SequenceRequests
        /// </summary>
        /// <param name="wapSubscriptionId"></param>
        /// <param name="id"></param>
        /// <returns>SequenceRequests object</returns>
        /// 
        //*********************************************************************

        public SequenceRequest FetchSequenceRequest(
            string wapSubscriptionId, int id)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    IOrderedQueryable<SequenceRequest> vmrQ;

                    if (0 == wapSubscriptionId.Length)
                        wapSubscriptionId = null;

                    if (null == wapSubscriptionId)
                        vmrQ = from sr in db.SequenceRequests
                               where (sr.Id == id)
                               orderby sr.Id
                               select sr;
                    else
                        vmrQ = from sr in db.SequenceRequests
                               where (sr.WapSubscriptionID == wapSubscriptionId && sr.Id == id)
                               orderby sr.Id
                               select sr;

                    return vmrQ.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchSequenceRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///      This methid is used to update SequenceRequest size
        ///  </summary>
        ///  <param name="vmDepReqId"></param>
        /// <param name="size"></param>
        ///  
        //*********************************************************************
        public void UpdateSequenceRequestSize(int vmDepReqId, string size)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var foundReqList = from vmr in db.CmpRequests
                                       where vmr.Id == vmDepReqId
                                       select vmr;

                    if (!foundReqList.Any())
                    {
                        throw new Exception("Unable to locate VM request record: ID: "
                            + vmDepReqId);
                    }

                    var foundReq = foundReqList.First();
                    foundReq.VmSize = size;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogThis("Exception in UpdateVmSize() : ", "UpdateVmSize()", ex, EventLogEntryType.Error);

                throw new Exception("Exception in UpdateVmSize() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to delete SequenceRequest.
        /// </summary>
        /// <param name="vmDepReqId"></param>
        /// 
        //*********************************************************************

        public void DeleteSequenceRequest(int vmDepReqId)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var foundReqList = from vmr in db.CmpRequests
                                       where vmr.Id == vmDepReqId
                                       select vmr;

                    foreach (var foundReq in foundReqList)
                        db.CmpRequests.Remove(foundReq);

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in DeleteVmDepRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///     This method is use dto set SequenceRequest status
        ///  </summary>
        /// <param name="vmRequest"></param>
        /// <param name="warningList"></param>
        ///  
        //*********************************************************************

        public void SetSequenceRequestStatus(CmpRequest vmRequest, List<string> warningList)
        {
            try
            {
                var now = DateTime.UtcNow;

                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var foundReqList = from vmr in db.CmpRequests
                                       where vmr.Id == vmRequest.Id
                                       select vmr;

                    if (!foundReqList.Any())
                    {
                        throw new Exception("Unable to locate VM request record: ID: "
                            + vmRequest.Id);
                    }

                    var foundReq = foundReqList.First();

                    if (null != warningList)
                        foreach (string warning in warningList)
                        {
                            if (null == foundReq.Warnings)
                                foundReq.Warnings = warning;
                            else
                                foundReq.Warnings = foundReq.Warnings + "; " + warning;
                        }

                    //FoundReq.AftsID = vmRequest.AftsID;
                    foundReq.StatusCode = vmRequest.StatusCode;
                    foundReq.StatusMessage = vmRequest.StatusMessage;
                    foundReq.ExceptionMessage = vmRequest.ExceptionMessage;
                    //FoundReq.CurrentStateStartTime = vmRequest.CurrentStateStartTime;
                    foundReq.LastStatusUpdate = now;
                    foundReq.Warnings = vmRequest.Warnings;
                    foundReq.Config = vmRequest.Config;
                    foundReq.AddressFromVm = vmRequest.AddressFromVm;

                    //if (null != vmRequest.SourceVhdFilesCSV)
                    //    FoundReq.SourceVhdFilesCSV = vmRequest.SourceVhdFilesCSV;

                    //if (null != vmRequest.ServiceProviderStatusCheckTag)
                    //    FoundReq.ServiceProviderStatusCheckTag = vmRequest.ServiceProviderStatusCheckTag;

                    db.SaveChanges();

                    //*** Save change record ***

                    /*Models.ChangeLog CL = new Models.ChangeLog
                        {
                            Message = vmRequest.ExceptionMessage,
                            StatusCode = vmRequest.StatusCode,
                            RequestID = vmRequest.ID,
                            When = Now
                        };

                        SaveChangeRecord(db, CL);*/
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in SetVmRequestStatus() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to fetch maximum SequenceRequest Id.
        /// </summary>
        /// <param name="db"></param>
        /// <returns>maximum SequenceRequest Id</returns>
        /// 
        //*********************************************************************

        private int FetchMaxSequenceRequestId(MicrosoftMgmtSvcCmpContext db)
        {
            int maxId;

            try
            {
                maxId = (from rb in db.CmpRequests
                         select rb.Id).Max();
            }
            catch (InvalidOperationException)
            {
                maxId = 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchMaxVmDepID() : " + ex.Message);
            }

            return maxId;
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to insert a particular SequenceRequest into DB.
        /// </summary>
        /// <param name="sequenceRequest"></param>
        /// <returns>SequenceRequest object</returns>
        /// 
        //*********************************************************************

        public SequenceRequest InsertSequenceRequest(SequenceRequest sequenceRequest)
        {
            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    sequenceRequest.Id = FetchMaxSequenceRequestId(db) + 1;

                    db.SequenceRequests.Add(sequenceRequest);
                    db.SaveChanges();

                    return sequenceRequest;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in InsertSquenceRequest() "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region ---  Azure Vnets ---

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches all VNets
        /// </summary>
        /// <returns>VNet IEnumerable</returns>
        /// 
        //*********************************************************************

        public IEnumerable<AzureAdminSubscriptionVnet> FetchVnetList()
        {
            var vmrList = new List<AzureAdminSubscriptionVnet>();

            try
            {
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var vmrQ = (from rb in db.AzureAdminSubscriptionVnets
                                orderby rb.VNetName
                                where rb.IsActive
                                select rb);

                    vmrList.AddRange(vmrQ);
                    return vmrList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVnetList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches Vnets info by subscription id
        /// </summary>
        /// <returns>VNets IEnumerable</returns>
        /// 
        //*********************************************************************

        public IEnumerable<AzureAdminSubscriptionVnet> FetchVnetList(string wapSubscriptionId)
        {
            var vmrList = new List<AzureAdminSubscriptionVnet>();
            string planId = GetPlanMappedToWapSubscription(wapSubscriptionId); //Name is the PlanId

            try
            {
                if (planId == null)
                    throw new InvalidDataException("No plan was found mapped to the provided WapSubscription");
                
                //Now using the resolved planId as the clause to search the correct AzureAdminSubscriptionMapping
                //This block is the same as the admin portion, since it relies on a PlanId.
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var vmrQ = (db.AzureAdminSubscriptionVnets.Join(db.AzureAdminSubscriptionVnetMappings,
                                            vnet => vnet.Id,
                                            subVnetMap => subVnetMap.VnetId,
                                            (vnet, subVnetMap) => new { VNet = vnet, SubMap = subVnetMap })
                                        .Where(map => map.SubMap.PlanId.ToLower() == planId.ToLower()
                                               && map.VNet.IsActive)
                                        .Select(vmo => vmo.VNet));

                    vmrList.AddRange(vmrQ);
                    return vmrList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVnetList() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region --- Mappings (Azure subs to Plans) ---

        //In this case, Azure subscription = Service Provider Account in CMP

        //*********************************************************************
        ///
        /// <summary>
        ///     This method updates the mapping table between azure subscriptions
        ///     and plans
        /// </summary>
        /// <param name="subsList"></param>
        /// <param name="planId"></param>
        /// 
        //*********************************************************************
        public void UpdateAzureSubToPlanMapping(IEnumerable<AzureSubscription> subsList, string planId)
        {
            foreach (var item in subsList)
            {
                var aasm = new AzureAdminSubscriptionMapping
                {
                    SubId = item.Id,
                    PlanId = planId,
                    IsActive = item.Active.GetValueOrDefault(),
                };

                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                    var mappingsFromDb = (from map in db.AzureAdminSubscriptionMappings
                        where map.PlanId == aasm.PlanId && map.SubId == aasm.SubId
                        select map).ToList();

                    if (mappingsFromDb.Any())
                    {
                        //A mapping already exists. 
                        var mappingToUpdate = mappingsFromDb.FirstOrDefault();
                        if (mappingToUpdate != null) mappingToUpdate.IsActive = aasm.IsActive;
                    }
                    else
                    {
                        //It's a new mapping
                        db.AzureAdminSubscriptionMappings.Add(aasm);
                    }

                    db.SaveChanges();
                }
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     Gets the Azure Subscriptions (Service Provider Account from CMP)
        ///     associated with the plan ID provided
        /// </summary>
        /// <returns>
        /// IDictionary of AzureSubscription objects as Key, with its active state
        /// as the value
        /// </returns>
        /// 
        //*********************************************************************
        public IDictionary<AzureSubscription, bool> FetchAzureSubToPlanMapping(string planId)
        {
            VMServiceRepository vmsRepo = new VMServiceRepository();

            IEnumerable<int> spaIds;
            IEnumerable<int> activeMappings;
            IDictionary<AzureSubscription, bool> result = new Dictionary<AzureSubscription, bool>();

            using (var db = new MicrosoftMgmtSvcCmpContext())
            {
                var spaMappings = (from map in db.AzureAdminSubscriptionMappings
                                    where map.PlanId == planId && map.IsActive
                                    select map);

                /*In order not to mix the two contexts (DBs and avoid model overhead in projects,
                 * we have to extract the scalar values of the FK Id and make Enumerables of 
                 * which ones are within the plan and which ones are active. Otherwise, EF (or rather Linq
                 * to Entity throws an exception)
                 */

                if (!spaMappings.Any())
                    return result;

                spaIds = spaMappings.Select(s => s.SubId).ToList();
                activeMappings = spaMappings.Where(s => s.IsActive).Select(s => s.SubId).ToList();
            }

            var allPlanSpas = vmsRepo.FetchServiceProviderAccountList(spaIds);
            //var planActiveSpas = vmsRepo.FetchServiceProviderAccountList(activeMappings);

            var resultSpas = allPlanSpas.Select(x => new
            {
                Key = x,
                Value = activeMappings.Contains(x.ID)
            });

                foreach (var item in resultSpas.Distinct())
                {
                    AzureSubscription key = new AzureSubscription
                    {
                        Id = item.Key.ID,
                        Name = item.Key.Name,
                        Description = item.Key.Description,
                        AccountType = item.Key.AccountType,
                        ExpirationDate = item.Key.ExpirationDate,
                        CertificateBlob = item.Key.CertificateBlob,
                        CertificateThumbprint = item.Key.CertificateThumbprint,
                        AccountID = item.Key.AccountID,
                        AccountPassword = item.Key.AccountPassword,
                        Active = item.Key.Active,
                        TenantID = item.Key.TenantID,
                        ClientID = item.Key.ClientID,
                        ClientKey = item.Key.ClientKey,
                        PlanId = planId
                    };
                    result.Add(key, item.Value);
                }
            //}
            return result;
        }

        #endregion

        #region --- Class Utilites ---

        //*********************************************************************
        ///
        /// <summary>
        ///     This method retrieves the plan information associated with the
        ///     WAP subscription provided
        /// </summary>
        /// <returns>Plan Id (as defined by MgmtSvc.Store database)</returns>
        /// 
        //*********************************************************************
        public string GetPlanMappedToWapSubscription(string wapSubscriptionId)
        {
            Models.MicrosoftMgmtSvcStore.Plan result;
            
            //Getting the mapping of a wapSubscription to a plan so we can query the Admin Mappings
            using (var db = new Models.MicrosoftMgmtSvcStore.MicrosoftMgmtSvcStoreContext())
            {
                result = (from wapSub in db.Subscriptions
                              join plan in db.Plans
                              on wapSub.PlanId equals plan.Id
                              where wapSub.SubscriptionId == wapSubscriptionId
                              select plan).FirstOrDefault();
            }

            if (result == null)
                return String.Empty;

            return result.Name;
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     Gets the Azure Subscriptions (Service Provider Account from CMP)
        ///     associated with the plan ID provided
        /// </summary>
        /// <returns>
        /// IEnumerable of Service Provider Account IDs
        /// </returns>
        /// 
        //*********************************************************************
        public IEnumerable<ServiceProviderAccount> FetchServiceProviderAccountsAssociatedWithPlan(string planId)
        {
            VMServiceRepository vmsRepo = new VMServiceRepository();

            using (var db = new MicrosoftMgmtSvcCmpContext())
            {
                var spaMappings = (from map in db.AzureAdminSubscriptionMappings
                                   where map.PlanId == planId && map.IsActive
                                   select map);

                /*In order not to mix the two contexts (DBs and avoid model overhead in projects,
                 * we have to extract the scalar values of the FK Id and make Enumerables of 
                 * which ones are within the plan and which ones are active. Otherwise, EF (or rather Linq
                 * to Entity throws an exception)
                 */

                if (!spaMappings.Any())
                    return Enumerable.Empty<ServiceProviderAccount>();

                var activeMappings = spaMappings.Where(s => s.IsActive).Select(s => s.SubId).ToList();
                var spasActive = vmsRepo.FetchServiceProviderAccountList(activeMappings);

                return spasActive;
            }
        }

        #endregion
    }
}
