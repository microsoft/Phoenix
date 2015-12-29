using System;
using System.Collections.Generic;
using CmpCommon;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api
{
    public interface IVMServiceRepository
    {
        ICmpWapDb WapDbContract // parameter injection for database hookup
        { get; set; }

        ICmpApiClient CmpSvProxy { get; set; }

        ///
        /// <summary>
        ///     This method is used to fetch CMP request based on CMP request Id.
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <returns>Deployment request object</returns>
        /// 
        //*********************************************************************
        CmpClient.CmpService.VmDeploymentRequest FetchCmpRequest(int cmpRequestId);

        ///
        /// <summary>
        ///     This method is used to fetch ServiceProviderAccount list.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns>list of ServiceProviderAccount object</returns>
        /// 
        //*********************************************************************
        List<ApiClient.DataContracts.ServiceProviderAccount>
            FetchServiceProviderAccountList(string groupName);

        //*********************************************************************
        ///
        /// <summary>
        ///     Fetches a ServiceProviderAccount list given
        ///     an IEnumerable of Service Provider Account ID
        /// </summary>
        /// <param name="idsToSearch"></param>
        /// <returns>list of ServiceProviderAccount objects</returns>
        /// 
        //*********************************************************************

        IEnumerable<ApiClient.DataContracts.ServiceProviderAccount>
            FetchServiceProviderAccountList(IEnumerable<int> idsToSearch);

        //*********************************************************************
        ///
        /// <summary>
        ///     Fetches all the ServiceProviderAccount
        /// </summary>
        /// <returns>list of ServiceProviderAccount objects</returns>
        /// 
        //*********************************************************************

        IEnumerable<ApiClient.DataContracts.ServiceProviderAccount>
            FetchServiceProviderAccountList();

        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPa"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************
        List<ApiClient.DataContracts.ServiceProviderAccount>
            InsertServiceProviderAccount(ApiClient.DataContracts.ServiceProviderAccount sPa);

        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPa"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************
        List<ApiClient.DataContracts.ServiceProviderAccount>
            UpdateServiceProviderAccount(ApiClient.DataContracts.ServiceProviderAccount sPa);

        ///
        /// <summary>
        /// Get list of disks not associated with a VM
        /// </summary>
        /// <returns>VhdInfo object</returns>
        /// 
        //*********************************************************************
        IEnumerable<VhdInfo> GetDetachedDisks(int? cmpRequestId);

        /// 
        ///  <summary>
        ///     This method is used to delete a VM
        ///  </summary>
        ///  <param name="cmpRequestId"></param>
        /// <param name="deleteFromStorage"></param>
        /// <returns>0</returns>
        ///  
        //*********************************************************************
        int DeleteVm(int cmpRequestId, bool deleteFromStorage);

        /// 
        ///  <summary>
        ///  This method is used to start a VM
        ///  </summary>
        ///  <param name="cmpRequestId"></param>
        /// <returns>0</returns>
        ///  
        //*********************************************************************
        int StartVm(int cmpRequestId);

        ///
        /// <summary>
        /// This methid is used to add Data Disk to a VM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="disks"></param>
        /// <returns>0</returns>
        /// 
        //*********************************************************************
        int AddDisk(int cmpRequestId, List<VhdInfo> disks);

        ///
        /// <summary>
        /// This method is used to Stop a VM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="disks"></param>
        /// <returns>0</returns>
        /// 
        //*********************************************************************
        int StopVm(int cmpRequestId);

        ///
        /// <summary>
        ///     This method is used to get VM dashboard Info
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <returns>VmDashboardInfo</returns>
        /// 
        //*********************************************************************
        VmDashboardInfo GetVm(int cmpRequestId);

        ///
        /// <summary>
        /// This method is used to deallocate VM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="disks"></param>
        /// <returns>0</returns>
        /// 
        //*********************************************************************
        int DeallocateVm(int cmpRequestId);

        ///
        /// <summary>
        /// This method is used to resize VM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="size"></param>
        /// <returns>0</returns>
        /// 
        //*********************************************************************
        int ResizeVM(int cmpRequestId, string size);

        /// 
        ///  <summary>
        ///     This method is used to fetch VM data disk count
        ///  </summary>
        ///  <param name="cmpRequestId"></param>
        /// <param name="roleSizeName"></param>
        /// <returns>data disk count</returns>
        ///  
        //*********************************************************************
        int FetchDiskCount(int cmpRequestId, out string roleSizeName);

        ///
        /// <summary>
        ///     This method is used to restart a VM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <returns>0</returns>
        /// 
        //*********************************************************************
        int RebootVm(int cmpRequestId);

        ///
        /// <summary>
        ///     This method is used to detach a disk from a VM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="disk"></param>
        /// <param name="deleteFromStorage"></param>
        /// 
        //*********************************************************************
        void DetachDisk(int? cmpRequestId, VhdInfo disk, bool deleteFromStorage);

        ///
        /// <summary>
        ///     This method attaches a disk to a given VM.
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="disk"></param>
        /// 
        //*********************************************************************
        void AttachExistingDisk(int? cmpRequestId, VhdInfo disk);

        void PerformAppDataOps(CreateVm createVmModel);

        ///
        /// <summary>
        ///     This method is used to submit VM request fro provisioning.
        /// </summary>
        /// <param name="createVmModel"></param>
        /// <returns>VM object</returns>
        /// 
        //*********************************************************************
        CreateVm SubmitVmRequest(CreateVm createVmModel);

        ///
        /// <summary>
        ///     This method submits VM to Operations queue
        /// </summary>
        /// <param name="opSpec"></param>
        /// <returns>OpSpec Object</returns>
        /// 
        //*********************************************************************
        CmpInterfaceModel.Models.OpSpec SubmitOperation(
            CmpInterfaceModel.Models.OpSpec opSpec);

        CmpInterfaceModel.Models.OpSpec GetVmOpsRequestSpec(string vmName);
        IEnumerable<CreateVm> FetchVms(string subscriptionId);

        void PerformResourceGroupOps(string resGroup);
    }
}
