using System.Collections.Generic;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api
{
    public interface ICmpWapDb
    {
        //*********************************************************************
        ///
        /// <summary>
        ///     This method checks if Application data exists
        /// </summary>
        /// <returns>boolean</returns>
        /// 
        //*********************************************************************
        bool CheckAppDataRecord(CreateVm vm);

        ///
        /// <summary>
        ///     This method inserts Application data
        /// </summary>
        /// <returns>void</returns>
        /// 
        //*********************************************************************
        void InsertAppDataRecord(CreateVm vm);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method lists application data
        /// </summary>
        /// <returns>list of application data</returns>
        /// 
        //*********************************************************************
        List<Models.Application> FetchAppList();

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches VM size info
        /// </summary>
        /// <param name="onlyActiveOnes"></param>
        /// <returns>VM size list</returns>
        /// 
        //*********************************************************************
        IEnumerable<Models.VmSize> FetchVmSizeInfoList(bool onlyActiveOnes);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches VM size info based on size name
        /// </summary>
        /// <param name="roleSizeName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************
        Models.VmSize FetchVmSizeInfo(string roleSizeName);

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie method updates VM Size information based on an VmSize object
        /// </summary>
        /// <param name="vmSize"></param>
        /// 
        //*********************************************************************
        void UpdateVmSizeInfo(Models.VmSize vmSize);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to insert a list of VM Size objects
        ///     into the DB
        /// </summary>
        /// <param name="vms">A list of VmSize objects</param>
        /// 
        //*********************************************************************
        void InsertVmSizeByBatch(IEnumerable<Models.VmSize> vms);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to delete a VM Size.
        /// </summary>
        /// <param name="vmSizeId"></param>
        /// 
        //*********************************************************************
        void DeleteVmSize(int vmSizeId);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches OS info.
        /// </summary>
        /// <param name="onlyActiveOnes"></param>
        /// <returns>OS list</returns>
        /// 
        //*********************************************************************
        IEnumerable<Models.VmOs> FetchOsInfoList(bool onlyActiveOnes);

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie method updates VM OS information based on an VmOs object
        /// </summary>
        /// <param name="vmOsInfo"></param>
        /// 
        //*********************************************************************
        void UpdateOsInfo(Models.VmOs vmOsInfo);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to insert an enmum VM OS info record into 
        ///     the DB
        /// </summary>
        /// <param name="vmOsInfo">An enum of VmOs objects</param>
        /// 
        //*********************************************************************
        void InsertOsInfoByBatch(IEnumerable<Models.VmOs> vmOsInfo);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to delete a VM OS Info record.
        /// </summary>
        /// <param name="vmOsInfoId"></param>
        /// 
        //*********************************************************************
        void DeleteOsInfo(int vmOsInfoId);

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie method fetches Azure region information
        /// </summary>
        /// <param name="onlyActiveOnes"></param>
        /// <returns>list of Azure region</returns>
        /// 
        //*********************************************************************
        List<Models.AzureRegion> FetchAzureRegionList(bool onlyActiveOnes);

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie method fetches Azure region information by plan ID
        /// </summary>
        /// <returns>list of Azure region</returns>
        /// 
        //*********************************************************************

        List<Models.AzureRegion> FetchAzureRegionListByPlan(string planId);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to insert an Azure region into the DB
        /// </summary>
        /// <param name="ar">An enum of AzureRegion objects</param>
        /// 
        //*********************************************************************
        void InsertAzureRegionByBatch(IEnumerable<Models.AzureRegion> ar);

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie method updates Azure region information
        /// </summary>
        /// <param name="azureRegion"></param>
        /// 
        //*********************************************************************
        void UpdateAzureRegion(Models.AzureRegion azureRegion);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to delete an Azure Region.
        /// </summary>
        /// <param name="azureRegionId"></param>
        /// 
        //*********************************************************************
        void DeleteAzureRegion(int azureRegionId);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches resource provider account group information
        /// </summary>
        /// <returns>resource provider account group list</returns>
        /// 
        //*********************************************************************
        IEnumerable<Models.ResourceProviderAcctGroup> FetchResourceGroupList();

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches resource provider account group information
        /// </summary>
        /// <returns>resource provider account group list</returns>
        /// 
        //*********************************************************************
        IEnumerable<Models.ResourceProviderAcctGroup> FetchResourceGroupList(
            string wapSubscriptionId);

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wapSubscriptionId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************
        string FetchDefaultResourceProviderGroupName(string wapSubscriptionId);

        //*********************************************************************
        ///
        /// <summary>
        ///     Check if Resource Group Exists
        /// </summary>
        /// <returns>boolean</returns>
        /// 
        //*********************************************************************
        bool CheckResourceGroupExists(string resourceGroup);

        //*********************************************************************
        ///
        /// <summary>
        ///     Insert a new Resource Group
        /// </summary>
        /// <returns>void</returns>
        /// 
        //*********************************************************************
        void InsertResourceProviderAcctGroup(string resourceGroup);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches domain list information
        /// </summary>
        /// <returns> AD Domain list</returns>
        /// 
        //*********************************************************************
        List<Models.AdDomainMap> FetchDomainInfoList();

        //*********************************************************************
        /// <summary>
        ///     This method fetches VM deployment request information
        /// </summary>
        /// <param name="status"></param>
        /// <param name="active"></param>
        /// <param name="subscriptionId"></param>
        /// <returns>VM deployment request list</returns>
        //*********************************************************************
        IEnumerable<Models.CmpRequest> FetchVmDepRequests(string status, bool active, string subscriptionId = null);

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
        Models.CmpRequest FetchVmDepRequest(int deploymentRequestId);

        //*********************************************************************
        /// 
        ///  <summary>
        ///      Tis method is used for updating VM size.
        ///  </summary>
        ///  <param name="vmDepReqId"></param>
        /// <param name="size"></param>
        ///  
        //*********************************************************************
        void UpdateVmSize(int vmDepReqId, string size);

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie methid is used to delete VM deployment request
        /// </summary>
        /// <param name="vmDepReqId"></param>
        /// 
        //*********************************************************************
        void DeleteVmDepRequest(int vmDepReqId);

        //*********************************************************************
        /// 
        ///  <summary>
        ///     This method is used to set VM deployment request status.
        ///  </summary>
        /// <param name="vmRequest"></param>
        /// <param name="warningList"></param>
        ///  
        //*********************************************************************
        void SetVmDepRequestStatus(Models.CmpRequest vmRequest, List<string> warningList);

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
        Models.CmpRequest InsertVmDepRequest(Models.CmpRequest vmRequest);

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
        CreateVm InsertVmDepRequest(CreateVm createVm);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to fetch EnvironmentType information
        /// </summary>
        /// <returns>EnvironmentType list</returns>
        /// 
        //*********************************************************************
        List<Models.EnvironmentType> FetchEnvironmentTypeInfoList();

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches Server role information list
        /// </summary>
        /// <returns>ServerRole list</returns>
        /// 
        //*********************************************************************
        List<Models.ServerRole> FetchServerRoleInfoList();

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches Server role drive information list
        /// </summary>
        /// <returns>ServerRole drive list</returns>
        /// 
        //*********************************************************************
        List<Models.ServerRoleDriveMapping> FetchServerRoleDriveInfoList();

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches ServiceCategory information list
        /// </summary>
        /// <returns>ServiceCategory list</returns>
        /// 
        //*********************************************************************
        List<Models.ServiceCategory> FetchServiceCategoryInfoList();

        //*********************************************************************
        ///
        /// <summary>
        ///     Thie method fetches network NIC list
        /// </summary>
        /// <returns>network NIC list</returns>
        /// 
        //*********************************************************************
        List<Models.NetworkNIC> FetchNetworkNicInfoList();

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches SQLCollation information list.
        /// </summary>
        /// <returns>SQLCollation list</returns>
        /// 
        //*********************************************************************
        List<Models.SQLCollation> FetchSqlCollationInfoList();

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches SQL version information list
        /// </summary>
        /// <returns>SQL version list</returns>
        /// 
        //*********************************************************************
        List<Models.SQLVersion> FetchSqlVersionInfoList();

        ///
        /// <summary>
        ///     This method fetches SQLAnalysisServicesMode list
        /// </summary>
        /// <returns>SQLAnalysisServicesMode list</returns>
        /// 
        //*********************************************************************
        List<Models.SQLAnalysisServicesMode> FetchSqlAnalysisServicesModeInfoList();

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches IISRoleService list
        /// </summary>
        /// <returns>IISRoleService list</returns>
        /// 
        //*********************************************************************
        List<Models.IISRoleService> FetchIisRoleServiceInfoList();

        //*********************************************************************
        ///
        /// <summary>
        ///     This method fetches SequenceRequests list
        /// </summary>
        /// <param name="wapSubscriptionId"></param>
        /// <returns>SequenceRequests list</returns>
        /// 
        //*********************************************************************
        List<Models.SequenceRequest> FetchSequenceRequests(string wapSubscriptionId);

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
        Models.SequenceRequest FetchSequenceRequest(
            string wapSubscriptionId, string serviveProviderJobId);

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
        Models.SequenceRequest FetchSequenceRequest(
            string wapSubscriptionId, int id);

        //*********************************************************************
        /// 
        ///  <summary>
        ///      This methid is used to update SequenceRequest size
        ///  </summary>
        ///  <param name="vmDepReqId"></param>
        /// <param name="size"></param>
        ///  
        //*********************************************************************
        void UpdateSequenceRequestSize(int vmDepReqId, string size);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to delete SequenceRequest.
        /// </summary>
        /// <param name="vmDepReqId"></param>
        /// 
        //*********************************************************************
        void DeleteSequenceRequest(int vmDepReqId);

        //*********************************************************************
        /// 
        ///  <summary>
        ///     This method is use dto set SequenceRequest status
        ///  </summary>
        /// <param name="vmRequest"></param>
        /// <param name="warningList"></param>
        ///  
        //*********************************************************************
        void SetSequenceRequestStatus(Models.CmpRequest vmRequest, List<string> warningList);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to insert a particular SequenceRequest into DB.
        /// </summary>
        /// <param name="sequenceRequest"></param>
        /// <returns>SequenceRequest object</returns>
        /// 
        //*********************************************************************
        Models.SequenceRequest InsertSequenceRequest(Models.SequenceRequest sequenceRequest);

        //*********************************************************************
        ///
        /// <summary>
        ///     This method retrieves the plan information associated with the
        ///     WAP subscription provided
        /// </summary>
        /// <returns>Plan object (as defined by MgmtSvc.Store database)</returns>
        /// 
        //*********************************************************************
        string GetPlanMappedToWapSubscription(string wapSubscriptionId);

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
        IEnumerable<ServiceProviderAccount> FetchServiceProviderAccountsAssociatedWithPlan(string planId);
    }
}