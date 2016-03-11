//*****************************************************************************
// File: VmsController.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class contains methods that provide VM related information.
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System.Diagnostics;
using Microsoft.WindowsAzurePack.Samples.DataContracts;
using Microsoft.WindowsAzurePack.Samples;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Web.Http.OData;
using System.Text;
using System.Text.RegularExpressions;



namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Controllers
{
    /// <remarks>
    ///     This class contains methods that provide VM related information.
    /// </remarks>
    public class VmsController : ApiController
    {
        private static List<CreateVm> VmList = new List<CreateVm>();
        private static bool _havePendingStatus = false;
        string adminserviceEndpoint = System.Configuration.ConfigurationManager.AppSettings["AdminserviceEndpoint"];
        string windowsAuthSiteEndpoint = System.Configuration.ConfigurationManager.AppSettings["WindowsAuthEndpoint"];

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to list VMs
        /// </summary>
        /// <returns>list of CreateVm Object</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        [EnableQuery]
        public IQueryable<CreateVm> ListVms()
        {
            try
            {
                _havePendingStatus = true;
                lock (VmList)
                {
                 

                    if (_havePendingStatus)
                        FetchVmListFromDb();
                    else if (0 == VmList.Count)
                        FetchVmListFromDb();
                }

                var shares = from share in VmList
                             select share;


            

                return shares.AsQueryable();
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.ListVms()", 100, 1);
                throw;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to retrieve the WApsubscriptions for the given user
        /// </summary>
        /// <returns>Wap Subscription list</returns>
        /// 
        //*********************************************************************
        [HttpGet]
        [ActionName("wapsubs")]
        public async Task<IQueryable<WapSubscription>> subscriptions(string userId)
        {
            //supress certificate error.needs to removed when trusted certs installed in the environment
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

            AdminManagementClient adminClient;
            var token = TokenIssuer.GetWindowsAuthToken(windowsAuthSiteEndpoint, null, null, null, false);
            adminClient = new AdminManagementClient(new Uri(adminserviceEndpoint), token);
            Query query = new Query();

            var result = await adminClient.ListUserSubscriptionsAsync(userId, query);
            var subscriptionlist = result.items.Select(x => new WapSubscription
            {
                AdminEmailId = x.AccountAdminLiveEmailId,
                CoAdminNames = x.CoAdminNames,
                OfferFriendlyName = x.OfferFriendlyName,
                PlanId = x.PlanId,
                SubscriptionID = x.SubscriptionID,
                SubscriptionName = x.SubscriptionName
            }).AsQueryable();

            return subscriptionlist;
          
           
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to list VMs based pon subscription Id
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns>return list of VM</returns>
        /// 
        //*********************************************************************

        [HttpGet]
        [EnableQuery]
        public IQueryable<CreateVm> ListVms(string subscriptionId)
        {
            try
            {
                _havePendingStatus = true;
                lock (VmList)
                {
                    if (_havePendingStatus)
                        FetchVmListFromDb();
                    else if (0 == VmList.Count)
                        FetchVmListFromDb();
                }

                if (string.IsNullOrWhiteSpace(subscriptionId))
                {
                    throw new ArgumentNullException(subscriptionId);
                }

                var shares = from share in VmList.ToList()
                             where string.Equals(share.SubscriptionId, 
                             subscriptionId, StringComparison.OrdinalIgnoreCase)
                             select share;

                return shares.AsQueryable();
            }
            catch(Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.ListVms()", 100, 1);
                throw;
            }
        }


        #region GetVM
        //*********************************************************************
        /// 
        ///  <summary>
        ///     This method is used to get a particular VM based on subscription
        ///     Id and VM Id.
        ///  </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="Id"></param>
        ///  
        //*********************************************************************
        [HttpGet]
        public VmDashboardInfo GetVm(string subscriptionId, int Id)
        {
            try
            {
                ICmpWapDb cwdb = new CmpWapDb();
                ICmpWapDbTenantRepository cwdbTenant = new CmpWapDb();

                var foundVmDepRequest = cwdb.FetchVmDepRequest(Id);
                var vmsizes = cwdbTenant.FetchVmSizeInfoList(subscriptionId);
                if (null != foundVmDepRequest && null != foundVmDepRequest.CmpRequestID)
                {
                    var cmpi = new VMServiceRepository(_eventLog);
                    
                    //when we solve the big 'method not returning' bug: uncomment line below, delete second line below
                    //var vm = cmpi.GetVm(Convert.ToInt32(foundVmDepRequest.CmpRequestID), 
                    //    CmpInterfaceModel.Constants.FetchType.AzureStatus);
                    var vm = GetLocalvmDBI(foundVmDepRequest);
                    cwdb.UpdateVmIp(Id,vm.InternalIP);
                    vm.Cores = vmsizes.Where(x => x.Name == vm.RoleSize).Select(x => x.Cores).FirstOrDefault().ToString();
                    vm.OSVirtualHardDisk.Type = "OS Disk";
                    if(vm.DataVirtualHardDisks != null)
                        vm.DataVirtualHardDisks.Select(d => { d.Type = "Data Disk"; return d; }).ToList();
                    return vm;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.GetVm()", 100, 1);
                throw;
            }
        }

        //********************************************************************
        ///
        /// <summary>
        /// Ugly code, does not belong here. Belongs in a lib. Should remove
        /// this when we solve the big 'method not returning' bug
        /// </summary>
        /// <param name="cmpReq"></param>
        /// <returns></returns>
        /// 
        //********************************************************************

        private VmDashboardInfo GetLocalvmDBI(Models.CmpRequest cmpReq)
        {
            string cmpDbConnectionString = GetCmpContextConnectionStringFromConfig();

            CmpServiceLib.CmpService cmps = new CmpServiceLib.CmpService(eventLog, cmpDbConnectionString, null);
            var vmg = cmps.VmGet((int)cmpReq.CmpRequestID, CmpInterfaceModel.Constants.FetchType.AzureFull);

            VmDashboardInfo vmDBI = new VmDashboardInfo()
            {
                //Cores = "",
                DataVirtualHardDisks = ConvertDisk(vmg.DataVirtualHardDisks),
                DeploymentID = vmg.DeploymentID,
                DNSName = vmg.DNSName,
                InternalIP = vmg.InternalIP,
                MediaLocation = vmg.MediaLocation,
                OSVersion = vmg.OSVersion,
                OSVirtualHardDisk = new OsVirtualHardDisk()
                {
                    DiskLabel = vmg.OSVirtualHardDisk.DiskLabel,
                    DiskName = vmg.OSVirtualHardDisk.DiskName,
                    HostCaching = vmg.OSVirtualHardDisk.HostCaching,
                    MediaLink = vmg.OSVirtualHardDisk.MediaLink,
                    OS = vmg.OSVirtualHardDisk.OS,
                    RemoteSourceImageLink = vmg.OSVirtualHardDisk.RemoteSourceImageLink,
                    SourceImageName = vmg.OSVirtualHardDisk.SourceImageName
                },
                QueueStatus = "",
                RDPCertificateThumbprint = "",
                RoleName = vmg.RoleName,
                RoleSize = cmpReq.VmSize,
                Status = vmg.Status,
                Subscription = new SubscriptionInfo()
                {
                    CurrentCoreCount = vmg.Subscription.CurrentCoreCount,
                    MaximumCoreCount = vmg.Subscription.MaximumCoreCount,
                    SubscriptionID = vmg.Subscription.SubscriptionID,
                    SubscriptionName = vmg.Subscription.SubscriptionName
                }
            };

            return vmDBI;
        }

        //********************************************************************
        ///
        /// <summary>
        /// Ugly code, does not belong here. Belongs in a lib. Should remove
        /// this when we solve the big 'method not returning' bug
        /// </summary>
        /// <param name="dVHDs"></param>
        /// <returns></returns>
        /// 
        //********************************************************************

        private IList<ApiClient.DataContracts.DataVirtualHardDisk> ConvertDisk(
            IList<CmpInterfaceModel.Models.DataVirtualHardDisk> dVHDs)
        {
            var dVHDsOut = new List<ApiClient.DataContracts.DataVirtualHardDisk>();

            if(null == dVHDs)
                return dVHDsOut;

            foreach(var dVHD in dVHDs)
            {
                dVHDsOut.Add(new DataVirtualHardDisk()
                {
                    DiskLabel = dVHD.DiskLabel,
                    DiskName = dVHD.DiskName,
                    HostCaching = dVHD.HostCaching,
                    LogicalDiskSizeInGB = dVHD.LogicalDiskSizeInGB,
                    Lun = dVHD.Lun,
                    MediaLink = dVHD.MediaLink,
                    SourceMediaLink = dVHD.SourceMediaLink,
                    Type = ""
                });
            }

            return dVHDsOut;
        }

        //********************************************************************
        ///
        /// <summary>
        /// Ugly code, does not belong here. Belongs in a lib. Should remove
        /// this when we solve the big 'method not returning' bug
        /// </summary>
        /// <returns></returns>
        /// 
        //********************************************************************

        private string GetCmpContextConnectionStringFromConfig()
        {
            try
            {
                var xk = new KryptoLib.X509Krypto(null);
                return (xk.GetKTextConnectionString("CMPContext", "CMPContextPassword"));
            }
            catch (Exception ex)
            {
                if (null != _eventLog)
                    _eventLog.WriteEntry("Exception when reading CMPContext connection string : " +
                        ex.Message, EventLogEntryType.Error, 100, 100);

                return null;
            }
        }

        //********************************************************************
        ///
        /// <summary>
        /// Ugly code, does not belong here. Belongs in a lib. Should remove
        /// this when we solve the big 'method not returning' bug
        /// </summary>
        /// <param name="cmpReq"></param>
        /// <returns></returns>
        /// 
        //********************************************************************

        private VmDashboardInfo GetLocalvmDBI2(Models.CmpRequest cmpReq)
        {
            VmDashboardInfo vmDBI = new VmDashboardInfo()
            {
                //Cores = "",
                DataVirtualHardDisks = new List<ApiClient.DataContracts.DataVirtualHardDisk>(),
                DeploymentID = "",
                DNSName = cmpReq.TargetVmName + "." + cmpReq.TargetLocation.ToLower().Replace(" ", "") + ".cloudapp.azure.com",
                InternalIP = cmpReq.AddressFromVm,
                MediaLocation = null,
                OSVersion = cmpReq.SourceImageName,
                OSVirtualHardDisk = new OsVirtualHardDisk()
                {
                    DiskLabel = "C",
                    DiskName = "osdisk",
                    HostCaching = "ReadWrite",
                    MediaLink = string.Format("http://{0}.blob.core.windows.net/{1}/{2}.vhd",
                        Utilities.GetXmlInnerText(cmpReq.Config, "newStorageAccountName"),
                        Utilities.GetXmlInnerText(cmpReq.Config, "vmStorageAccountContainerName"),
                        Utilities.GetXmlInnerText(cmpReq.Config, "OSDiskName")),
                    OS = "Windows",
                    RemoteSourceImageLink = null,
                    SourceImageName = null
                },
                QueueStatus = "",
                RDPCertificateThumbprint = "",
                RoleName = cmpReq.TargetVmName,
                RoleSize = cmpReq.VmSize,
                Status = "Running", //*** TODO * MW * This is not good
                Subscription = new SubscriptionInfo()
                {
                    CurrentCoreCount = "0",
                    MaximumCoreCount = "0",
                    SubscriptionID = "",
                    SubscriptionName = ""
                }
            };

            return vmDBI;
        }

        #endregion

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to update a particular VM based on VM Obejct
        ///     and subscription Id.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="vMToUpdate"></param>
        /// 
        //*********************************************************************

        [HttpPut]
        public void UpdateVm(string subscriptionId, CreateVm vMToUpdate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subscriptionId))
                {
                    throw new ArgumentNullException("subscriptionId", @"SubcriptionID is NULL");
                }

                if (vMToUpdate == null)
                {
                    throw new ArgumentNullException("vMToUpdate");
                }

                var vM = (from share in VmList
                          where share.Id == vMToUpdate.Id && string.Equals(share.SubscriptionId, vMToUpdate.SubscriptionId, StringComparison.OrdinalIgnoreCase)
                          select share).FirstOrDefault();

                if (vM != null)
                {
                    throw new Exception("vM not found");
                }

                vM.Name = vMToUpdate.Name;
                //vM.FileServerName = vMToUpdate.FileServerName;
                //vM.Size = vMToUpdate.Size;
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.UpdateVm()", 100, 1);
                throw;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to create a particular VM.
        /// </summary>
        /// <param name="vM"></param>
        /// 
        //*********************************************************************

        [HttpPost]
        public async Task<HttpResponseMessage> CreateVm([FromBody] CreateVm vM)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    var result = new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = new StringContent(
                            "Model State is not valid", Encoding.UTF8,
                            "application/json"),
                        ReasonPhrase = "ModelState is not valid"
                    };
                    return result;
                }

                LogThis(EventLogEntryType.Information, "VM Create Request Submitted", 2, 1);

                vM.CreatedBy = await GetWapAdmin(vM.SubscriptionId);

                var cwdb = new CmpWapDb();

                //*** Map WAP sub to ResProvGroupId

                var resourceProviderGroupName =
                    cwdb.FetchDefaultResourceProviderGroupName(vM.SubscriptionId);

                if (null == resourceProviderGroupName)
                    throw new Exception("Could not locate DefaultResourceProviderGroupName for WAP subscription");

                vM.EnvResourceGroupName = resourceProviderGroupName;

                var cmpi = new VMServiceRepository(_eventLog);

                lock (vM)
                {
                    //Insert app data to the DB 
                    cmpi.PerformAppDataOps(new CreateVm { VmAppName = vM.VmAppName, 
                        VmAppId = vM.VmAppId, 
                        SubscriptionId = vM.SubscriptionId, 
                        AccountAdminLiveEmailId = vM.AccountAdminLiveEmailId,
                        VmRegion = vM.VmRegion
                    });
                    //Submit VM information to the WAP DB
                    vM = cmpi.SubmitVmRequest(vM);
                    AddVmToList(vM);
                }

                LogThis(EventLogEntryType.Information, "VM Create Request Submitted OK", 2, 2);

                //return Ok(vM);
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                LogThis(ex, EventLogEntryType.Error, "CmpWapExtension.VmsController.CreateVm()", 100, 1);

                //return InternalServerError(ex);
                //throw new Microsoft.WindowsAzurePack.CmpWapExtension.Common.PortalException(ex.Message);
                var reason = "Exception while submitting request to Create VM : " +
                    Regex.Replace(CmpCommon.Utilities.UnwindExceptionMessages(ex), @"\t|\n|\r", "");

                // Making this as a bad request instead of Internal Server error because the reason phrase for Internal Server error is not able to be customized.
                var result = new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = reason };
                return result;
            }
        }

   

        #region --- Helpers ---------------------------------------------------
        //*********************************************************************
        ///
        /// <summary>
        /// This method is used to retrieve WAPadmin based on subscriptionId
        /// From WAPUI it is possible to pass WAPAdmin but from external UIs(who consumes this API) its not possible,so added this method
        /// </summary>
        /// <param name="vMId"></param>
        /// 
        //*********************************************************************


        private async Task<string> GetWapAdmin(string wapSubscriptionId)
        {
            //*** TODO * Markw * Getting accessdenied on this call
            return wapSubscriptionId;

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

            AdminManagementClient adminClient;
            var token = TokenIssuer.GetWindowsAuthToken(windowsAuthSiteEndpoint, null, null, null, false);
            adminClient = new AdminManagementClient(new Uri(adminserviceEndpoint), token);
            Query query = new Query();

            var result = await adminClient.GetSubscriptionAsync(wapSubscriptionId);

            return result.AccountAdminLiveEmailId;
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to remove VM from VM list.
        /// </summary>
        /// <param name="vMId"></param>
        /// 
        //*********************************************************************

        public static void RemoveVmFromList(int vMId)
        {
            if (null == VmList)
                return;

            foreach (CreateVm cVm in VmList.ToList())
                if (cVm.Id == vMId)
                    VmList.Remove(cVm);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to add VM to VM list.
        /// </summary>
        /// <param name="cmpReq"></param>
        /// 
        //*********************************************************************

        private void AddVmToList(Models.CmpRequest cmpReq)
        {
            if (!cmpReq.StatusCode.Equals(CmpInterfaceModel.Constants.StatusEnum.Complete.ToString()))
                _havePendingStatus = true;

            VmList.Add(item: new CreateVm
            {
                Id = cmpReq.Id,
                //Id = VmList.Count,
                CmpRequestId = Convert.ToInt32(cmpReq.CmpRequestID),
                Name = cmpReq.TargetVmName,
                SubscriptionId = cmpReq.WapSubscriptionID,
                StatusCode = cmpReq.StatusCode,
                StatusMessage = cmpReq.StatusMessage,
                ExceptionMessage=cmpReq.ExceptionMessage,
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
            });
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to add VM to VM list.
        /// </summary>
        /// <param name="vM"></param>
        /// 
        //*********************************************************************

        private void AddVmToList(CreateVm vM)
        {
            if (!vM.StatusCode.Equals(CmpInterfaceModel.Constants.StatusEnum.Complete.ToString()))
                _havePendingStatus = true;

            VmList.Add(vM);
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to fetch VM list from WAP DB.
        /// </summary>
        /// 
        //*********************************************************************

        private void FetchVmListFromDb()
        {
            var cwdb = new CmpWapDb();
            var cmpReqList = cwdb.FetchVmDepRequests(null, true);

            _havePendingStatus = false;
            VmList.Clear();

            foreach (Models.CmpRequest cmpReq in cmpReqList)
                AddVmToList(cmpReq);

            SyncWithCmp();
        }

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to get VM status.
        /// </summary>
        /// <param name="vdr"></param>
        /// <returns>VM status message string</returns>
        /// 
        //*********************************************************************

        string GetStatusMessage(CmpClient.CmpService.VmDeploymentRequest vdr)
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

        //*********************************************************************
        ///
        /// <summary>
        ///     This method is used to Sync VM Object with CMP.
        /// </summary>
        /// 
        //*********************************************************************

        void SyncWithCmp()
        {
            var cmpi = new VMServiceRepository(_eventLog);
            var cwdb = new CmpWapDb();

            foreach (CreateVm vM in VmList.ToList())
            {
                //*** If status is complete or exception then don't process ***
                //if (vM.StatusCode.Equals(CmpInterfaceModel.Constants.StatusEnum.Complete.ToString()) ||
                //    vM.StatusCode.Equals(CmpInterfaceModel.Constants.StatusEnum.Exception.ToString()))

                if (vM.StatusCode.Equals(CmpInterfaceModel.Constants.StatusEnum.Complete.ToString()))
                    continue;

                try
                {
                    //*** Access the CMP service ***
                    var fetchedVm = cmpi.FetchCmpRequest(vM.CmpRequestId);

                    //*** update CmpWapDb if status has changed ***
                    //if (!vM.StatusCode.Equals(fetchedVm.Status))

            if(true)

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

        #endregion

        #region --- Utilities -------------------------------------------------

        static EventLog _eventLog = null;

        /// <summary></summary>
        public static EventLog eventLog 
        { 
            set { _eventLog = value; }
            get 
            {
                if (null == _eventLog)
                {
                    try
                    {
                        _eventLog = new EventLog("Application")
                        {
                            Source = CmpCommon.Constants.CmpWapConnector_EventlogSourceName
                        };
                    }
                    catch(Exception)
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
    }
}
