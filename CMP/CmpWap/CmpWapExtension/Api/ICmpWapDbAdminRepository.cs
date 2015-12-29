using System.Collections.Generic;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api
{
    public interface ICmpWapDbAdminRepository
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
        IDictionary<VmSize, bool> FetchVmSizeInfoList(string planId);

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
        void SetVmSizeByBatch(IEnumerable<VmSize> vmSizeList, string planId);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches AzureRegion info based on a plan ID
        /// </summary>
        /// <param name="planId"></param>
        /// <returns>Azure Region IDictionary</returns>
        /// 
        //*********************************************************************

        IDictionary<AzureRegion, bool> FetchAzureRegionList(string planId);

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

        void SetAzureRegionByBatch(IEnumerable<AzureRegion> azureRegionList, string planId);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches VM Os info based on a plan ID
        /// </summary>
        /// <param name="planId"></param>
        /// <returns>Vm Os IDictionary</returns>
        /// 
        //*********************************************************************
        IDictionary<VmOs, bool> FetchOsInfoList(string planId);

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
        void SetVmOsByBatch(IEnumerable<VmOs> vmOsList, string planId);

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
        void UpdateAzureSubToPlanMapping(IEnumerable<AzureSubscription> subsList, string planId);

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
        IDictionary<AzureSubscription, bool> FetchAzureSubToPlanMapping(string planId);
    }
}