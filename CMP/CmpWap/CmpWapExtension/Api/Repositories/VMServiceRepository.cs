//*****************************************************************************
// File: CmpInterface.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class contains methods that act as interface for making calls
//          outside of the current project namespace.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.Collections.Generic;
using Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient;
using Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System;
using System.Linq;
using System.Diagnostics;
using CmpCommon;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api
{
    public class VMServiceRepository : IVMServiceRepository
    {
        static EventLog _eventLog = null;
        /// <summary>
        /// Set Database contract here
        /// </summary>        
        private ICmpWapDb _cwdb;
        public ICmpWapDb WapDbContract // parameter injection for database hookup
        {
            get 
            {
                if(_cwdb == null)
                {
                    _cwdb = new CmpWapDb(); // take built-in WAP DB by default, else swap with your DB
                }
                return _cwdb; 
            }
            set 
            { 
                _cwdb = value; 
            }
        }

        private ICmpApiClient _cmp;

        public ICmpApiClient CmpSvProxy
        {
            get
            {
                if (_cmp == null)
                {
                    _cmp = new CmpApiClient(eventLog);
                }
                return _cmp;
            }
            set { _cmp = value; }
        }
        



        //*********************************************************************
        ///
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="eventLog"></param>
        /// 
        //*********************************************************************

        public VMServiceRepository(EventLog eventLog)
        {
            _eventLog = eventLog;
        }

        public VMServiceRepository()
        {
            // TODO: Complete member initialization
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to fetch CMP request based on CMP request Id.
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <returns>Deployment request object</returns>
        /// 
        //*********************************************************************

        public CmpClient.CmpService.VmDeploymentRequest FetchCmpRequest(int cmpRequestId)
        {
            var cmp = CmpSvProxy;
            var vmReqResponse = cmp.FetchCmpRequest(cmpRequestId);

            /*var createVmModel = new CreateVm
            {
                StatusCode = vmReqResponse.Status,
                StatusMessage = vmReqResponse.StatusMessage
            };*/

            return vmReqResponse;
        }


        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to return ServiceProviderAccount object.
        /// </summary>
        /// <param name="spa"></param>
        /// <returns>ServiceProviderAccount object</returns>
        /// 
        //*********************************************************************

        ApiClient.DataContracts.ServiceProviderAccount Translate(
            CmpClient.Models.ServiceProviderAccount spa)
        {
            return new ApiClient.DataContracts.ServiceProviderAccount
            {
                ID = spa.ID,
                AccountID = spa.AccountID,
                AccountPassword = spa.AccountPassword,
                AccountType = spa.AccountType,
                Active = (bool)spa.Active,
                AzAffinityGroup = spa.AzAffinityGroup,
                AzRegion = spa.AzRegion,
                AzStorageContainerUrl = spa.AzStorageContainerUrl,
                AzSubnet = spa.AzSubnet,
                AzVNet = spa.AzVNet,
                CertificateBlob = spa.CertificateBlob,
                CertificateThumbprint = spa.CertificateThumbprint,
                Config = spa.Config,
                CoreCountCurrent = (int)spa.CoreCountCurrent,
                CoreCountMax = (int)spa.CoreCountMax,
                Description = spa.Description,
                ExpirationDate = (DateTime)spa.ExpirationDate,
                Name = spa.Name,
                OwnerNamesCSV = spa.OwnerNamesCSV,
                ResourceGroup = spa.ResourceGroup,
                TagData = spa.TagData
            };
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spa"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        CmpClient.Models.ServiceProviderAccount Translate(
            ApiClient.DataContracts.ServiceProviderAccount spa)
        {
            return new CmpClient.Models.ServiceProviderAccount
            {
                ID = spa.ID,
                AccountID = spa.AccountID,
                AccountPassword = spa.AccountPassword,
                AccountType = spa.AccountType,
                Active = (bool)spa.Active,
                AzAffinityGroup = spa.AzAffinityGroup,
                AzRegion = spa.AzRegion,
                AzStorageContainerUrl = spa.AzStorageContainerUrl,
                AzSubnet = spa.AzSubnet,
                AzVNet = spa.AzVNet,
                CertificateBlob = spa.CertificateBlob,
                CertificateThumbprint = spa.CertificateThumbprint,
                Config = spa.Config,
                CoreCountCurrent = (int)spa.CoreCountCurrent,
                CoreCountMax = (int)spa.CoreCountMax,
                Description = spa.Description,
                ExpirationDate = (DateTime)spa.ExpirationDate,
                Name = spa.Name,
                OwnerNamesCSV = spa.OwnerNamesCSV,
                ResourceGroup = spa.ResourceGroup,
                TagData = spa.TagData,
                AzureADClientId = spa.ClientID,
                AzureADTenantId = spa.TenantID,
                AzureADClientKey = spa.ClientKey
            };
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spa"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        ApiClient.DataContracts.ServiceProviderAccount Translate(
            CmpApiClient.ServiceProviderAccount spa)
        {
            return new ApiClient.DataContracts.ServiceProviderAccount
            {
                ID = spa.ID,
                AccountID = spa.AccountID,
                AccountPassword = spa.AccountPassword,
                AccountType = spa.AccountType,
                Active = (bool)spa.Active,
                AzAffinityGroup = spa.AzAffinityGroup,
                AzRegion = spa.AzRegion,
                AzStorageContainerUrl = spa.AzStorageContainerUrl,
                AzSubnet = spa.AzSubnet,
                AzVNet = spa.AzVNet,
                CertificateBlob = spa.CertificateBlob,
                CertificateThumbprint = spa.CertificateThumbprint,
                Config = spa.Config,
                CoreCountCurrent = (int)spa.CoreCountCurrent,
                CoreCountMax = (int)spa.CoreCountMax,
                Description = spa.Description,
                ExpirationDate = (DateTime)spa.ExpirationDate,
                Name = spa.Name,
                OwnerNamesCSV = spa.OwnerNamesCSV,
                ResourceGroup = spa.ResourceGroup,
                TagData = spa.TagData
            };
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     Fetches a ServiceProviderAccount list given
        ///     a group name
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns>list of ServiceProviderAccount object</returns>
        /// 
        //*********************************************************************

        public List<ApiClient.DataContracts.ServiceProviderAccount>
            FetchServiceProviderAccountList(string groupName)
        {
            var cmp = CmpSvProxy;
            var spaListOut = new List<ApiClient.DataContracts.ServiceProviderAccount>();
            var spaList = cmp.FetchServProvAcctList(groupName);

            foreach (var spa in spaList)
                spaListOut.Add(Translate(spa));

            return spaListOut;
        }

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

        public IEnumerable<ApiClient.DataContracts.ServiceProviderAccount>
            FetchServiceProviderAccountList(IEnumerable<int> idsToSearch)
        {
            var cmp = CmpSvProxy;
            var spaListResult = new List<ApiClient.DataContracts.ServiceProviderAccount>();
            var spaList = cmp.FetchServProvAcctList(idsToSearch);

            foreach (var spa in spaList)
                spaListResult.Add(Translate(spa));

            return spaListResult;
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     Fetches all the ServiceProviderAccount
        /// </summary>
        /// <returns>list of ServiceProviderAccount objects</returns>
        /// 
        //*********************************************************************

        public IEnumerable<ApiClient.DataContracts.ServiceProviderAccount>
            FetchServiceProviderAccountList()
        {
            var cmp = CmpSvProxy;
            var spaListResult = new List<ApiClient.DataContracts.ServiceProviderAccount>();
            var spaList = cmp.FetchServProvAcctList();

            foreach (var spa in spaList)
                spaListResult.Add(Translate(spa));

            return spaListResult;
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

        public List<ApiClient.DataContracts.ServiceProviderAccount>
            InsertServiceProviderAccount(ApiClient.DataContracts.ServiceProviderAccount sPa)
        {
            var cmp = CmpSvProxy;
            var spaListOut = new List<ApiClient.DataContracts.ServiceProviderAccount>();
            var spaOut = cmp.InsertServiceProviderAccount(Translate(sPa));

            spaListOut.Add(Translate(spaOut));

            return spaListOut;
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

        public List<ApiClient.DataContracts.ServiceProviderAccount>
            UpdateServiceProviderAccount(ApiClient.DataContracts.ServiceProviderAccount sPa)
        {
            var cmp = CmpSvProxy;
            var spaListOut = new List<ApiClient.DataContracts.ServiceProviderAccount>();
            var spaOut = cmp.UpdateServiceProviderAccount(Translate(sPa));

            spaListOut.Add(Translate(spaOut));

            return spaListOut;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Get list of disks not associated with a VM
        /// </summary>
        /// <returns>VhdInfo object</returns>
        /// 
        //*********************************************************************

        public IEnumerable<VhdInfo> GetDetachedDisks(int? cmpRequestId)
        {
            var cmp = CmpSvProxy;
            return cmp.GetDetachedDisks(cmpRequestId);
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///     This method is used to delete a VM
        ///  </summary>
        ///  <param name="cmpRequestId"></param>
        /// <param name="deleteFromStorage"></param>
        /// <returns>0</returns>
        ///  
        //*********************************************************************

        public int DeleteVm(int cmpRequestId, bool deleteFromStorage)
        {
            var cmp = CmpSvProxy;
            var vmReqResponse = cmp.DeleteVm(cmpRequestId, deleteFromStorage);

            return 0;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  This method is used to start a VM
        ///  </summary>
        ///  <param name="cmpRequestId"></param>
        /// <returns>0</returns>
        ///  
        //*********************************************************************

        public int StartVm(int cmpRequestId)
        {
            var cmp = CmpSvProxy;
            var vmReqResponse = cmp.StartVm(cmpRequestId);

            return 0;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// This methid is used to add Data Disk to a VM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="disks"></param>
        /// <returns>0</returns>
        /// 
        //*********************************************************************

        public int AddDisk(int cmpRequestId, List<VhdInfo> disks)
        {
            var cmp = CmpSvProxy;
            var vmReqResponse = cmp.AddDisk(cmpRequestId, disks);

            return 0;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// This method is used to Stop a VM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="disks"></param>
        /// <returns>0</returns>
        /// 
        //*********************************************************************

        public int StopVm(int cmpRequestId)
        {
            var cmp = CmpSvProxy;
            var vmReqResponse = cmp.StopVm(cmpRequestId);

            return 0;
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to change type from VMInfo to VM Dashboard Info.
        /// </summary>
        /// <param name="vm"></param>
        /// <returns>VmDashboardInfo object</returns>
        /// 
        //*********************************************************************

        VmDashboardInfo Translatevm(CmpApiClient.VmInfo vm)
        {
            return new VmDashboardInfo()
            {
                DataVirtualHardDisks = (vm.DataVirtualHardDisks == null) ? null : 
                    vm.DataVirtualHardDisks.Select(x => new DataVirtualHardDisk
                    {
                        DiskLabel=x.DiskLabel,
                        DiskName=x.DiskName,
                        HostCaching=x.HostCaching,
                        LogicalDiskSizeInGB=x.LogicalDiskSizeInGB,
                        Lun=x.Lun,
                        MediaLink=x.MediaLink,
                        SourceMediaLink=x.SourceMediaLink

                    }).ToList(),
                OSVirtualHardDisk = new OsVirtualHardDisk 
                { 
                    DiskLabel=vm.OSVirtualHardDisk.DiskLabel,
                    DiskName=vm.OSVirtualHardDisk.DiskName,
                    HostCaching=vm.OSVirtualHardDisk.HostCaching,
                    MediaLink=vm.OSVirtualHardDisk.MediaLink,
                    OS=vm.OSVirtualHardDisk.OS,
                    RemoteSourceImageLink=vm.OSVirtualHardDisk.RemoteSourceImageLink,
                    SourceImageName=vm.OSVirtualHardDisk.SourceImageName
                },
                RDPCertificateThumbprint = vm.RDPCertificateThumbprint,
                InternalIP = vm.InternalIP,
                DeploymentID = vm.DeploymentID,
                DNSName = vm.DNSName,
                RoleName = vm.RoleName,
                RoleSize = vm.RoleSize,
                Status = vm.Status,
                Subscription = new SubscriptionInfo 
                {
                    CurrentCoreCount=vm.Subscription.CurrentCoreCount,
                    MaximumCoreCount=vm.Subscription.MaximumCoreCount,
                    SubscriptionID=vm.Subscription.SubscriptionID,
                    SubscriptionName=vm.Subscription.SubscriptionName
                },
                QueueStatus = vm.QueueStatus, // get queue status of the VM
                MediaLocation = vm.MediaLocation, 
                OSVersion = vm.OSVersion 
            };
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to get VM dashboard Info
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <returns>VmDashboardInfo</returns>
        /// 
        //*********************************************************************

        public VmDashboardInfo GetVm(int cmpRequestId, CmpInterfaceModel.Constants.FetchType fetchType )
        {
            var cmp = CmpSvProxy;
            var vmInfo = cmp.GetVm(cmpRequestId, fetchType);


            var vm = Translatevm(vmInfo);

            return vm;
            //return 0;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// This method is used to deallocate VM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="disks"></param>
        /// <returns>0</returns>
        /// 
        //*********************************************************************

        public int DeallocateVm(int cmpRequestId)
        {
            var cmp = CmpSvProxy;
            var vmReqResponse = cmp.DeallocateVm(cmpRequestId);

            return 0;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// This method is used to resize VM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="size"></param>
        /// <returns>0</returns>
        /// 
        //*********************************************************************

        public int ResizeVM(int cmpRequestId, string size)
        {
            var cmp = CmpSvProxy;
            var vmReqResponse = cmp.Resize(cmpRequestId, size);

            return 0;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///     This method is used to fetch VM data disk count
        ///  </summary>
        ///  <param name="cmpRequestId"></param>
        /// <param name="roleSizeName"></param>
        /// <returns>data disk count</returns>
        ///  
        //*********************************************************************

        public int FetchDiskCount(int cmpRequestId, out string roleSizeName)
        {
            var cmp = CmpSvProxy;
            return cmp.FetchDiskCount(cmpRequestId, out roleSizeName);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to restart a VM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <returns>0</returns>
        /// 
        //*********************************************************************

        public int RebootVm(int cmpRequestId)
        {
            var cmp = CmpSvProxy;
            var vmReqResponse = cmp.RebootVm(cmpRequestId);

            return 0;
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to detach a disk from a VM
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="disk"></param>
        /// <param name="deleteFromStorage"></param>
        /// 
        //*********************************************************************

        public void DetachDisk(int? cmpRequestId, VhdInfo disk, bool deleteFromStorage)
        {
            var cmp = CmpSvProxy;
            cmp.DetachDisk(cmpRequestId, disk, deleteFromStorage);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method attaches a disk to a given VM.
        /// </summary>
        /// <param name="cmpRequestId"></param>
        /// <param name="disk"></param>
        /// 
        //*********************************************************************

        public void AttachExistingDisk(int? cmpRequestId, VhdInfo disk)
        {
            var cmp = CmpSvProxy;
            cmp.AttachExistingDisk(cmpRequestId, disk);
        }

        public void PerformAppDataOps(CreateVm createVmModel)
        {
            var cmpWapDb = WapDbContract;

            if (!WapDbContract.CheckAppDataRecord(createVmModel))
            {
                WapDbContract.InsertAppDataRecord(createVmModel);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to submit VM request fro provisioning.
        /// </summary>
        /// <param name="createVmModel"></param>
        /// <returns>VM object</returns>
        /// 
        //*********************************************************************

        public CreateVm SubmitVmRequest(CreateVm createVmModel)
        {
            bool Ismsitmonitored;
            var reqRec = new CmpVmBuildRequest
            {
                RequestID = 0,
                AppID = createVmModel.VmAppId,
                AppName = createVmModel.VmAppName,
                RequestName = createVmModel.Name,
                AzureRegionName = createVmModel.VmRegion,
                CreatedBy = createVmModel.CreatedBy,
                OSName = createVmModel.VmSourceImage,
                RequestAdmins=createVmModel.VmAdminName,
                VmAdminName = createVmModel.VmAdminName,
                VmAdminPassword = createVmModel.VmAdminPassword,
                ResourceGroup=createVmModel.EnvResourceGroupName,
                SKU_CustomerName = createVmModel.VmSize,
                StorageConfigXML = createVmModel.VmDiskSpec,
                ActiveDirectoryDomain = createVmModel.VmDomain,
                ITSMServiceCategory = createVmModel.ServiceCategory,
                EnvironmentName = createVmModel.EnvironmentClass,
                NIC1_Config = createVmModel.Nic1,
                ITSMMonitoredFlag = Boolean.TryParse(createVmModel.Msitmonitored, out Ismsitmonitored) == true ? Ismsitmonitored : false,
                SQLBuildOut = createVmModel.sqlconfig == null ? false : createVmModel.sqlconfig.InstallSql,
                SQLAdminGroup = createVmModel.sqlconfig == null ? null : createVmModel.sqlconfig.AdminGroups,
                SQLInstanceName = createVmModel.sqlconfig == null ? null : createVmModel.sqlconfig.SqlInstancneName,
                SQLVersionName = createVmModel.sqlconfig == null ? null : createVmModel.sqlconfig.Version,
                SQLEnableReplication = createVmModel.sqlconfig == null ? false : createVmModel.sqlconfig.InstallAnalysisServices,
                SQLEnableSSAS = createVmModel.sqlconfig == null ? false : createVmModel.sqlconfig.InstallAnalysisServices,
                SQLEnableSSIS= createVmModel.sqlconfig == null ? false : createVmModel.sqlconfig.InstallIntegrationServices,
                SQLSSASMode = createVmModel.sqlconfig == null ? null : createVmModel.sqlconfig.AnalysisServicesMode,
                SQLCollation = createVmModel.sqlconfig == null ? null : createVmModel.sqlconfig.Collation,
                IISBuildOut = createVmModel.IIsconfig == null ? false : createVmModel.IIsconfig.InstallIis,
                IISServiceRole = createVmModel.IIsconfig == null ? null : createVmModel.IIsconfig.RoleServices,
                DateSubmitted = DateTime.UtcNow,
                OSCode = createVmModel.OsCode,
                AzureApiName = createVmModel.AzureApiName,
                AzureImagePublisher = "",
                AzureImageOffer = "",
                AzureWindowsOSVersion =""
            };

            var ecsPortalDB = new ECSPortalDB();
            var ecsSubscription = ecsPortalDB.FetchSubscription(System.Guid.Parse(createVmModel.SubscriptionId));
            
            // This has been commented out for debugging purposes to remove exceptions occuring due to no ITSM Records.
            // Please uncomment and delete the code below once ITSM has been integrated.
           if(ecsSubscription!=null)
            { 
                reqRec.OrgFinancialAssetOwner = ecsSubscription.FinancialOwner;
                reqRec.OrgChargebackGroup = ecsSubscription.ChargeBackGroup;
                reqRec.OrgID = ecsSubscription.OrganizationID;
                reqRec.ITSMResponsibleOwner = ecsSubscription.ResponsibleOwner;
                reqRec.ITSMAccountableOwner = ecsSubscription.CIOwner;
                reqRec.ITSMCIOwner = ecsSubscription.CIOwner;
            }
            
            var vmReq = new CmpVmRequest
            {
                RequestRecord = reqRec
            };

            // Insert data to CmpDb
            var cmp = CmpSvProxy;
            var vmReqResponse = cmp.SubmitToCmp(vmReq);
            createVmModel.CmpRequestId = vmReqResponse.ID;
            createVmModel.ExceptionMessage =vmReqResponse.ExceptionMessage;

            // Insert data into WapDB
            WapDbContract.InsertVmDepRequest(createVmModel);

            return createVmModel;
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method submits VM to Operations queue
        /// </summary>
        /// <param name="opSpec"></param>
        /// <returns>OpSpec Object</returns>
        /// 
        //*********************************************************************

        public CmpInterfaceModel.Models.OpSpec SubmitOperation(
            CmpInterfaceModel.Models.OpSpec opSpec)
        {
            var cmp = CmpSvProxy;
            return cmp.SubmitOpToQueue(opSpec);
        }


        public CmpInterfaceModel.Models.OpSpec GetVmOpsRequestSpec(string vmName)
        {
            var cmp = CmpSvProxy;
            var oprequest = cmp.GetVmOpsRequestSpec(vmName);
            return oprequest;
        }

        public IEnumerable<CreateVm> FetchVms(string subscriptionId)
        {

            var cwdb = WapDbContract;
            var vmlist = new List<CreateVm>();
            var cmpReqList = cwdb.FetchVmDepRequests(null, true, subscriptionId);
            foreach (Models.CmpRequest cmpReq in cmpReqList.ToList())
                vmlist.Add(MarshallVmInfo(cmpReq));

            SyncWithCmp(vmlist);
            return vmlist;
        }


        #region --- Utilities -------------------------------------------------

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmpReq"></param>
        /// 
        //*********************************************************************

        private CreateVm MarshallVmInfo(Models.CmpRequest cmpReq)
        {
            return new CreateVm
            {
                Id = cmpReq.Id,
                //Id = VmList.Count,
                CmpRequestId = Convert.ToInt32(cmpReq.CmpRequestID),
                Name = cmpReq.TargetVmName,
                SubscriptionId = cmpReq.WapSubscriptionID,
                StatusCode = cmpReq.StatusCode,
                StatusMessage = cmpReq.StatusMessage,
                ExceptionMessage = cmpReq.ExceptionMessage,
                VmAppName = cmpReq.ParentAppName,
                VmDomain = cmpReq.Domain,
                VmAdminName = "",
                //VmAdminPassword = "",
                VmSourceImage = cmpReq.SourceImageName,
                VmSize = cmpReq.VmSize,
                VmRegion = cmpReq.TargetLocation,
                VmRole = "",
                VmDiskSpec = "",
                VmConfig = cmpReq.Config,
                VmTagData = cmpReq.TagData,
                AddressFromVm = cmpReq.AddressFromVm
                //ExtensionData = ""
            };
        }

        private void SyncWithCmp(List<CreateVm> vmlist)
        {
            var cmpi = new VMServiceRepository(_eventLog);
            var cwdb = WapDbContract;

            foreach (CreateVm vM in vmlist)
            {

                if (vM.StatusCode.Equals(CmpInterfaceModel.Constants.StatusEnum.Complete.ToString()))
                    continue;

                try
                {
                    //*** Access the CMP service ***
                    var fetchedVm = cmpi.FetchCmpRequest(vM.CmpRequestId);

                    //*** update CmpWapDb if status has changed ***
                    //if (!vM.StatusCode.Equals(fetchedVm.Status))

                    if (true)
                    {
                        vM.StatusCode = fetchedVm.Status;
                        vM.StatusMessage = fetchedVm.StatusMessage;

                        var cmpWapReq = new Models.CmpRequest
                        {
                            StatusCode = fetchedVm.Status,
                            StatusMessage = GetStatusMessage(fetchedVm),
                            ExceptionMessage = fetchedVm.ExceptionMessage,
                            Id = vM.Id,
                            Config = fetchedVm.Config
                        };

                        if (null != fetchedVm.Config)
                        {
                            var vmc = CmpInterfaceModel.Models.VmConfig.Deserialize(fetchedVm.Config);
                            if (null != vmc)
                                if (null != vmc.InfoFromVM)
                                    cmpWapReq.AddressFromVm = vmc.InfoFromVM.VmAddress;
                        }

                        cwdb.SetVmDepRequestStatus(cmpWapReq, null);
                    }
                }
                catch (Exception)
                {
                    vM.StatusCode = "NoCmpContact";
                    vM.StatusMessage = "Temporarily unable to contact CMP for status";
                }
            }
        }

        private string GetStatusMessage(CmpClient.CmpService.VmDeploymentRequest vdr)
        {
            if (null != vdr.ExceptionMessage)
                if (vdr.Status == CmpInterfaceModel.Constants.StatusEnum.Exception.ToString() ||
                    vdr.Status == CmpInterfaceModel.Constants.StatusEnum.Rejected.ToString())
                {
                    string messageOut = CmpInterfaceModel.Utilities.GetXmlInnerText(vdr.ExceptionMessage, "Message");

                    if (null == messageOut)
                        messageOut = vdr.ExceptionMessage;

                    return messageOut;
                }

            return vdr.StatusMessage;
        }

        /// <summary></summary>
        private static EventLog eventLog
        {
            set { _eventLog = value; }
            get
            {
                if (null == _eventLog)
                {
                    try
                    {
                        _eventLog = new EventLog("Application");
                        _eventLog.Source = CmpCommon.Constants.CmpWapConnector_EventlogSourceName;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }

                return _eventLog;
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
                if (null != eventLog)
                    eventLog.WriteEntry(prefix + " : " +
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex), type, id, category);
            }
            catch (Exception ex2)
            { string x = ex2.Message; }
        }
        private void LogThis(EventLogEntryType type, string message,
            int id, short category)
        {
            try
            {
                if (null != eventLog)
                    eventLog.WriteEntry(message, type, id, category);
            }
            catch (Exception ex2)
            { string x = ex2.Message; }
        }

        #endregion


        public void PerformResourceGroupOps(string resGroup)
        {
            var cwdb = WapDbContract;
            if (!cwdb.CheckResourceGroupExists(resGroup))
            {
                cwdb.InsertResourceProviderAcctGroup(resGroup);
            }
        }
    }
}
