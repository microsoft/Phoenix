//*****************************************************************************
// File: CmpWapDbAdminRepository.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api.Repositories
// Purpose: This class contains methods that interact with WAP DB.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Repositories
{
    /// <remarks>
    /// This class contains methods that interact with WAP DB.
    /// </remarks>
    public class CmpWapDbAdminRepository
    {
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
                using (var db = new Models.MicrosoftMgmtSvcCmpContext())
                {
                    var vmSizesQuery = from vms in db.VmSizes
                                       join subMapTable in db.AzureAdminSubscriptionVmSizeMappings
                                       on vms.VmSizeId equals subMapTable.VmSizeId
                                       into subVmSizes
                                       from svs in subVmSizes
                                       join planMapTable in db.AzureAdminSubscriptionMappings
                                       on svs.SubscriptionId equals planMapTable.SubId
                                       where vms.IsActive & planMapTable.PlanId == planId
                                       select new { Key = vms, Value = svs.IsActive };

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
                throw new Exception("Exception in FetchVmSizeInfoList() : "
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
                using (var db = new Models.MicrosoftMgmtSvcCmpContext())
                {
                    //Get all subscriptions tied to plan, and get all the mappings to VmSize objects that contain said subscriptions.
                    var subscriptionsList = db.AzureAdminSubscriptionMappings.Where(x => x.PlanId == planId && x.IsActive).Select(y => y.SubId);
                    var vmSizePlanMappingList = db.AzureAdminSubscriptionVmSizeMappings.Where(x => subscriptionsList.Contains(x.SubscriptionId));

                    foreach (var vmSizeParameter in vmSizeList)
                    {
                        if (vmSizePlanMappingList.Select(x => x.VmSizeId).Contains(vmSizeParameter.VmSizeId))
                        {
                            //Mapping exists on table. Just toggle the IsActive value for every subscription which implements that VmSize Object
                            var targetSubscriptions = vmSizePlanMappingList.Where(x => x.VmSizeId == vmSizeParameter.VmSizeId);
                            foreach (var item in targetSubscriptions)
                            {
                                db.AzureAdminSubscriptionVmSizeMappings.Where(x => x.Id == item.Id).FirstOrDefault().IsActive = vmSizeParameter.IsActive;
                            }
                        }
                        else
                        {
                            //Mapping doesn't exist on table. It's a new association. Add mapping record to table for each of the subscriptions associated to the plan.
                            foreach (var subscription in subscriptionsList)
                            {
                                db.AzureAdminSubscriptionVmSizeMappings.Add(new AzureAdminSubscriptionVmSizeMapping
                                {
                                    VmSizeId = vmSizeParameter.VmSizeId,
                                    SubscriptionId = subscription,
                                    IsActive = vmSizeParameter.IsActive
                                });
                            }
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
        ///     This method fetches VmOs info based on a plan ID
        /// </summary>
        /// <param name="planId"></param>
        /// <returns>Vm Os IDictionary</returns>
        /// 
        //*********************************************************************

        public IDictionary<Models.VmOs, bool> FetchOsInfoList(string planId)
        {
            var vmOsResult = new Dictionary<VmOs, bool>();

            if (string.IsNullOrEmpty(planId))
                return null;

            try
            {
                using (var db = new Models.MicrosoftMgmtSvcCmpContext())
                {
                    var vmOsQuery = from vmos in db.VmOs
                                    join subMapTable in db.AzureAdminSubscriptionVmOsMappings
                                    on vmos.VmOsId equals subMapTable.VmOsId
                                    into subVmOss
                                    from svs in subVmOss
                                    join planMapTable in db.AzureAdminSubscriptionMappings
                                    on svs.SubscriptionId equals planMapTable.SubId
                                    where vmos.IsActive & planMapTable.PlanId == planId
                                    select new { Key = vmos, Value = svs.IsActive };

                    if (!vmOsQuery.Any())
                        return null;

                    foreach (var item in vmOsQuery.Distinct())
                    {
                        vmOsResult.Add(item.Key, item.Value);
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
                using (var db = new Models.MicrosoftMgmtSvcCmpContext())
                {
                    //Get all subscriptions tied to plan, and get all the mappings to VmOs objects that contain said subscriptions.
                    var subscriptionsList = db.AzureAdminSubscriptionMappings.Where(x => x.PlanId == planId && x.IsActive).Select(y => y.SubId);
                    var vmOsPlanMappingList = db.AzureAdminSubscriptionVmOsMappings.Where(x => subscriptionsList.Contains(x.SubscriptionId));

                    foreach (var vmOsParameter in vmOsList)
                    {
                        if (vmOsPlanMappingList.Select(x => x.VmOsId).Contains(vmOsParameter.VmOsId))
                        {
                            //Mapping exists on table. Just toggle the IsActive value for every subscription which implements that VmOs Object
                            var targetSubscriptions = vmOsPlanMappingList.Where(x => x.VmOsId == vmOsParameter.VmOsId);
                            foreach (var item in targetSubscriptions)
                            {
                                db.AzureAdminSubscriptionVmOsMappings.Where(x => x.Id == item.Id).FirstOrDefault().IsActive = vmOsParameter.IsActive;
                            }
                        }
                        else
                        {
                            //Mapping doesn't exist on table. It's a new association. Add mapping record to table for each of the subscriptions associated to the plan.
                            foreach (var subscription in subscriptionsList)
                            {
                                db.AzureAdminSubscriptionVmOsMappings.Add(new AzureAdminSubscriptionVmOsMapping
                                {
                                    VmOsId = vmOsParameter.VmOsId,
                                    SubscriptionId = subscription,
                                    IsActive = vmOsParameter.IsActive
                                });
                            }
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
    }
}