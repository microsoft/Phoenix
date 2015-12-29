using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api
{
    public interface ICmpWapDbTenantRepository
    {
        //*********************************************************************
        /// <summary>
        ///     This method fetches VM size info by subscription id
        /// </summary>
        /// <param name="wapSubscriptionId"></param>
        /// <returns>VM Size IEnumerable</returns>
        /// 
        //*********************************************************************
        IEnumerable<Models.VmSize> FetchVmSizeInfoList(string wapSubscriptionId);

        //*********************************************************************
        /// <summary>
        ///  This method fetches os list by subscription id
        /// </summary>
        /// <param name="wapSubscriptionId"></param>
        /// <returns> OS List</returns>
        List<Models.VmOs> FetchOsInfoList(string wapSubscriptionId);

        //*********************************************************************
        /// <summary>
        ///     Thie method fetches Azure region information by subscription ID
        /// </summary>
        /// <param name="wapSubscriptionId"></param>
        /// <returns>list of Azure regions</returns>
        /// 
        //*********************************************************************
        List<Models.AzureRegion> FetchAzureRegionList(string wapSubscriptionId);

        //*********************************************************************
        /// <summary>
        ///     This method fetches all VNets
        /// </summary>
        /// <returns>VNet IEnumerable</returns>
        /// 
        //*********************************************************************
        IEnumerable<Models.AzureAdminSubscriptionVnet> FetchVnetList();

        //*********************************************************************
        /// <summary>
        ///     This method fetches Vnets info by subscription id
        /// </summary>
        /// <param name="wapSubscriptionId"></param>
        /// <returns>VNets IEnumerable</returns>
        /// 
        //*********************************************************************
        IEnumerable<Models.AzureAdminSubscriptionVnet> FetchVnetList(string wapSubscriptionId);
    }
}