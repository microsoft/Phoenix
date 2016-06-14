using System.Collections.Generic;
using CmpCommon;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient
{
    public interface ICmpApiClient
    {
        /// <summary>
        /// Connection string for the CMP database
        /// </summary>
        string CmpDbConnectionString { set; get; }

        ///
        /// <summary>
        /// Credentials for the CMP client
        /// </summary>
        ///
//*********************************************************************
        System.Net.NetworkCredential CmpClientCredentials { get; set; }

        ///
        /// <summary>
        /// Returns a list of detached disks for the subscription of the VM 
        /// associated with the given CMP request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <returns>A list of detached disks for the subscription of the VM 
        /// associated with the CMP request ID</returns>
        /// 
        //*********************************************************************
        IEnumerable<VhdInfo> GetDetachedDisks(int? cmpRequestId);

        /// 
        ///  <summary>
        ///  Sends a request to delete the VM associated with the given CMP 
        /// request ID
        ///  </summary>
        ///  <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <param name="deleteFromStorage">Boolean specifying whether or not
        /// to delete the VM</param>
        /// <returns>0 on success, 1 on exception or failure</returns>
        ///  
        //*********************************************************************
        int DeleteVm(int cmpRequestId, bool deleteFromStorage);

        ///
        /// <summary>
        /// Reboots the VM associated with the given CMP request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <returns>0 on success, 1 on exception or failure</returns>
        /// 
        //*********************************************************************
        int RebootVm(int cmpRequestId);

        ///
        /// <summary>
        /// Reboots the VM associated with the given CMP request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <returns>0 on success, 1 on exception or failure</returns>
        /// 
        //*********************************************************************
        int StartVm(int cmpRequestId);

        ///
        /// <summary>
        /// Returns info on the VM associated with the CMP given request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <returns>VMinfo object containing VM information</returns>
        /// 
        //*********************************************************************
        CmpApiClient.VmInfo GetVm(int cmpRequestId, CmpInterfaceModel.Constants.FetchType fetchType);

        ///
        /// <summary>
        /// Stops the VM associated with the given CMP request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <returns>0 on success, 1 on exception or failure</returns>
        /// 
        //*********************************************************************
        int StopVm(int cmpRequestId);

        /// 
        /// <summary>
        /// Deallocates the VM associated with the given CMP request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <returns>0 on success, 1 on exception or failure</returns>
        /// 
        //*********************************************************************
        int DeallocateVm(int cmpRequestId);

        ///
        /// <summary>
        /// Adds the specified disks to the VM associated with the given CMP 
        /// request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <param name="disks">List of disk objects to add to the VM </param>
        /// <returns>0 on success, 1 on exception or failure</returns>
        /// 
        //*********************************************************************
        int AddDisk(int cmpRequestId, List<VhdInfo> disks);

        ///
        /// <summary>
        /// Removes the specified disk from the VM associated with the given 
        /// CMP request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated 
        /// with the VM</param>
        /// <param name="disk">Disk to be deleted</param>
        /// <param name="deleteFromStorage">Specifies whether to delete the disk
        /// once it is detached or keep it</param>
        /// 
        //*********************************************************************
        void DetachDisk(int? cmpRequestId, VhdInfo disk, bool deleteFromStorage);

        ///
        /// <summary>
        /// Attaches an available disk to the VM associated with the given 
        /// CMP request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated 
        /// with the VM</param>
        /// <param name="disk">Disk to add to the VM</param>
        /// 
        //*********************************************************************
        void AttachExistingDisk(int? cmpRequestId, VhdInfo disk);

        ///
        /// <summary>
        /// Increases the size of a data disk on the VM associated with the 
        /// given CMP request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <param name="disks">Disks to grow on the VM</param>
        /// <returns>0 on success, 1 on exception or failure</returns>
        /// 
        //*********************************************************************
        int GrowDisk(int cmpRequestId, List<VhdInfo> disks);

        ///
        /// <summary>
        /// Changes the SKU size of the VM associated with the given CMP 
        /// request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <param name="size">SKU size to change the VM size to</param>
        /// <returns>0 on success, 1 on exception or failure</returns>
        /// 
        //*********************************************************************
        int Resize(int cmpRequestId, string size);

        /// 
        ///  <summary>
        ///  Returns the number of disks of the VM associated with the given 
        /// CMP request ID
        ///  </summary>
        ///  <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <param name="roleSizeName">Role type of the disks returned</param>
        /// <returns>0 on success, 1 on exception or failure</returns>
        ///  
        //*********************************************************************
        int FetchDiskCount(int cmpRequestId, out string roleSizeName);

        ///
        /// <summary>
        /// Submits the given operation to the CMP queue
        /// </summary>
        /// <param name="opSpec">The operation to be submitted to the CMP queue
        /// </param>
        /// <returns>The operation submitted to the CMP queue</returns>
        /// 
        //*********************************************************************
        CmpInterfaceModel.Models.OpSpec SubmitOpToQueue(CmpInterfaceModel.Models.OpSpec opSpec);

        ///
        /// <summary>
        /// Returns a list of the Service provider accounts for the given group 
        /// </summary>
        /// <param name="groupName">Name of the group to find service provider
        /// accounts for</param>
        /// <returns>A list of service provider accounts for the given group</returns>
        /// 
        //*********************************************************************
        List<CmpApiClient.ServiceProviderAccount> FetchServProvAcctList(string groupName);

        //*********************************************************************
        ///
        /// <summary>
        /// Returns a list of the Service Provider Accounts for the given IEnumerable
        /// of IDs to search 
        /// </summary>
        /// <param name="idsToSearch">A set of IDs to look up in the DB
        /// accounts for</param>
        /// <returns>A list of service provider accounts for the given set of IDs
        /// </returns>
        /// 
        //*********************************************************************

        List<CmpApiClient.ServiceProviderAccount> FetchServProvAcctList(IEnumerable<int> idsToSearch);

        //*********************************************************************
        ///
        /// <summary>
        /// Returns a list of all the active Service Provider Accounts
        /// </summary>
        /// <returns>A list of service provider accounts
        /// </returns>
        /// 
        //*********************************************************************

        List<CmpApiClient.ServiceProviderAccount> FetchServProvAcctList();

        CmpApiClient.ServiceProviderAccount InsertServiceProviderAccount(Models.ServiceProviderAccount sPa);
        CmpApiClient.ServiceProviderAccount UpdateServiceProviderAccount(Models.ServiceProviderAccount sPa);

        ///
        /// <summary>
        /// Fetches CMP requests from the CMP database
        /// </summary>
        /// <returns>List of the CMP requests fetched from the CMP database
        /// </returns>
        /// 
        //*********************************************************************
        List<CmpService.VmDeploymentRequest> FetchCmpRequests();

        ///
        /// <summary>
        /// Fetches a CMP request from the CMP database
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request to get 
        /// from the CMP request database
        /// </param>
        /// <returns>The CMP request fetched from the CMP request database
        /// </returns>
        /// 
        //*********************************************************************
        CmpService.VmDeploymentRequest FetchCmpRequest(int cmpRequestId);

        ///
        /// <summary>
        /// Submits a request to the CMP operations database to deploy a new VM
        /// </summary>
        /// <param name="cmpVmRequest">Details of the deployment request
        /// for the new VM</param>
        /// <returns>The deployment request submitted to the CMP request
        /// database</returns>
        /// 
        //*********************************************************************
        CmpService.VmDeploymentRequest SubmitToCmp(CmpVmRequest cmpVmRequest);

        CmpService.VmDeploymentRequest SubmitToCmpStaticTemplate(CmpVmRequest cmpVmRequest, string vmConfig);


        ///
        /// <summary>
        /// DEPRECATED - 
        /// Submits a request to the CMP operations database to deploy a new VM
        /// </summary>
        /// <param name="cmpVmRequest">Details of the deployment request
        /// for the new VM</param>
        /// <returns>The deployment request submitted to the CMP request
        /// database</returns>
        /// 
        //*********************************************************************
        CmpService.VmDeploymentRequest SubmitToCmp_Old(CmpVmRequest cmpVmRequest);

        ///
        /// <summary>
        /// Gets a list of domain information from the CMP DB 
        /// </summary>
        /// <returns>A list of domain information from the CMP DB</returns>
        ///
        //*********************************************************************
        List<Models.AdDomainMap> FetchDomainInfoList();

        ///
        /// <summary>
        /// Gets the IPAK versions
        /// </summary>
        /// <param name="versionName">Version name for the IPAK version</param>
        /// <param name="adDirectory">Ad directory of the IPAK version </param>
        /// <returns>List of the IPAK versions</returns>
        /// 
        //*********************************************************************
        List<Models.IpakVersionMap> FetchIpakVersionMap(
            string versionName, string adDirectory);

        ///
        /// <summary>
        /// Gets the Azure region map from the CMP database
        /// </summary>
        /// <returns>The Azure region map from the CMP database</returns>
        /// 
        //*********************************************************************
        List<Models.AzureRegion> FetchAzureRegionMap();

        /// 
        /// <summary>
        /// Gets the a list of OS information from the CMP database
        /// </summary>
        /// <returns>The list of OS information from the CMP database</returns>
        /// 
        //*********************************************************************
        List<Models.VmOs> FetchOsInfoList();

        CmpInterfaceModel.Models.OpSpec GetVmOpsRequestSpec(string vmName);
    }
}