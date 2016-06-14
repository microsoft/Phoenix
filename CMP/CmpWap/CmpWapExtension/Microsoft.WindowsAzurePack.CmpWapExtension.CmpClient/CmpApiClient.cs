//*****************************************************************************
//
// Purpose: A library to interface with the CMP (Cloud Management Platform)
//          service, which creates and manages Azure public cloud VMs.
// Author: Mark West (mark.west@microsoft.com)
// Copyright: Microsoft 2013
//
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Threading;
using CmpServiceLib;
using CmpCommon;
using CmpInterfaceModel.Models;
using Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models;
using Constants = CmpInterfaceModel.Constants;


/*to do: remove commented code blocks and empty methods*/

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient
{
    //*************************************************************************
    /// <summary>User info for a VM</summary>
    //*************************************************************************
    /* todo: move this class into a separate file */
    public class VmUserInfo
    {
        public string UserName;
        public string Password;
    }

    //*************************************************************************
    /// <summary>Request object for a CMP VM</summary>
    //*************************************************************************
    /* todo: move this class into a separate file */
    public class CmpVmRequest
    {
        public Models.CmpVmBuildRequest RequestRecord;
    }

    //*************************************************************************
    /// 
    /// <summary> 
    /// 
    /// </summary>
    /// 
    //*************************************************************************
    public class CmpApiClient : ICmpApiClient
    {
        /// <summary>
        /// CMP access modes
        /// </summary>
        public enum CmpAccessModeEnum
        {
            Service,
            Monolith
        };

        /// <summary>
        /// Image types of the source
        /// </summary>
        public enum SourceImageTypeEnum
        {
            Unavailable,
            OsImage,
            OsVhd
        };

        public CmpAccessModeEnum CmpAccessMode = CmpAccessModeEnum.Monolith;

        static EventLog _eventLog = null;
        static string _cmpDbConnectionString = null;
        static Random _storageAccntRandomNum = new Random();

        /// <summary>
        /// Event log for logging
        /// </summary>
        public static EventLog EventLog { set { _eventLog = value; } }

        /// <summary>
        /// Connection string for the CMP database
        /// </summary>
        public string CmpDbConnectionString
        {
            set { _cmpDbConnectionString = value; }
            get
            {
                if (null == _cmpDbConnectionString)
                    _cmpDbConnectionString = GetCmpContextConnectionStringFromConfig();

                return _cmpDbConnectionString;
            }
        }
   
        private readonly char[] _ignoreDriveList = { 'C' };
        private System.Net.NetworkCredential _cmpClientCredentials = null;
        List<Models.VmOs> _VmOsMap = null;


        //*********************************************************************
        ///
        /// <summary>
        /// Credentials for the CMP client
        /// </summary>
        ///
        //*********************************************************************
        public System.Net.NetworkCredential CmpClientCredentials
        {
            get
            {
                //if (null == _cmpClientCredentials)
                //    _cmpClientCredentials = GetCmpClientCredsFromConfig();

                return _cmpClientCredentials;
            }
            set
            {
                _cmpClientCredentials = value;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Constructor; initializes an event log for the CMP client
        /// </summary>
        /// <param name="eventLog">Event log for the CMP Api client</param>
        ///
        //*********************************************************************
        public CmpApiClient(EventLog eventLog)
        {
            _eventLog = eventLog;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets the  CMP Context string with encrypted password from config 
        /// file and decrypts the password
        /// </summary>
        /// <returns>The connection string for for the CMP context</returns>
        /// 
        //*********************************************************************

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

        //*********************************************************************
        ///
        /// <summary>
        /// Gets the encrypted CMP client credentials from config and decrypts
        /// the credentials using the KryptoCert certificate.
        /// </summary>
        /// <returns>Network credentials for the CMP client</returns>
        /// 
        //*********************************************************************

        private System.Net.NetworkCredential GetCmpClientCredsFromConfig()
        {
            var cmpCreds = System.Configuration.ConfigurationManager.AppSettings["CmpServiceCredentials"];

            if (null == cmpCreds)
                throw new Exception("Missing 'CmpServiceCredentials' key in application config");

            var xk = new KryptoLib.X509Krypto("KryptoCert");

            cmpCreds = xk.DecrpytKText(cmpCreds);

            var cmpNamePassword = cmpCreds.Split('\\');

            if (2 != cmpNamePassword.Length)
                throw new Exception("'CmpServiceCredentials' value in application config not in <name>\\<password> format");

            return new System.Net.NetworkCredential(cmpNamePassword[0], cmpNamePassword[1]);
        }

        //*********************************************************************
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

        public IEnumerable<VhdInfo> GetDetachedDisks(int? cmpRequestId)
        {
            if (CmpAccessMode == CmpAccessModeEnum.Monolith)
            {
                var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                return cmp.GetDetachedDisks(cmpRequestId);
            }
            else
            {
                throw new Exception();
            }
        }

        //*********************************************************************
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

        public int DeleteVm(int cmpRequestId, bool deleteFromStorage)
        {
            if (CmpAccessMode == CmpAccessModeEnum.Monolith)
            {
                //addsa -- Impersonate ITSM user (config in CMPWAPExtension.Api)
                string userName = null;//System.Configuration.ConfigurationManager.AppSettings["ItsmUserName"];
                string userDomain = null; //System.Configuration.ConfigurationManager.AppSettings["ItsmUserDomain"];
                string userPassword = null; //System.Configuration.ConfigurationManager.AppSettings["ItsmUserPassword"];

                var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);

                if (null == userName) //assumes config file has no impersonation values, then use default
                {
                    cmp.VmDelete(cmpRequestId, deleteFromStorage, true, true);
                }
                else // impersonate
                {
                    using (var xk = new KryptoLib.X509Krypto())
                    {
                        userPassword = xk.DecrpytKText(userPassword);
                    }

                    using (new Impersonator(userName, userDomain, userPassword))
                    {
                        cmp.VmDelete(cmpRequestId, deleteFromStorage, true, true);
                    }
                }

                //return Translate(fdr);
                return 0;
            }
            else
            {
                var vmDepReq = FetchCmpRequest(cmpRequestId);

                if (null == vmDepReq)
                    return 0;

                var cmpServiceUrl = System.Configuration.ConfigurationManager.AppSettings["CmpServiceUrl"];
                var cmpServ = new CmpService.Container(new Uri(cmpServiceUrl));
                cmpServ.DeleteObject(vmDepReq);
                var dsr = cmpServ.SaveChanges();

                return 1;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Reboots the VM associated with the given CMP request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <returns>0 on success, 1 on exception or failure</returns>
        /// 
        //*********************************************************************

        public int RebootVm(int cmpRequestId)
        {
            // http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/calling-an-odata-service-from-a-net-client

            if (CmpAccessMode == CmpAccessModeEnum.Monolith)
            {
                var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                cmp.VmRestart(cmpRequestId);

                //     cmp.VmGet(cmpRequestId);
                //return Translate(fdr);
                return 0;
            }
            else
            {
                var param = 1;

                var cmpServiceUrl = System.Configuration.ConfigurationManager.AppSettings["CmpServiceUrl"];
                var cmpServ = new CmpService.Container(new Uri(cmpServiceUrl));

                var index = cmpServiceUrl.LastIndexOf("/");
                cmpServiceUrl = cmpServiceUrl.Substring(0, index);

                var actionUri = new Uri(new Uri(cmpServiceUrl), "odata/VmDeployments(" + cmpRequestId + ")/Reboot");
                var averageRating = cmpServ.Execute<double>(
                    actionUri, "POST", true, new BodyOperationParameter("Param", param)).First();
            }

            return 0;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPaSpec"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public CmpInterfaceModel.Models.ServiceProviderAccountSpec 
            InsertServiceProviderAccount(CmpInterfaceModel.Models.ServiceProviderAccountSpec sPaSpec)
        {
            var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
            return cmp.InsertServiceProviderAccount(sPaSpec);
        }

        #region New Methods added
        //*********************************************************************
        ///
        /// <summary>
        /// Reboots the VM associated with the given CMP request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <returns>0 on success, 1 on exception or failure</returns>
        /// 
        //*********************************************************************

        public int StartVm(int cmpRequestId)
        {
            // http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/calling-an-odata-service-from-a-net-client

            if (CmpAccessMode == CmpAccessModeEnum.Monolith)
            {
                var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                cmp.VmStart(cmpRequestId);

                //return Translate(fdr);
                return 0;
            }
            return 0;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Returns info on the VM associated with the CMP given request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <returns>VMinfo object containing VM information</returns>
        /// 
        //*********************************************************************

        public CmpApiClient.VmInfo GetVm(int cmpRequestId, CmpInterfaceModel.Constants.FetchType fetchType)
        {
            if (CmpAccessMode == CmpAccessModeEnum.Monolith)
            {
                var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                var vmRole = cmp.VmGet(cmpRequestId, fetchType);
                var queueStatus = "";
                try
                {
                    var cdb = new CmpDb(CmpDbConnectionString);
                    var vmData = cdb.FetchVMOpRequest(vmRole.RoleName);

                    if (vmData == null)
                    {
                        queueStatus = "NA";      // Set queue status to not available   
                    }
                    else
                    {
                        queueStatus = vmData.StatusCode;
                    }

                    //return Translate(fdr);
                    var vmInfo = new VmInfo
                    {
                        DataVirtualHardDisks = vmRole.DataVirtualHardDisks,
                        DeploymentID = vmRole.DeploymentID,
                        DNSName = vmRole.DNSName,
                        InternalIP = vmRole.InternalIP,
                        OSVirtualHardDisk = vmRole.OSVirtualHardDisk,
                        RDPCertificateThumbprint = vmRole.RDPCertificateThumbprint,
                        RoleName = vmRole.RoleName,
                        RoleSize = vmRole.RoleSize,
                        Status = vmRole.Status,
                        Subscription = vmRole.Subscription,
                        QueueStatus = queueStatus,
                        OSVersion = vmRole.OSVersion, 
                        MediaLocation = vmRole.MediaLocation
                    };
                    return vmInfo;
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception in CmpApiClient.GetVm() : "
                                        + CmpServiceLib.Utilities.UnwindExceptionMessages(ex));
                }

            }
            return null;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Stops the VM associated with the given CMP request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <returns>0 on success, 1 on exception or failure</returns>
        /// 
        //*********************************************************************

        public int StopVm(int cmpRequestId)
        {
            // http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/calling-an-odata-service-from-a-net-client

            if (CmpAccessMode == CmpAccessModeEnum.Monolith)
            {
                var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                cmp.VmStop(cmpRequestId);

                return 0;
            }
            return 0;
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Deallocates the VM associated with the given CMP request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <returns>0 on success, 1 on exception or failure</returns>
        /// 
        //*********************************************************************

        public int DeallocateVm(int cmpRequestId)
        {
            // http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/calling-an-odata-service-from-a-net-client

            if (CmpAccessMode == CmpAccessModeEnum.Monolith)
            {
                var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                cmp.VmDeallocate(cmpRequestId);

                //return Translate(fdr);
                return 0;
            }
            return 0;
        }

        //*********************************************************************
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

        public int AddDisk(int cmpRequestId, List<VhdInfo> disks)
        {
            if (CmpAccessMode == CmpAccessModeEnum.Monolith)
            {
                var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                cmp.VmAddDisk(cmpRequestId, disks);

                //return Translate(fdr);
                return 0;
            }
            return 0;
        }

        //*********************************************************************
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

        public void DetachDisk(int? cmpRequestId, VhdInfo disk, bool deleteFromStorage)
        {
            if (CmpAccessMode == CmpAccessModeEnum.Monolith)
            {
                var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                cmp.DetachDisk(cmpRequestId, disk, deleteFromStorage);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Attaches an available disk to the VM associated with the given 
        /// CMP request ID
        /// <param name="cmpRequestId">The ID of the CMP request associated 
        /// with the VM</param>
        /// <param name="disk">Disk to add to the VM</param>
        /// 
        //*********************************************************************

        public void AttachExistingDisk(int? cmpRequestId, VhdInfo disk)
        {
            if (CmpAccessMode == CmpAccessModeEnum.Monolith)
            {
                var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                cmp.AttachExistingDisk(cmpRequestId, disk);
            }
        }

        //*********************************************************************
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
        public int GrowDisk(int cmpRequestId, List<VhdInfo> disks)
        {
            //throw new NotImplementedException();

            if (CmpAccessMode == CmpAccessModeEnum.Monolith)
            {
                var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                cmp.VmGrowDisk(cmpRequestId, disks);

                //return Translate(fdr);
                return 0;
            }
            return 0;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Changes the SKU size of the VM associated with the given CMP 
        /// request ID
        /// </summary>
        /// <param name="cmpRequestId">The ID of the CMP request associated
        /// with the VM</param>
        /// <param name="disks">SKU size to change the VM size to</param>
        /// <returns>0 on success, 1 on exception or failure</returns>
        /// 
        //*********************************************************************
        public int Resize(int cmpRequestId, string size)
        {
            var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
            cmp.VmResize(cmpRequestId, size);

            //return Translate(fdr);
            return 0;

        }

        //*********************************************************************
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
        public int FetchDiskCount(int cmpRequestId, out string roleSizeName)
        {
            var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
            return cmp.FetchDiskCount(cmpRequestId, out roleSizeName);
        }
        #endregion New Methods added

        //*********************************************************************
        ///
        /// <summary>
        /// Submits the given operation to the CMP queue
        /// </summary>
        /// <param name="opSpec">The operation to be submitted to the CMP queue
        /// </param>
        /// <returns>The operation submitted to the CMP queue</returns>
        /// 
        //*********************************************************************

        public CmpInterfaceModel.Models.OpSpec SubmitOpToQueue(CmpInterfaceModel.Models.OpSpec opSpec)
        {
            var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
            return cmp.SubmitOpToQueue(opSpec);
        }

        /// <summary>
        /// Return Operation data performed on a VM via polling
        /// </summary>
        /// <returns></returns>
        public CmpInterfaceModel.Models.OpSpec GetVmOpsRequestSpec(string vmName)
        {
            try
            {
                var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);

                var vmData = cmp.FetchVmOpRequest(vmName);

                var vmOpData = new OpSpec()
                {
                    Name = vmData.TargetName,
                    Id = vmData.Id,
                    Opcode = vmData.RequestType,
                    StatusCode = vmData.StatusCode,
                    StatusMessage = vmData.StatusMessage,
                    TargetName = vmData.TargetName,
                    TargetType = vmData.TargetTypeCode,
                    Requestor = vmData.WhoRequested
                };

                if (vmData.StatusCode == Constants.StatusEnum.Exception.ToString())
                {
                    vmOpData.StatusMessage = vmData.ExceptionMessage;
                }

                return vmOpData;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpClient.GetVmOpsRequestSpec() : " +
                    Utils.UnwindExceptionMessages(ex));
            }
        }
      
        //*********************************************************************
        ///
        /// <summary>
        /// Class for a Service Provider Account
        /// </summary>
        /// 
        //*********************************************************************
        /* todo: move this class into a separate file */
        public class ServiceProviderAccount
        {
            public ServiceProviderAccount()
            {
                
            }
            /// <summary>
            /// Initializes a new Service Provider Account
            /// </summary>
            /// <param name="spa">The service account to initialize</param>
            public ServiceProviderAccount(CmpServiceLib.Models.ServiceProviderAccount spa)
            {
                if (null == spa.Active)
                    spa.Active = false;

                if (null == spa.CoreCountCurrent)
                    spa.CoreCountCurrent = 0;

                if (null == spa.CoreCountMax)
                    spa.CoreCountMax = 0;

                if (null == spa.ExpirationDate)
                    spa.ExpirationDate = DateTime.Now.AddYears(1);

                ID = spa.ID;
                AccountID = spa.AccountID;
                AccountPassword = spa.AccountPassword;
                AccountType = spa.AccountType;
                Active = (bool)spa.Active;
                AzAffinityGroup = spa.AzAffinityGroup;
                AzRegion = spa.AzRegion;
                AzStorageContainerUrl = spa.AzStorageContainerUrl;
                AzSubnet = spa.AzSubnet;
                AzVNet = spa.AzVNet;
                CertificateBlob = spa.CertificateBlob;
                CertificateThumbprint = spa.CertificateThumbprint;
                Config = spa.Config;
                CoreCountCurrent = (int)spa.CoreCountCurrent;
                CoreCountMax = (int)spa.CoreCountMax;
                Description = spa.Description;
                ExpirationDate = (DateTime)spa.ExpirationDate;
                Name = spa.Name;
                OwnerNamesCSV = spa.OwnerNamesCSV;
                ResourceGroup = spa.ResourceGroup;
                TagData = spa.TagData;
            }

            /// <summary>
            /// Service Account ID
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// Name of the Service Account
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Description of the Service Account
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// CSV containing the Service Account owners
            /// </summary>
            public string OwnerNamesCSV { get; set; }

            /// <summary>
            /// XML configuration for the Service Account
            /// </summary>
            public string Config { get; set; }

            /// <summary>
            /// Tag data for the Service Account
            /// </summary>
            public string TagData { get; set; }

            /// <summary>
            /// ID for the Tag data of the Service Account
            /// </summary>
            public int TagID { get; set; }

            /// <summary>
            /// Type of the Service Account
            /// </summary>
            public string AccountType { get; set; }

            /// <summary>
            /// Expiration date of the Service Account
            /// </summary>
            public System.DateTime ExpirationDate { get; set; }

            /// <summary>
            /// Certificate blob for the Service Account
            /// </summary>
            public string CertificateBlob { get; set; }

            /// <summary>
            /// Thumbprint for the certificate blob of the Service Account
            /// </summary>
            public string CertificateThumbprint { get; set; }

            /// <summary>
            /// ID for the Service Account
            /// </summary>
            public string AccountID { get; set; }

            /// <summary>
            /// Password for the Service Account
            /// </summary>
            public string AccountPassword { get; set; }

            /// <summary>
            /// Flag on whether the Service Account is active
            /// </summary>
            public bool Active { get; set; }

            /// <summary>
            /// Azure region of the Service Account
            /// </summary>
            public string AzRegion { get; set; }

            /// <summary>
            /// Azure affinity group of the Service Account
            /// </summary>
            public string AzAffinityGroup { get; set; }

            /// <summary>
            /// Azure virtual network of the Service Account
            /// </summary>
            public string AzVNet { get; set; }

            /// <summary>
            /// Azure subnet of the Service Account
            /// </summary>
            public string AzSubnet { get; set; }
            /// <summary>
            /// Url of the Azure storage container for the Service Account
            /// </summary>
            public string AzStorageContainerUrl { get; set; }

            /// <summary>
            /// Resource Group of the Service Account
            /// </summary>
            public string ResourceGroup { get; set; }

            /// <summary>
            /// Maximum core count for the Service Account
            /// </summary>
            public int CoreCountMax { get; set; }

            /// <summary>
            /// Current core count for the Service Account
            /// </summary>
            public int CoreCountCurrent { get; set; }

        }

        //*********************************************************************
        ///
        /// <summary>
        /// Contains information about a virtual machine
        /// </summary>
        /// 
        //*********************************************************************

        /* todo: move this class into a separate file */
        /// 
        public class VmInfo
        {
            /// <summary>
            /// List of virtual hard disks on the VM
            /// </summary>
            public IList<DataVirtualHardDisk> DataVirtualHardDisks { get; set; }

            /// <summary>
            /// Remote Desktop Protocol certificate thumbprint for the VM
            /// </summary>
            public string RDPCertificateThumbprint { get; set; }
            
            /// <summary>
            /// Internal IP for the VM
            /// </summary>
            public string InternalIP { get; set; }
            
            /// <summary>
            /// Status of the VM
            /// </summary>
            public string Status { get; set; }
            
            /// <summary>
            /// Uri specifying the media location of the VM
            /// </summary>
            public Uri MediaLocation { get; set; }
            
            /// <summary>
            /// OS version running on the VM
            /// </summary>
            public string OSVersion { get; set; }
            
            /// <summary>
            /// Virtual hard disk containing the OS on the VM
            ///</summary>
            public OsVirtualHardDisk OSVirtualHardDisk { get; set; }

            /// <summary>
            /// Role name of the VM
            /// </summary>
            public string RoleName { get; set; }

            /// <summary>
            /// Role size of the VM
            /// </summary>
            public string RoleSize { get; set; }

            /// <summary>
            /// DNS name for the VM
            /// </summary>
            public string DNSName { get; set; }

            /// <summary>
            /// Deployment ID for the VM
            /// </summary>
            public string DeploymentID { get; set; }

            /// <summary>
            /// Azure subscription details for the VM
            /// </summary>
            public SubscriptionInfo Subscription { get; set; }

            /// <summary>
            /// Status of the VM in the CMP operations queue
            /// </summary>
            public string QueueStatus { get; set; }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Returns a list of the Service provider accounts for the given group 
        /// </summary>
        /// <param name="groupName">Name of the group to find service provider
        /// accounts for</param>
        /// <returns>A list of service provider accounts for the given group</returns>
        /// 
        //*********************************************************************

        public List<ServiceProviderAccount> FetchServProvAcctList(string groupName)
        {
            try
            {
                if (CmpAccessMode == CmpAccessModeEnum.Monolith)
                {
                    var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                    var spaListOut = new List<ServiceProviderAccount>();

                    var spaList = cmp.FetchServiceProviderAccounts(groupName);

                    if (null != spaList)
                        foreach (var spa in spaList)
                            spaListOut.Add(new ServiceProviderAccount(spa));

                    return spaListOut;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpClient.FetchServProvAcctList() : " +
                    Utils.UnwindExceptionMessages(ex));
            }
        }

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

        public List<ServiceProviderAccount> FetchServProvAcctList(IEnumerable<int> idsToSearch)
        {
            try
            {
                if (CmpAccessMode == CmpAccessModeEnum.Monolith)
                {
                    var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                    var spaListOut = new List<ServiceProviderAccount>();

                    var spaList = cmp.FetchServiceProviderAccounts(idsToSearch);

                    if (null != spaList)
                        foreach (var spa in spaList)
                            spaListOut.Add(new ServiceProviderAccount(spa));

                    return spaListOut;
                }
            return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpClient.FetchServProvAcctList() : " +
                    Utils.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Returns a list of all the active Service Provider Accounts
        /// </summary>
        /// <returns>A list of service provider accounts
        /// </returns>
        /// 
        //*********************************************************************

        public List<ServiceProviderAccount> FetchServProvAcctList()
        {
            try
            {
                if (CmpAccessMode == CmpAccessModeEnum.Monolith)
                {
                    var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                    var spaListOut = new List<ServiceProviderAccount>();

                    var spaList = cmp.FetchServiceProviderAccounts();

                    if (null != spaList)
                        foreach (var spa in spaList)
                            spaListOut.Add(new ServiceProviderAccount(spa));

                    return spaListOut;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpClient.FetchServProvAcctList() : " +
                    Utils.UnwindExceptionMessages(ex));
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

        public ServiceProviderAccount InsertServiceProviderAccount(Models.ServiceProviderAccount sPa)
        {
            try
            {
                if (CmpAccessMode == CmpAccessModeEnum.Monolith)
                {
                    var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                    var spaOut = cmp.InsertServiceProviderAccount(sPa.GetModel());
                    return new ServiceProviderAccount(spaOut);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpClient.InsertServiceProviderAccount() : " +
                    Utils.UnwindExceptionMessages(ex));
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

        public ServiceProviderAccount UpdateServiceProviderAccount(Models.ServiceProviderAccount sPa)
        {
            try
            {
                if (CmpAccessMode == CmpAccessModeEnum.Monolith)
                {
                    var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                    var spaOut = cmp.UpdateServiceProviderAccount(sPa.GetModel());
                    return new ServiceProviderAccount(spaOut);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpClient.UpdateServiceProviderAccount() : " +
                    Utils.UnwindExceptionMessages(ex));
            }
        }


        //*********************************************************************
        ///
        /// <summary>
        /// Fetches CMP requests from the CMP database
        /// </summary>
        /// <returns>List of the CMP requests fetched from the CMP database
        /// </returns>
        /// 
        //*********************************************************************

        public List<CmpService.VmDeploymentRequest> FetchCmpRequests()
        {
            try
            {
                if (CmpAccessMode == CmpAccessModeEnum.Monolith)
                {
                    var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                    var fdrList = cmp.FetchVmDepRequests();

                    var outList = new List<CmpService.VmDeploymentRequest>(fdrList.Count);
                    outList.AddRange(fdrList.Select(Translate));
                    return outList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpClient.FetchCmpRequests() : " + Utils.UnwindExceptionMessages(ex));
            }
        }

        public List<CmpServiceLib.Models.Container> FetchAzureResourceGroups()
        {
            try
            {
                if (CmpAccessMode == CmpAccessModeEnum.Monolith)
                {
                    var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                    var fdrList = cmp.FetchAzureResourceGroups();

                    //var outList = new List<CmpService.VmDeploymentRequest>(fdrList.Count);
                    //outList.AddRange(fdrList.Select(Translate));
                    return fdrList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpClient.FetchCmpRequests() : " + Utils.UnwindExceptionMessages(ex));
            }
        }


        //*********************************************************************
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

        public CmpService.VmDeploymentRequest FetchCmpRequest(int cmpRequestId)
        {
            try
            {
                if (CmpAccessMode == CmpAccessModeEnum.Monolith)
                {
                    var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);
                    var fdr = cmp.FetchVmDepRequest(cmpRequestId);
                    return Translate(fdr);
                }
                else
                {
                    CmpService.VmDeploymentRequest vmRedReq = null;
                    var cmpServiceUrl = System.Configuration.ConfigurationManager.AppSettings["CmpServiceUrl"];

                    var index = cmpServiceUrl.LastIndexOf("/");
                    cmpServiceUrl = cmpServiceUrl.Substring(0, index);

                    var client = new HttpClient { BaseAddress = new Uri(cmpServiceUrl) };

                    // Add an Accept header for JSON format.
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUrl = "odata/VmDeployments(" + cmpRequestId.ToString() + ")";

                    var response = client.GetAsync(requestUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var body = response.Content.ReadAsStringAsync().Result;
                        vmRedReq = Newtonsoft.Json.JsonConvert.DeserializeObject<CmpService.VmDeploymentRequest>(body);

                        //*** TODO: Mark West : Maybe the CMP should set the status message instead
                        if (vmRedReq.Status.Equals(CmpInterfaceModel.Constants.StatusEnum.Exception.ToString()))
                            vmRedReq.StatusMessage = vmRedReq.ExceptionMessage;
                    }
                    else
                    {
                        throw new Exception(string.Format("URL: '{0}' Code: {1}, Message: {2}",
                            cmpServiceUrl + requestUrl, (int)response.StatusCode, response.ReasonPhrase));
                    }

                    return vmRedReq;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpClient.FetchCmpRequest() : " + Utils.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Maps a CmpService.VMDeploymentRequest object to a 
        /// CmpServiceLib.Models.VmDeploymentRequest object
        /// </summary>
        /// <param name="vmDepReq">The CmpService.VMDeploymentRequest 
        /// object to map to a CmpServiceLib.Models.VMDeploymentRequest object
        /// </param>
        /// <returns>The CmpServiceLib.Models.VmDeploymentRequest object mapped
        /// from the CmpService.VmDeploymentRequest object</returns>
        /// 
        //*********************************************************************
        /* to do: move into a separate mapper class */
        private CmpServiceLib.Models.VmDeploymentRequest Translate(
            CmpService.VmDeploymentRequest vmDepReq)
        {
            var vmDepReqOut = new CmpServiceLib.Models.VmDeploymentRequest
            {
                VmSize = vmDepReq.VmSize,
                Active = vmDepReq.Active,
                AftsID = vmDepReq.AftsID,
                Config = vmDepReq.Config,
                ExceptionMessage = vmDepReq.ExceptionMessage,
                ID = vmDepReq.ID,
                LastStatusUpdate = vmDepReq.WhenRequested,
                ParentAppID = vmDepReq.ParentAppID,
                ParentAppName = vmDepReq.ParentAppName,
                RequestDescription = vmDepReq.RequestDescription,
                RequestName = vmDepReq.RequestName,
                RequestType = vmDepReq.RequestType,
                SourceServerName = vmDepReq.SourceServerName,
                SourceVhdFilesCSV = vmDepReq.SourceVhdFilesCSV,
                StatusCode = vmDepReq.Status,
                StatusMessage = vmDepReq.StatusMessage,
                TagData = vmDepReq.TagData,
                TagID = vmDepReq.TagID,
                TargetAccount = vmDepReq.TargetAccount,
                TargetAccountCreds = vmDepReq.TargetAccountCreds,
                TargetAccountType = vmDepReq.TargetAccountType,
                TargetLocation = vmDepReq.TargetLocation,
                TargetLocationType = vmDepReq.TargetLocationType,
                TargetServicename = vmDepReq.TargetServiceName,
                TargetServiceProviderType = vmDepReq.TargetServiceProviderType,
                TargetVmName = vmDepReq.TargetVmName,
                WhenRequested = vmDepReq.WhenRequested,
                WhoRequested = vmDepReq.WhoRequested
                //ServiceProviderAccountID = 0,
                //ServiceProviderResourceGroup = "",
                //SourceServerRegion = ""
            };

            return vmDepReqOut;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Maps a CmpServiceLib.Models.VmDeploymentRequest object to a
        /// CmpService.VMDeploymentRequest object
        /// </summary>
        /// <param name="vmDepReq">The CmpServiceLib.Models.VMDeploymentRequest 
        /// object to map to a CmpService.VmDeploymentRequest object
        /// </param>
        /// <returns>The CmpService.VmDeploymentRequest object mapped
        /// from the CmpServiceLib.Models.VmDeploymentRequest object</returns>
        /// 
        //*********************************************************************
        /* to do: move into a separate mapper class */
        private CmpService.VmDeploymentRequest Translate(
            CmpServiceLib.Models.VmDeploymentRequest vmDepReq)
        {
            var vmDepReqOut = new CmpService.VmDeploymentRequest
            {
                VmSize = vmDepReq.VmSize,
                Active = Convert.ToBoolean(vmDepReq.Active),
                AftsID = Convert.ToInt16(vmDepReq.AftsID),
                Config = vmDepReq.Config,
                ExceptionMessage = vmDepReq.ExceptionMessage,
                ID = vmDepReq.ID,
                ParentAppID = vmDepReq.ParentAppID,
                ParentAppName = vmDepReq.ParentAppName,
                RequestDescription = vmDepReq.RequestDescription,
                RequestName = vmDepReq.RequestName,
                RequestType = vmDepReq.RequestType,
                SourceServerName = vmDepReq.SourceServerName,
                SourceVhdFilesCSV = vmDepReq.SourceVhdFilesCSV,
                Status = vmDepReq.StatusCode,
                StatusMessage = vmDepReq.StatusMessage,
                TagData = vmDepReq.TagData,
                TagID = Convert.ToInt32(vmDepReq.TagID),
                TargetAccount = vmDepReq.TargetAccount,
                TargetAccountCreds = vmDepReq.TargetAccountCreds,
                TargetAccountType = vmDepReq.TargetAccountType,
                TargetLocation = vmDepReq.TargetLocation,
                TargetLocationType = vmDepReq.TargetLocationType,
                TargetServiceName = vmDepReq.TargetServicename,
                TargetServiceProviderType = vmDepReq.TargetServiceProviderType,
                TargetVmName = vmDepReq.TargetVmName,
                WhoRequested = vmDepReq.WhoRequested
            };

            if (null != vmDepReq.LastStatusUpdate)
                vmDepReqOut.LastStatusUpdate = Convert.ToDateTime(vmDepReq.LastStatusUpdate);
            if (null != vmDepReq.WhenRequested)
                vmDepReqOut.WhenRequested = Convert.ToDateTime(vmDepReq.WhenRequested);

            return vmDepReqOut;
        }

        //*********************************************************************
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
        public CmpService.VmDeploymentRequest SubmitToCmp(CmpVmRequest cmpVmRequest)
        {
            try
            {

                var vmDepReq =
                    BuildCmpDeploymentRequest(cmpVmRequest);

                if (CmpAccessMode == CmpAccessModeEnum.Monolith)
                {
                    var NewRequest = true;
                    CmpServiceLib.Models.VmDeploymentRequest response = null;

                    var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);

                    if (NewRequest)
                        response = cmp.InsertVmDepRequest(Translate(vmDepReq));
                    //else
                    //    cmp.UpdateVmDepRequest(vmDepReq, null);

                    return Translate(response);
                }
                else
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback =
                        delegate(object s, X509Certificate certificate, X509Chain chain,
                            System.Net.Security.SslPolicyErrors sslPolicyErrors)
                        { return true; };

                    var cmpServiceUrl = System.Configuration.ConfigurationManager.AppSettings["CmpServiceUrl"];

                    var cmpServ = new CmpService.Container(new Uri(cmpServiceUrl));
                    cmpServ.SendingRequest2 += FS_SendingRequest2;
                    cmpServ.WritingEntity += FS_WritingEntity;

                    //cmpServ.ClientCredentials = CmpClientCredentials;

                    cmpServ.AddObject("VmDeployments", vmDepReq);
                    var dsrList = cmpServ.SaveChanges();
                    CmpService.VmDeploymentRequest vdr = null;

                    //foreach (System.Data.Services.Client.ChangeOperationResponse cs in dsrList)
                    foreach (var dsr in dsrList)
                    {
                        var cs = dsr as System.Data.Services.Client.ChangeOperationResponse;

                        if (null == cs)
                            throw new Exception("NULL ChangeOperationResponse CMP Service response");

                        var ed = cs.Descriptor as System.Data.Services.Client.EntityDescriptor;

                        if (null == ed)
                            throw new Exception("NULL EntityDescriptor in CMP Service response");

                        vdr = ed.Entity as CmpService.VmDeploymentRequest;
                        break;
                    }

                    if (null == vdr)
                        throw new Exception("No ChangeOperationResponse in CMP Service response");

                    //*** TODO: Mark West : Maybe the CMP should set the status message instead
                    if (vdr.Status.Equals(CmpInterfaceModel.Constants.StatusEnum.Exception.ToString()))
                        vdr.StatusMessage = vdr.ExceptionMessage;

                    return vdr;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpWapExtension.CmpClient.SubmitToCmp(): " +
                    Utils.UnwindExceptionMessages(ex));
            }
        }

        public CmpService.VmDeploymentRequest SubmitToCmpStaticTemplate(CmpVmRequest cmpVmRequest, string vmConfig)
        {
            try
            {

                var vmDepReq =                  
                    BuildCmpDeploymentRequestStaticTemplate(cmpVmRequest, vmConfig);

                if (CmpAccessMode == CmpAccessModeEnum.Monolith)
                {
                    var NewRequest = true;
                    CmpServiceLib.Models.VmDeploymentRequest response = null;

                    var cmp = new CmpServiceLib.CmpService(_eventLog, CmpDbConnectionString, null);

                    if (NewRequest)
                        response = cmp.InsertVmDepRequest(Translate(vmDepReq));

                    return Translate(response);
                }
                else
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback =
                        delegate(object s, X509Certificate certificate, X509Chain chain,
                            System.Net.Security.SslPolicyErrors sslPolicyErrors)
                        { return true; };

                    var cmpServiceUrl = System.Configuration.ConfigurationManager.AppSettings["CmpServiceUrl"];

                    var cmpServ = new CmpService.Container(new Uri(cmpServiceUrl));
                    cmpServ.SendingRequest2 += FS_SendingRequest2;
                    cmpServ.WritingEntity += FS_WritingEntity;

                    //cmpServ.ClientCredentials = CmpClientCredentials;

                    cmpServ.AddObject("VmDeployments", vmDepReq);
                    var dsrList = cmpServ.SaveChanges();
                    CmpService.VmDeploymentRequest vdr = null;

                    //foreach (System.Data.Services.Client.ChangeOperationResponse cs in dsrList)
                    foreach (var dsr in dsrList)
                    {
                        var cs = dsr as System.Data.Services.Client.ChangeOperationResponse;

                        if (null == cs)
                            throw new Exception("NULL ChangeOperationResponse CMP Service response");

                        var ed = cs.Descriptor as System.Data.Services.Client.EntityDescriptor;

                        if (null == ed)
                            throw new Exception("NULL EntityDescriptor in CMP Service response");

                        vdr = ed.Entity as CmpService.VmDeploymentRequest;
                        break;
                    }

                    if (null == vdr)
                        throw new Exception("No ChangeOperationResponse in CMP Service response");

                    //*** TODO: Mark West : Maybe the CMP should set the status message instead
                    if (vdr.Status.Equals(CmpInterfaceModel.Constants.StatusEnum.Exception.ToString()))
                        vdr.StatusMessage = vdr.ExceptionMessage;

                    return vdr;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpWapExtension.CmpClient.SubmitToCmp(): " +
                    Utils.UnwindExceptionMessages(ex));
            }
        }


        //*********************************************************************
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
        public CmpService.VmDeploymentRequest SubmitToCmp_Old(CmpVmRequest cmpVmRequest)
        {
            try
            {
                var vmDepReq =
                    BuildCmpDeploymentRequest(cmpVmRequest);

                //***********************

                var rdr = new CmpServiceLib.Models.VmDeploymentRequest
                {
                    VmSize = vmDepReq.VmSize,
                    Active = vmDepReq.Active,
                    AftsID = vmDepReq.AftsID,
                    Config = vmDepReq.Config,
                    ExceptionMessage = vmDepReq.ExceptionMessage,
                    ID = vmDepReq.ID,
                    LastStatusUpdate = vmDepReq.WhenRequested,
                    ParentAppID = vmDepReq.ParentAppID,
                    ParentAppName = vmDepReq.ParentAppName,
                    RequestDescription = vmDepReq.RequestDescription,
                    RequestName = vmDepReq.RequestName,
                    RequestType = vmDepReq.RequestType,
                    SourceServerName = vmDepReq.SourceServerName,
                    SourceVhdFilesCSV = vmDepReq.SourceVhdFilesCSV,
                    StatusCode = vmDepReq.Status,
                    StatusMessage = vmDepReq.StatusMessage,
                    TagData = vmDepReq.TagData,
                    TagID = vmDepReq.TagID,
                    TargetAccount = vmDepReq.TargetAccount,
                    TargetAccountCreds = vmDepReq.TargetAccountCreds,
                    TargetAccountType = vmDepReq.TargetAccountType,
                    TargetLocation = vmDepReq.TargetLocation,
                    TargetLocationType = vmDepReq.TargetLocationType,
                    TargetServicename = vmDepReq.TargetServiceName,
                    TargetServiceProviderType = vmDepReq.TargetServiceProviderType,
                    TargetVmName = vmDepReq.TargetVmName,
                    WhenRequested = vmDepReq.WhenRequested,
                    WhoRequested = vmDepReq.WhoRequested
                    //ServiceProviderAccountID = 0,
                    //ServiceProviderResourceGroup = "",
                    //SourceServerRegion = ""
                };

                var cdc = new CmpDbClient(null);
                var fdr = cdc.InsertVmDepRequest(rdr);

                var odr = new CmpService.VmDeploymentRequest
                {
                    VmSize = fdr.VmSize,
                    Active = Convert.ToBoolean(fdr.Active),
                    AftsID = Convert.ToInt16(fdr.AftsID),
                    Config = fdr.Config,
                    ExceptionMessage = fdr.ExceptionMessage,
                    ID = fdr.ID,
                    ParentAppID = fdr.ParentAppID,
                    ParentAppName = fdr.ParentAppName,
                    RequestDescription = fdr.RequestDescription,
                    RequestName = fdr.RequestName,
                    RequestType = fdr.RequestType,
                    SourceServerName = fdr.SourceServerName,
                    SourceVhdFilesCSV = fdr.SourceVhdFilesCSV,
                    Status = fdr.StatusCode,
                    StatusMessage = fdr.StatusMessage,
                    TagData = fdr.TagData,
                    TagID = Convert.ToInt32(fdr.TagID),
                    TargetAccount = fdr.TargetAccount,
                    TargetAccountCreds = fdr.TargetAccountCreds,
                    TargetAccountType = fdr.TargetAccountType,
                    TargetLocation = fdr.TargetLocation,
                    TargetLocationType = fdr.TargetLocationType,
                    TargetServiceName = fdr.TargetServicename,
                    TargetServiceProviderType = fdr.TargetServiceProviderType,
                    TargetVmName = fdr.TargetVmName,
                    WhoRequested = fdr.WhoRequested
                };

                if (null != fdr.LastStatusUpdate)
                    odr.LastStatusUpdate = Convert.ToDateTime(fdr.LastStatusUpdate);
                if (null != fdr.WhenRequested)
                    odr.WhenRequested = Convert.ToDateTime(fdr.WhenRequested);

                return odr;

                //***********************

                /*System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    delegate(object s, X509Certificate certificate, X509Chain chain,
                    System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };

                string cmpServiceUrl = System.Configuration.ConfigurationManager.AppSettings["CmpServiceUrl"];

                var cmpServ = new CmpService.Container(new Uri(cmpServiceUrl));
                cmpServ.SendingRequest2 += FS_SendingRequest2;
                cmpServ.WritingEntity += FS_WritingEntity;

                cmpServ.ClientCredentials = CmpClientCredentials;

                cmpServ.AddObject("VmDeployments", vmDepReq);
                System.Data.Services.Client.DataServiceResponse dsrList = cmpServ.SaveChanges();
                CmpService.VmDeploymentRequest vdr = null;

                //foreach (System.Data.Services.Client.ChangeOperationResponse cs in dsrList)
                foreach (var dsr in dsrList)
                {
                    var cs = dsr as System.Data.Services.Client.ChangeOperationResponse;

                    if (null == cs)
                        throw new Exception("NULL ChangeOperationResponse CMP Service response");

                    var ed = cs.Descriptor as System.Data.Services.Client.EntityDescriptor;

                    if (null == ed)
                        throw new Exception("NULL EntityDescriptor in CMP Service response");

                    vdr = ed.Entity as CmpService.VmDeploymentRequest;
                    break;
                }

                if (null == vdr)
                    throw new Exception("No ChangeOperationResponse in CMP Service response");

                //*** TODO: Mark West : Maybe the CMP should set the status message instead
                if (vdr.Status.Equals(CmpInterfaceModel.Constants.StatusEnum.Exception.ToString()))
                    vdr.StatusMessage = vdr.ExceptionMessage;

                return vdr;*/
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpWapExtension.CmpClient.SubmitToCmp(): " +
                    Utils.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        //*********************************************************************

        void CmpServ_SendingRequest(object sender, System.Data.Services.Client.SendingRequestEventArgs e)
        {
            //throw new NotImplementedException();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Maps a CmpVMRequest to a CmpService.VMDeploymentRequest
        /// </summary>
        /// <param name="cmpVmReq">The CmpVmRequest to map to a 
        /// CmpService.VmDeploymentRequest</param>
        /// <returns>THe CmpService.VmDeploymentRequest mapped from the given
        /// CmpVmRequest</returns>
        /// 
        //*********************************************************************

        CmpService.VmDeploymentRequest BuildCmpDeploymentRequest(CmpVmRequest cmpVmReq)
        {
            try
            {
                var cdr = new CmpService.VmDeploymentRequest
                {
                    ID = 0,
                    Active = true,
                    ParentAppID = cmpVmReq.RequestRecord.AppID,
                    ParentAppName = cmpVmReq.RequestRecord.AppName,
                    RequestName = "Wap:" + cmpVmReq.RequestRecord.RequestName,
                    RequestDescription = "Wap request (" + cmpVmReq.RequestRecord.RequestID.ToString() +
                                         ") for Azure VM : '" + cmpVmReq.RequestRecord.RequestName,
                    RequestType = CmpInterfaceModel.Constants.RequestTypeEnum.NewVM.ToString(),
                    Status = CmpInterfaceModel.Constants.StatusEnum.Submitted.ToString(),
                    TagData =
                        "<WapReq>" + CmpInterfaceModel.Utilities.Serialize(typeof(CmpVmRequest), cmpVmReq) +
                        "</WapReq>",
                    TagID = cmpVmReq.RequestRecord.RequestID,
                    TargetLocation = cmpVmReq.RequestRecord.AzureRegionName,
                    TargetLocationType = CmpInterfaceModel.Constants.TargetLocationTypeEnum.Region.ToString(),
                    TargetServiceProviderType =
                        CmpInterfaceModel.Constants.TargetServiceProviderTypeEnum.Azure.ToString(),
                    TargetVmName = cmpVmReq.RequestRecord.RequestName,
                    WhoRequested = cmpVmReq.RequestRecord.CreatedBy,
                    WhenRequested = DateTime.UtcNow,
                    TargetServiceName = GetHostServiceName(cmpVmReq, true)
                };

                cdr.Config = BuildCmpConfig(cdr, cmpVmReq);

                return cdr;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in BuildCmpDeploymentRequest() : " + Utils.UnwindExceptionMessages(ex));
            }
        }
        CmpService.VmDeploymentRequest BuildCmpDeploymentRequestStaticTemplate(CmpVmRequest cmpVmReq, string vmConfig)
        {
            try
            {
                var cdr = new CmpService.VmDeploymentRequest
                {
                    ID = 0,
                    Active = true,
                    ParentAppID = cmpVmReq.RequestRecord.AppID,
                    ParentAppName = cmpVmReq.RequestRecord.AppName,
                    RequestName = "Wap:" + cmpVmReq.RequestRecord.RequestName,
                    RequestDescription = "Wap request (" + cmpVmReq.RequestRecord.RequestID.ToString() +
                                         ") for Azure VM : '" + cmpVmReq.RequestRecord.RequestName,
                    RequestType = CmpInterfaceModel.Constants.RequestTypeEnum.NewVM.ToString(),
                    Status = CmpInterfaceModel.Constants.StatusEnum.Submitted.ToString(),
                    TagData =
                        "<WapReq>" + CmpInterfaceModel.Utilities.Serialize(typeof(CmpVmRequest), cmpVmReq) +
                        "</WapReq>",
                    TagID = cmpVmReq.RequestRecord.RequestID,
                    TargetLocation = cmpVmReq.RequestRecord.AzureRegionName,
                    TargetLocationType = CmpInterfaceModel.Constants.TargetLocationTypeEnum.Region.ToString(),
                    TargetServiceProviderType =
                        CmpInterfaceModel.Constants.TargetServiceProviderTypeEnum.Azure.ToString(),
                    TargetVmName = cmpVmReq.RequestRecord.RequestName,
                    WhoRequested = cmpVmReq.RequestRecord.CreatedBy,
                    WhenRequested = DateTime.UtcNow,
                    TargetServiceName = null
                };

                //cdr.Config = BuildCmpConfig(cdr, cmpVmReq);
                cdr.Config = vmConfig;
                return cdr;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in BuildCmpDeploymentRequest() : " + Utils.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Returns the Account ID of the default target service provider
        /// specified in the config
        /// </summary>
        /// <returns>the Account ID of the default target service provider
        /// specified in the config</returns>
        /// 
        //*********************************************************************

        int GetDefaultTargetServiceProviderAccountID()
        {
            var spid = System.Configuration.ConfigurationManager.AppSettings["DefaultTargetServiceProviderAccountID"];

            if (null == spid)
                return 0;

            try
            {
                return Convert.ToInt32(spid);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        //*********************************************************************
        /// <summary>
        /// Returns the default target service provider account group specified
        /// in the config
        /// </summary>
        /// <returns>The default target service provider account group specified
        /// in the config</returns>
        //*********************************************************************
  
        string GetDefaultTargetServiceProviderAccountGroup()
        {
            var ag = System.Configuration.ConfigurationManager.AppSettings["DefaultTargetServiceProviderAccountGroup"];

            if (null == ag)
                return "Default";

            return ag;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Searches the given Azure storage container for the available data
        /// disks specified in the diskSpec (XML string) belonging to the 
        /// given VM
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        List<CmpInterfaceModel.Models.DataVirtualHardDisk> GetDataDiskList(
            string storageContainer, string vmName, string diskSpec)
        {
            try
            {
                var clippedMediaLink = storageContainer + "/" + vmName;

                var vhdList = new List<CmpInterfaceModel.Models.DataVirtualHardDisk>();
                var Lun = 2;

                var xDoc = new XmlDocument();
                xDoc.Load(new StringReader(diskSpec));
                var xDiveList = xDoc.SelectNodes("/Drives/Drive");

                if (xDiveList != null)
                    foreach (XmlNode xDrive in xDiveList)
                    {
                        if (-1 == xDrive["Letter"].InnerText.IndexOfAny(_ignoreDriveList))
                        {
                            var vhd = new CmpInterfaceModel.Models.DataVirtualHardDisk
                            {
                                //DiskName            = TheDiskName,
                                DiskName = null,
                                DiskLabel = vmName + "-" + xDrive["Letter"].InnerText,
                                HostCaching = "None",
                                LogicalDiskSizeInGB = xDrive["SizeInGB"].InnerText,
                                Lun = Lun++.ToString(),
                                MediaLink = clippedMediaLink + "-" + xDrive["Letter"].InnerText + ".vhd"
                            };

                            vhdList.Add(vhd);
                        }
                    }

                return vhdList;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetDataDiskList() : " + Utils.UnwindExceptionMessages(ex));
            }
        }

        ArmStorageprofileDatadisk[] GetArmDataDiskList(
            string storageContainer, string vmName, string diskSpec)
        {
            try
            {
                var clippedMediaLink = storageContainer + "/" + vmName;

                var vhdList = new List<ArmStorageprofileDatadisk>();
                var lun = 2;

                var xDoc = new XmlDocument();
                xDoc.Load(new StringReader(diskSpec));
                var xDiveList = xDoc.SelectNodes("/Drives/Drive");

                if (null == xDiveList)
                    return null;

                if (0 == xDiveList.Count)
                    return null;

                foreach (XmlNode xDrive in xDiveList)
                {
                    if (-1 == xDrive["Letter"].InnerText.IndexOfAny(_ignoreDriveList))
                    {
                        var vhd = new ArmStorageprofileDatadisk
                        {
                            name = vmName + "-" + xDrive["Letter"].InnerText + ".vhd",
                            diskSizeGB = xDrive["SizeInGB"].InnerText,
                            lun = lun++,
                            vhd =
                                new ArmStorageprofileDatadiskVhd1()
                                {
                                    uri = clippedMediaLink + "-" + xDrive["Letter"].InnerText + ".vhd"
                                },
                            createOption = "Empty"
                        };

                        vhdList.Add(vhd);
                    }
                }

                return vhdList.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetArmDataDiskList() : " + Utils.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets the details of the each of the disks belonging to the VM 
        /// associated with the given CMP request ID
        /// </summary>
        /// <returns>The details of each of the disks belonging to the VM 
        /// associated with the given CMP request ID</returns>
        /// 
        //*********************************************************************

        List<CmpInterfaceModel.Models.DiskSpec> GetDiskSpecList(CmpVmRequest cmpVmReq, string diskSpec)
        {
            try
            {
                var vhdList = new List<CmpInterfaceModel.Models.DiskSpec>();
                var Lun = 2;

                var sql = (bool)cmpVmReq.RequestRecord.SQLBuildOut;


                var xDoc = new XmlDocument();
                xDoc.Load(new StringReader(diskSpec));
                var xDiveList = xDoc.SelectNodes("/Drives/Drive");

                if (xDiveList != null)
                    foreach (XmlNode xDrive in xDiveList)
                    {
                        if (-1 == xDrive["Letter"].InnerText.IndexOfAny(_ignoreDriveList))
                        {
                            var Vhd = new CmpInterfaceModel.Models.DiskSpec
                            {
                                DriveLetter = xDrive["Letter"].InnerText,
                                LogicalDiskSizeInGB = xDrive["SizeInGB"].InnerText,
                                Lun = Lun++.ToString(),
                            };

                            Vhd.BlockSizeK = sql && !Vhd.DriveLetter.Equals("C") && !Vhd.DriveLetter.Equals("D") ? 64 : 0;
                            vhdList.Add(Vhd);
                        }
                    }

                return vhdList;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetDiskSpecList() : " + Utils.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Creates a string containing the XML configuration string for a CMP
        /// build request from the given CmpService.VmDeploymentRequest and
        /// CmpVmRequest objects
        /// </summary>
        /// <param name="cdr">The deployment request for the VM</param>
        /// <param name="cmpVmReq">The CMP VM request for the VM</param>
        //*********************************************************************

        string BuildCmpConfig(CmpService.VmDeploymentRequest cdr, CmpVmRequest cmpVmReq)
        {
            try
            {
                bool isLinux = cmpVmReq.RequestRecord.OSCode.Equals("Linux");
                var computerName = cmpVmReq.RequestRecord.RequestName;
                var roleName = computerName;
                var hsName = GetHostServiceName(cmpVmReq, true);
                var DeploymentSlot = "Production";
                var storageContainer = GetStorageContainer(cmpVmReq);
                var mediaLink = storageContainer + "/" + computerName + "/" + computerName + "-C.vhd"; //* Sample - "http://markwestmsftinternalwest.blob.core.windows.net/vhds/-----.vhd";
                var diskName = computerName + "-" + computerName + "-C";
                X509Certificate2 serviceCert = null;

                //*** Hosted Service ***

                var hostServ = new CmpInterfaceModel.Models.HostedService
                {
                    Description = roleName,
                    Label = CmpInterfaceModel.Utilities.StringToB64(hsName),
                    ServiceName = hsName,

                };

                //*** Certificate ***

                List<CmpInterfaceModel.Models.CertificateFile> serviceCertFileList = null;

                /*if (UploadServiceCert_cb.Checked)
                {
                    try
                    {
                        ServiceCert = new X509Certificate2(VmServiceCertFileName_t.Text, VmServiceCertPassword_t.Text);
                        byte[] RawData = ServiceCert.GetRawCertData();
                        string RawDataB64 = Convert.ToBase64String(RawData);

                        CmpInterfaceModel.Models.CertificateFile CertFile = new CmpInterfaceModel.Models.CertificateFile
                        {
                            CertificateFormat = "pfx",
                            Data = RawDataB64,
                            Password = VmServiceCertPassword_t.Text
                        };

                        ServiceCertFileList = new List<CmpInterfaceModel.Models.CertificateFile>();
                        ServiceCertFileList.Add(CertFile);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("I got 100 problems son, and a certificate is one : " +
                            CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex),
                            "Certificate Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }*/

                //*** Disks ***

                var image = GetSourceImage(cmpVmReq);

                var ovd = new OsVirtualHardDisk
                {
                    MediaLink = mediaLink,
                    DiskName = diskName,
                    OS = image.OsFamily,
                    //OS = isLinux ? "Linux" : "Windows"
                };

                if (image.IsCustomImage)
                {
                    ovd.RemoteSourceImageLink = image.AzureImageName;
                }
                else
                {
                    ovd.SourceImageName = image.AzureImageName;
                }

                var vmDep = new CmpInterfaceModel.Models.AzureVmDeployment
                {
                    Name = GetHostServiceName(cmpVmReq, false),
                    Label = CmpInterfaceModel.Utilities.StringToB64(computerName),
                    DeploymentSlot = DeploymentSlot,
                    VirtualNetworkName = CmpInterfaceModel.Constants.AUTOVNET
                };

                //*** WinRM ***

                CmpInterfaceModel.Models.Listener list;

                if (null != serviceCert)
                    list = new CmpInterfaceModel.Models.Listener { Protocol = "Https", CertificateThumbprint = serviceCert.Thumbprint };
                else
                    list = new CmpInterfaceModel.Models.Listener { Protocol = "Https" };

                var listList = new List<CmpInterfaceModel.Models.Listener> {list};
                var wrm = new CmpInterfaceModel.Models.WinRM { Listeners = listList };

                //*** ConfigSet ***

                var vmAdminUserInfo = GetVmAdminInfo(cmpVmReq);
                ConfigurationSet cs = null;

                if (isLinux)
                {
                    //*** Linux ***

                    cs = new LinuxProvisioningConfigurationSet
                    {
                        ConfigurationSetType = "LinuxProvisioningConfiguration",
                        DisableSshPasswordAuthentication = "true",
                        HostName = computerName,
                        SSH = null,
                        UserName = vmAdminUserInfo.UserName,
                        UserPassword = vmAdminUserInfo.Password
                    };
                }
                else
                {
                    cs = new CmpInterfaceModel.Models.WindowsProvisioningConfigurationSet
                    {
                        AdminUsername = vmAdminUserInfo.UserName,
                        AdminPassword = vmAdminUserInfo.Password,
                        ComputerName = computerName,
                        ConfigurationSetType = "WindowsProvisioningConfiguration",
                        EnableAutomaticUpdates = "true",
                        WinRM = wrm
                        //DomainJoin = DJ,
                        //StoredCertificateSettings = CertSetList,
                        //TimeZone = "TZ",
                    };
                };

                //*** Domain Join ***

                Models.AdDomainMap domainInfo = null;
                var csList = new List<CmpInterfaceModel.Models.ConfigurationSet>();
                List<CmpInterfaceModel.Models.InputEndpoint> ieList = null;

                if (isLinux)
                {
                    //*** Open SSH port ***

                    var ie1 = new CmpInterfaceModel.Models.InputEndpoint
                    {
                        Port = "22",
                        LocalPort = "22",
                        Name = "SSH",
                        Protocol = "tcp"
                    };

                    ieList = new List<CmpInterfaceModel.Models.InputEndpoint> { ie1 };
                }
                else
                {
                    domainInfo = GetDomainInfo(cmpVmReq);

                    if (null != domainInfo)
                    {
                        /*var dj = new CmpInterfaceModel.Models.DomainJoin
                    {
                        Credentials = di,
                        JoinDomain = di.Domain
                    };

                    cs.DomainJoin = dj;*/

                        //var domainInfo = GetDomainInfo(cmpVmReq);

                        string djUserDomain = null;
                        string djUserName = null;

                        if (domainInfo.JoinCredsUserName.Contains('\\'))
                        {
                            var djCredSplit = domainInfo.JoinCredsUserName.Split('\\');
                            djUserDomain = djCredSplit[0];
                            djUserName = djCredSplit[1];
                        }
                        else
                        {
                            djUserDomain = domainInfo.DomainFullName;
                            djUserName = domainInfo.JoinCredsUserName;
                        }

                        var djCreds = new CmpInterfaceModel.Models.Credentials
                        {
                            Domain = djUserDomain,
                            Password = domainInfo.JoinCredsPasword,
                            Username = djUserName
                        };

                        var dj = new CmpInterfaceModel.Models.DomainJoin
                        {
                            Credentials = djCreds,
                            JoinDomain = domainInfo.DomainFullName,
                            MachineObjectOU = domainInfo.ServerOU
                        };

                        cs.DomainJoin = dj;
                    }
                    else
                    {
                        //*** If we don't join a domain then create public ports ***

                        var ie1 = new CmpInterfaceModel.Models.InputEndpoint
                        {
                            //Port = "5986",
                            LocalPort = "5986",
                            Name = "WinRmHTTPs",
                            Protocol = "tcp"
                        };

                        var ie2 = new CmpInterfaceModel.Models.InputEndpoint
                        {
                            //LoadBalancedEndpointSetName = "LBESN",
                            //LoadBalancerProbe = LBP,
                            //Port = "1234",
                            LocalPort = "3389",
                            Name = "RDP",
                            Protocol = "tcp"
                        };

                        ieList = new List<CmpInterfaceModel.Models.InputEndpoint> {ie1, ie2};
                    }
                }

                var subNameList = new CmpInterfaceModel.Models.SubnetNames { SubnetName = CmpInterfaceModel.Constants.AUTOSUBNETNAME };

                var cs2 = new CmpInterfaceModel.Models.NetworkConfigurationSet
                {
                    ConfigurationSetType = "NetworkConfiguration",
                    SubnetNames = subNameList,
                    InputEndpoints = ieList
                };

                csList.Add(cs);
                csList.Add(cs2);

                //*** Disks ***

                var vmRole = new CmpInterfaceModel.Models.PersistentVMRole
                {
                    //AvailabilitySetName = "AsName",
                    ConfigurationSets = csList,
                    DataVirtualHardDisks = GetDataDiskList(storageContainer, computerName, cmpVmReq.RequestRecord.StorageConfigXML),
                    OSVirtualHardDisk = ovd,
                    RoleName = roleName,
                    RoleSize = GetVmSize(cmpVmReq),
                    RoleType = "PersistentVMRole",
                    ProvisionGuestAgent = "true",
                    Label = CmpInterfaceModel.Utilities.StringToB64(roleName)
                };

                vmDep.RoleList = new List<CmpInterfaceModel.Models.Role> 
                {vmRole};

                //***

                var placement = new CmpInterfaceModel.Models.PlacementSpec
                {
                    Method = CmpInterfaceModel.Models.PlacementMethodEnum.AutoDefault.ToString(),
                    TargetServiceProviderAccountID = GetDefaultTargetServiceProviderAccountID(),

                    TargetServiceProviderAccountGroup = cmpVmReq.RequestRecord.ResourceGroup,//GetDefaultTargetServiceProviderAccountGroup(),
                };

                if (null != domainInfo)
                {
                    placement.ServerOu = domainInfo.ServerOU;
                    placement.WorkstationOu = domainInfo.WorkstationOU;
                }

                //***
                var vmSetStaticIpAddr = System.Configuration.ConfigurationManager.AppSettings["VmSetStaticIpAddr"];

                if (null != vmSetStaticIpAddr)
                    if (vmSetStaticIpAddr.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    {
                        placement.Config = (new PlacementConfig { UseStaticIpAddr = true }).Serialize();
                        //cs2.StaticVirtualNetworkIPAddress = CmpInterfaceModel.Constants.AUTOSTATICIPADDRESS;
                    }

                var cmdbConfig = new CmpInterfaceModel.Models.CmdbSpec
                {
                    ServiceCategory = cmpVmReq.RequestRecord.ITSMServiceCategory,
                    //*** TODO * Markw * Temp
                    //UpdateCmdb = true,
                    UpdateCmdb = true,
                    Nic1 = cmpVmReq.RequestRecord.NIC1_Config,
                    ITSMMonitoredFlag = cmpVmReq.RequestRecord.ITSMMonitoredFlag == null ? "false" : cmpVmReq.RequestRecord.ITSMMonitoredFlag.ToString(),
                    EnvironmentClass = cmpVmReq.RequestRecord.EnvironmentName,
                    OrgId = cmpVmReq.RequestRecord.OrgID == null ? null : cmpVmReq.RequestRecord.OrgID.ToString(),
                    OrgFinancialAssetOwner = cmpVmReq.RequestRecord.OrgFinancialAssetOwner,
                    OrgChargebackGroup = cmpVmReq.RequestRecord.OrgChargebackGroup,
                    ITSMResponsibleOwner = cmpVmReq.RequestRecord.ITSMResponsibleOwner,
                    ITSMAccountableOwner = cmpVmReq.RequestRecord.ITSMAccountableOwner,
                    ITSMCIOwner = cmpVmReq.RequestRecord.ITSMCIOwner

                };

                var arr = cmpVmReq.RequestRecord.SQLAdminGroup == null ? null : cmpVmReq.RequestRecord.SQLAdminGroup.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                var Sqlspec = new CmpInterfaceModel.Models.SqlSpec
                {
                    InstallSql = cmpVmReq.RequestRecord.SQLBuildOut,
                    InstallAnalysisServices = cmpVmReq.RequestRecord.SQLEnableSSAS,
                    InstallReplicationServices = cmpVmReq.RequestRecord.SQLEnableReplication,
                    InstallIntegrationServicesallSql = cmpVmReq.RequestRecord.SQLEnableSSIS,
                    SqlInstancneName = cmpVmReq.RequestRecord.SQLInstanceName,
                    Collation = cmpVmReq.RequestRecord.SQLCollation,
                    Version = cmpVmReq.RequestRecord.SQLVersionName,
                    AdminGroupList = arr == null ? null : arr.ToList(),
                    AnalysisServicesMode = cmpVmReq.RequestRecord.SQLSSASMode

                };

                var iisSpec = new CmpInterfaceModel.Models.IisSpec
                {
                    InstallIis = cmpVmReq.RequestRecord.IISBuildOut,
                    RoleServices = cmpVmReq.RequestRecord.IISServiceRole
                };

                var applicationspec = new CmpInterfaceModel.Models.ApplicationSpec
                {
                    IisConfig = iisSpec,
                    SqlConfig = Sqlspec
                };

                 // required for setting certain custom validations picked from DB.
                 var validationConfig = new CmpInterfaceModel.Models.ValidateSpec { Validate = false };

                List<SequenceSpec> scriptList = null;

                if (true)
                {
                    scriptList = new List<SequenceSpec>();

                    if (cmpVmReq.RequestRecord.SQLBuildOut)
                    {
                        scriptList.Add(GetSqlSequenceSpec(cmpVmReq));
                    }

                    if (cmpVmReq.RequestRecord.IISBuildOut)
                    {
                        scriptList.Add(GetIisSequenceSpec(cmpVmReq));
                    }

                }

                //**************************************************************
                //**************************************************************
                //*** ARM Below ************************************************
                //**************************************************************
                //**************************************************************

                /*var armVars = new ArmVariables()
                {
                    location = "West US",
                    imagePublisher = "MicrosoftWindowsServer",
                    imageOffer = "WindowsServer",
                    OSDiskName = "osdiskforwindowssimple",
                    nicName = "myvmnic",
                    //addressPrefix = "10.0.0.0/16",
                    subnetName = "Subnet",
                    //subnetPrefix = "10.0.0.0/24",
                    //storageAccountType = "Standard_LRS",
                    publicIPAddressName = "mypublicip",
                    publicIPAddressType = "Dynamic",
                    vmStorageAccountContainerName = "vhds",
                    vmName = "mywindowsvm",
                    vmSize = "Standard_D1",
                    virtualNetworkName = "myvnet",
                    vnetID = "[resourceId('Microsoft.Network/virtualNetworks',variables('virtualNetworkName'))]",
                    subnetRef = "[concat(variables('vnetID'),'/subnets/',variables('subnetName'))]",
                    //*** ***
                    adminPassword = "123Abc!!!",
                    adminUsername = "markw",
                    dnsNameForPublicIP = "armtestn27",
                    newStorageAccountName = "armtestn27",
                    windowsOSVersion = "2012-R2-Datacenter"
                };*/

                var adminUserInfo = GetVmAdminInfo(cmpVmReq);

                cmpVmReq.RequestRecord.AzureImagePublisher = image.AzureImagePublisher;
                cmpVmReq.RequestRecord.AzureImageOffer = image.AzureImageOffer;
                cmpVmReq.RequestRecord.AzureWindowsOSVersion = image.AzureWindowsOSVersion;

                computerName = computerName.ToLower();

                var safeServiceName = new string(cdr.TargetServiceName.Where(char.IsLetterOrDigit).ToArray()).ToLower();

                var armVars = new ArmVariables()
                {
                    location = cdr.TargetLocation,
                    //imagePublisher = "MicrosoftWindowsServer",
                    imagePublisher = cmpVmReq.RequestRecord.AzureImagePublisher,
                    //imageOffer = "WindowsServer",
                    imageOffer = cmpVmReq.RequestRecord.AzureImageOffer,
                    OSDiskName = diskName,
                    nicName = computerName + "_nic",
                    addressPrefix = "10.0.0.0/16",
                    //subnetName = computerName + "_subnet",
                    subnetName = safeServiceName + "_subnet",
                    subnetPrefix = "10.0.0.0/24",
                    storageAccountType = "Standard_LRS",
                    publicIPAddressName = computerName + "_ipaddr",
                    publicIPAddressType = "Dynamic",
                    //vmStorageAccountContainerName = CmpInterfaceModel.Constants.AUTOSTORAGECONTAINERNAME,
                    vmStorageAccountContainerName = "vhds",
                    vmName = computerName,
                    vmSize = GetVmSize(cmpVmReq),
                    //vmSize = "Standard_D1",                 //*** TODO * resolve size map
                    virtualNetworkName = safeServiceName + "_vnet",
                    vnetID = "[resourceId('Microsoft.Network/virtualNetworks',variables('virtualNetworkName'))]",
                    subnetRef = "[concat(variables('vnetID'),'/subnets/',variables('subnetName'))]",
                    //*** ***
                    adminPassword = adminUserInfo.Password,
                    adminUsername = adminUserInfo.UserName,
                    dnsNameForPublicIP = computerName,
                    //newStorageAccountName = CmpInterfaceModel.Constants.AUTOSTORAGEACCOUNTNAME,
                    //newStorageAccountName = safeServiceName + "store",
                    newStorageAccountName = computerName + "store"+_storageAccntRandomNum.Next(100000),
                    //windowsOSVersion = "2012-R2-Datacenter"
                    windowsOSVersion = cmpVmReq.RequestRecord.AzureWindowsOSVersion,
                    nsgName = computerName + "_nsg",
                    nsgId = "[resourceId('Microsoft.Network/networkSecurityGroups',variables('nsgName'))]",
                };

                var armResources = new ArmResource[6];
                int index = 0;
                var vhdStorageContainer = string.Format("http://{0}.blob.core.windows.net/{1}", armVars.newStorageAccountName, armVars.vmStorageAccountContainerName);

                //*** Storage account ***

                armResources[index++] = new ArmResource()
                {
                    apiVersion = "2015-05-01-preview",
                    type = "Microsoft.Storage/storageAccounts",
                    name = "[variables('newStorageAccountName')]",
                    location = "[variables('location')]",
                    tags = new ArmTags() { displayName = "StorageAccount" },
                    properties = new ArmResourceProperties() { accountType = "[variables('storageAccountType')]" }
                };

                //*** Public IP Address ***

                armResources[index++] = new ArmResource()
                {
                    apiVersion = "2015-05-01-preview",
                    type = "Microsoft.Network/publicIPAddresses",
                    name = "[variables('publicIPAddressName')]",
                    location = "[variables('location')]",
                    tags = new ArmTags() { displayName = "PublicIPAddress" },
                    properties = new ArmResourceProperties()
                    {
                        publicIPAllocationMethod = "[variables('publicIPAddressType')]",
                        dnsSettings = new ArmDnssettings() { domainNameLabel = "[variables('dnsNameForPublicIP')]" }
                    }
                };

                //*** Virtual Network ***

                armResources[index++] = new ArmResource()
                {
                    apiVersion = "2015-05-01-preview",
                    type = "Microsoft.Network/virtualNetworks",
                    name = "[variables('virtualNetworkName')]",
                    location = "[variables('location')]",
                    dependsOn = new string[] { "[variables('nsgId')]" },
                    tags = new ArmTags() { displayName = "VirtualNetwork" },
                    properties = new ArmResourceProperties()
                    {
                        addressSpace = new ArmAddressspace()
                        {
                            addressPrefixes = new string[] { "[variables('addressPrefix')]" }
                        },
                        subnets = new ArmSubnet[] 
                    { 
                        new ArmSubnet()
                        {
                            name = "[variables('subnetName')]", 
                            properties = new ArmSubnetProperties()
                            {
                                addressPrefix = "[variables('subnetPrefix')]",
                                networksecuritygroup = new ArmNetworkSecurityGroup(){id = "[variables('nsgId')]"}
                            }
                        } 
                    }
                    }
                };

                //*** Network Interface ***

                armResources[index++] = new ArmResource()
                {
                    apiVersion = "2015-05-01-preview",
                    type = "Microsoft.Network/networkInterfaces",
                    name = "[variables('nicName')]",
                    location = "[variables('location')]",
                    tags = new ArmTags() { displayName = "NetworkInterface" },
                    dependsOn = new string[]
                {
                    "[concat('Microsoft.Network/publicIPAddresses/', variables('publicIPAddressName'))]", 
                    "[concat('Microsoft.Network/virtualNetworks/', variables('virtualNetworkName'))]"
                },
                    properties = new ArmResourceProperties()
                    {
                        ipConfigurations = new ArmIpconfiguration[]
                    {
                        new ArmIpconfiguration()
                        {
                            name = "ipconfig", 
                            properties = new ArmIpconfigurationProperties()
                            {
                                privateIPAllocationMethod = "Dynamic", 
                                publicIPAddress = new ArmIpconfigurationPropertiesPublicipaddress()
                                {
                                    id = "[resourceId('Microsoft.Network/publicIPAddresses',variables('publicIPAddressName'))]"
                                }, 
                                subnet = new ArmIpconfigurationPropertiesSubnet(){id = "[variables('subnetRef')]"}
                            }
                        }
                    }
                    }
                };

                //*** Network Security Group ***

                armResources[index++] = new ArmResource()
                {
                    apiVersion = "2015-05-01-preview",
                    type = "Microsoft.Network/networkSecurityGroups",
                    name = "[variables('nsgName')]",
                    location = "[variables('location')]",
                    tags = new ArmTags() {displayName = "NetworkSecurityGroup"},
                    properties = new ArmResourceProperties()
                    {
                        securityRules = new Securityrule[]
                        {
                            new Securityrule()
                            {
                                name = "allow-WinRmHttp",
                                properties = new SecurityruleProperties()
                                {
                                    description = "Allow WinRM HTTP",
                                    protocol = "Tcp",
                                    sourcePortRange =  "*",
                                    destinationPortRange = "5985",
                                    sourceAddressPrefix = "*",
                                    destinationAddressPrefix = "*",
                                    access = "Allow",
                                    priority = 100,
                                    direction = "Inbound"
                                }
                            }, 
                            new Securityrule()
                            {
                                name = "allow-WinRmHttps",
                                properties = new SecurityruleProperties()
                                {
                                    description = "Allow WinRM HTTPs",
                                    protocol = "Tcp",
                                    sourcePortRange =  "*",
                                    destinationPortRange = "5986",
                                    sourceAddressPrefix = "*",
                                    destinationAddressPrefix = "*",
                                    access = "Allow",
                                    priority = 101,
                                    direction = "Inbound"
                                }
                            } , 
                            new Securityrule()
                            {
                                name = "allow-rdp",
                                properties = new SecurityruleProperties()
                                {
                                    description = "Allow RDP",
                                    protocol = "Tcp",
                                    sourcePortRange =  "*",
                                    destinationPortRange = "3389",
                                    sourceAddressPrefix = "*",
                                    destinationAddressPrefix = "*",
                                    access = "Allow",
                                    priority = 102,
                                    direction = "Inbound"
                                }
                            }  , 
                            new Securityrule()
                            {
                                name = "Allow-VnetInBound",
                                properties = new SecurityruleProperties()
                                {
                                    description = "Allow inbound traffic from all VMs in VNET",
                                    protocol = "*",
                                    sourcePortRange =  "*",
                                    destinationPortRange = "*",
                                    sourceAddressPrefix = "VirtualNetwork",
                                    destinationAddressPrefix = "VirtualNetwork",
                                    access = "Allow",
                                    priority = 650,
                                    direction = "Inbound"
                                }
                            }  , 
                            new Securityrule()
                            {
                                name = "Allow-AzureLoadBalancerInBound",
                                properties = new SecurityruleProperties()
                                {
                                    description = "Allow inbound traffic from azure load balancer",
                                    protocol = "*",
                                    sourcePortRange =  "*",
                                    destinationPortRange = "*",
                                    sourceAddressPrefix = "AzureLoadBalancer",
                                    destinationAddressPrefix = "*",
                                    access = "Allow",
                                    priority = 651,
                                    direction = "Inbound"
                                }
                            }  , 
                            new Securityrule()
                            {
                                name = "Deny-AllInBound",
                                properties = new SecurityruleProperties()
                                {
                                    description = "Deny all inbound traffic",
                                    protocol = "*",
                                    sourcePortRange =  "*",
                                    destinationPortRange = "*",
                                    sourceAddressPrefix = "*",
                                    destinationAddressPrefix = "*",
                                    access = "Deny",
                                    priority = 655,
                                    direction = "Inbound"
                                }
                            }  , 
                            new Securityrule()
                            {
                                name = "Allow-VnetOutBound",
                                properties = new SecurityruleProperties()
                                {
                                    description = "Allow outbound traffic from all VMs to all VMs in VNET",
                                    protocol = "*",
                                    sourcePortRange =  "*",
                                    destinationPortRange = "*",
                                    sourceAddressPrefix = "VirtualNetwork",
                                    destinationAddressPrefix = "VirtualNetwork",
                                    access = "Allow",
                                    priority = 650,
                                    direction = "Outbound"
                                }
                            }  , 
                            new Securityrule()
                            {
                                name = "Allow-InternetOutBound",
                                properties = new SecurityruleProperties()
                                {
                                    description = "Allow WinRM HTTPs",
                                    protocol = "*",
                                    sourcePortRange =  "*",
                                    destinationPortRange = "*",
                                    sourceAddressPrefix = "*",
                                    destinationAddressPrefix = "Internet",
                                    access = "Allow",
                                    priority = 651,
                                    direction = "Outbound"
                                }
                            }  , 
                            new Securityrule()
                            {
                                name = "Deny-AllOutBound",
                                properties = new SecurityruleProperties()
                                {
                                    description = "Deny all outbound traffic",
                                    protocol = "*",
                                    sourcePortRange =  "*",
                                    destinationPortRange = "*",
                                    sourceAddressPrefix = "*",
                                    destinationAddressPrefix = "*",
                                    access = "Deny",
                                    priority = 655,
                                    direction = "Outbound"
                                }
                            } 
                        }
                    }
                };

                //*** Virtual Machine ***

                armResources[index++] = new ArmResource()
                {
                    apiVersion = "2015-05-01-preview",
                    type = "Microsoft.Compute/virtualMachines",
                    name = "[variables('vmName')]",
                    location = "[variables('location')]",
                    tags = new ArmTags() { displayName = "VirtualMachine" },
                    dependsOn = new string[]
                    {
                        "[concat('Microsoft.Storage/storageAccounts/', variables('newStorageAccountName'))]",
                        "[concat('Microsoft.Network/networkInterfaces/', variables('nicName'))]"                
                    },
                    properties = new ArmResourceProperties()
                    {
                        hardwareProfile = new ArmHardwareprofile()
                        {
                            vmSize = "[variables('vmSize')]"
                        },
                        osProfile = new ArmOsprofile()
                        {
                            computername = "[variables('vmName')]",
                            adminUsername = "[variables('adminUsername')]",
                            adminPassword = "[variables('adminPassword')]",
                            windowsConfiguration = new Windowsconfiguration()
                            {
                                provisionVMAgent = true,
                                enableAutomaticUpdates = false/*,
                                winRM = new Winrm()
                                {
                                    listeners = new Listener[1]
                                    {
                                        new Listener() { Protocol = "https" }
                                    }
                                }*/
                            }
                        },
                        storageProfile = new ArmStorageprofile()
                        {
                            imageReference = new ArmStorageprofileImagereference()
                            {
                                publisher = "[variables('imagePublisher')]",
                                offer = "[variables('imageOffer')]",
                                sku = "[variables('windowsOSVersion')]",
                                version = "latest"
                            },
                            osDisk = new ArmStorageprofileOsdisk()
                            {
                                name = "osdisk",
                                caching = "ReadWrite",
                                createOption = "FromImage",
                                vhd = new ArmStorageprofileOsdiskVhd() { uri = "[concat('http://',variables('newStorageAccountName'),'.blob.core.windows.net/',variables('vmStorageAccountContainerName'),'/',variables('OSDiskName'),'.vhd')]" }
                                //vhd = new ArmStorageprofileOsdiskVhd() { uri = CmpInterfaceModel.Constants.AUTOBLOBSTORELOCATION + "/" + armVars.OSDiskName + ".vhd" }
                            },
                            dataDisks = GetArmDataDiskList(vhdStorageContainer, computerName, cmpVmReq.RequestRecord.StorageConfigXML)
                        },
                        networkProfile = new ArmNetworkprofile()
                        {
                            networkInterfaces = new ArmNetworkprofileNetworkinterface[]
                        {
                            new ArmNetworkprofileNetworkinterface() 
                            { id = "[resourceId('Microsoft.Network/networkInterfaces',variables('nicName'))]" }
                        }
                        }
                    }
                };

                //*** ***

                var vmDepArm = new AzureVmDeploymentArm
                {
                    properties = new ArmRootProperties()
                    {
                        mode = "Incremental",
                        template = new ArmTemplate()
                        {
                            contentVersion = "1.0.0.0",
                            schema = "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
                            //parameters = armParams, 
                            resources = armResources,
                            variables = armVars
                            //mode = "Incremental"
                        }
                    }
                };

                //**************************************************************
                //**************************************************************
                //*** ARM Above ************************************************
                //**************************************************************
                //**************************************************************

                var vmc = new CmpInterfaceModel.Models.VmConfig
                {
                    AzureArmConfig = vmDepArm,
                    AzureVmConfig = vmDep,
                    HostedServiceConfig = hostServ,
                    ServiceCertList = serviceCertFileList,
                    DiskSpecList = GetDiskSpecList(cmpVmReq, cmpVmReq.RequestRecord.StorageConfigXML),
                    Placement = placement,
                    UserSpecList = GetLocalUserList(cmpVmReq, null == domainInfo ? null : domainInfo.DefaultVmAdminMember),
                    PageFileConfig = GetPageFileSpec(cmpVmReq),
                    CmdbConfig = cmdbConfig,
                    ValidationConfig = validationConfig,
                    ScriptList = scriptList,
                    ApplicationConfig = applicationspec
                };

                return vmc.Serialize();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in BuildCmpConfig() : " + Utils.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// <summary>
        /// Builds SQL sequence node from the given CMP VM request
        /// </summary>
        /// <param name="cmpVmReq">CMP VM request to build the SQL sequence node from</param>
        /// <returns>The SQL sequence node for the VM</returns>
        /// 
        //*********************************************************************
        SequenceSpec GetSqlSequenceSpec(CmpVmRequest cmpVmReq)
        {
            if (!(cmpVmReq.RequestRecord.SQLBuildOut))
            {
                return null;
            }

            try
            {
                //sql ipak sequence construction starts here

                var domainInfo = GetDomainInfo(cmpVmReq);

                var iPakMapList = FetchIpakVersionMap(
                    cmpVmReq.RequestRecord.SQLVersionName, domainInfo.DomainShortName);

                if (0 == iPakMapList.Count)
                    throw new Exception("No IPAK record found for SQLVersionCode : " + cmpVmReq.RequestRecord.SQLVersionName + " and Domain" + domainInfo.DomainShortName);

                const string IPAK_SQL_TEMPLATE =
                @"/COLLATION:{0} /AS:{1} /ASSERVERMODE:{2} /SSIS:{3} /BIN:{4} /BAK:{5} /DAT:{6} /TRAN:{7} /TEMP:{8} /QFE:{9} /SQLAdmin:{10} /CORP /NoLogcopy";

                const int _IPAK_SQL_TIMEOUT_MINUTES = 60;

                //var arr = cmpVmReq.RequestRecord.SQLAdminGroup == null ? null : cmpVmReq.RequestRecord.SQLAdminGroup.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                var iPakParams = String.Format(IPAK_SQL_TEMPLATE,
                    cmpVmReq.RequestRecord.SQLCollation,
                    (bool)cmpVmReq.RequestRecord.SQLEnableSSAS ? "1" : "0",
                    cmpVmReq.RequestRecord.SQLSSASMode,
                    (bool)cmpVmReq.RequestRecord.SQLEnableSSIS ? "1" : "0",
                    "D",//cmpVmReq.RequestRecord.SQLBinariesDrive,
                    "E",//cmpVmReq.RequestRecord.SQLBackupDrive,
                    "H",//cmpVmReq.RequestRecord.SQLDataDrive,
                    "O",//cmpVmReq.RequestRecord.SQLLogDrive,
                    "T",//cmpVmReq.RequestRecord.SQLTempDBDrive,
                    iPakMapList[0].QfeVersion,
                    cmpVmReq.RequestRecord.SQLAdminGroup);

                iPakParams = iPakParams.Replace("Multi-Dimensional", "MULTIDIMENSIONAL");
                iPakParams = iPakParams.Replace("Tabular Cube", "TABULAR");

                var paramList = new List<PoshParamSpec>
                {
                    new PoshParamSpec() {Name = "IPAKParams", Value = iPakParams},
                    new PoshParamSpec() {Name = "IPAKSource", Value = iPakMapList[0].IpakDirLocation, SelectReachableHost = true},
                    new PoshParamSpec() {Name = "ServerName", Value = cmpVmReq.RequestRecord.RequestName + "." + domainInfo.DomainFullName},
                    new PoshParamSpec() {Name = "TimeoutMinutes", Value = _IPAK_SQL_TIMEOUT_MINUTES.ToString()}
                };

                var seq = new SequenceSpec
                {
                    BreakOn = CmpInterfaceModel.Models.SequenceSpec.BreakOnEnum.Exception.ToString(),
                    Config = null,
                    Engine = CmpInterfaceModel.Models.SequenceSpec.SequenceEngineEnum.SMA.ToString(),
                    ExecuteInState = CmpInterfaceModel.Constants.StatusEnum.PostProcessing1.ToString(),
                    Waitmode = CmpInterfaceModel.Models.SequenceSpec.WaitmodeEnum.Asynchronous.ToString(),
                    TimeoutMinutes = _IPAK_SQL_TIMEOUT_MINUTES,
                    ID = 0,
                    Locale = CmpInterfaceModel.Models.SequenceSpec.SequenceLocaleEnum.Remote.ToString(),
                    Name = "IPAK SQL Installation",
                    TagData = null,
                    ScriptList = null,
                    SmaConfig = new SmaConfigSpec()
                    {
                        SmaServerUrl = System.Configuration.ConfigurationManager.AppSettings["SMAService"],
                        RunbookName = "Install-WFIpak",
                        ParamList = paramList
                    }
                };

                return seq;
                //sql ipak sequence construction ends here
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetSqlSequenceSpec() : " + Utils.UnwindExceptionMessages(ex));
            }
        }


        //*********************************************************************
        ///
        /// <summary>
        /// Creates an IIS sequence spec from the given CMPVmRequest
        /// </summary>
        /// <param name="cmpVmReq">The CMP VM request to create the IIS
        /// sequence from</param>
        /// <returns>The IIS sequence spec</returns>
        /// 
        //*********************************************************************
        SequenceSpec GetIisSequenceSpec(CmpVmRequest cmpVmReq)
        {
            if (!(cmpVmReq.RequestRecord.IISBuildOut))
            {
                return null;
            }

            try
            {
                var domainInfo = GetDomainInfo(cmpVmReq);

                var iPakMapList = FetchIpakVersionMap(
                    "IIS", domainInfo.DomainShortName);

                if (0 == iPakMapList.Count)
                    throw new Exception("No IPAK record found for IisVersionCode : IIS and Domain " + domainInfo.DomainShortName);

                var dMap = GetDomainInfo(cmpVmReq);

                const string IPAK_IIS_TEMPLATE =
                @"/IIS /WEBROOT:D:\\webroot /LOGDIR:E:\wwwlog /UPTORELEASE:1999JAN /CORP /NoLogcopy";

                const int _IPAK_IIS_TIMEOUT_MINUTES = 60;

                var paramList = new List<PoshParamSpec>   
                {
                    new PoshParamSpec() {Name = "IPAKParams", Value = IPAK_IIS_TEMPLATE},
                    new PoshParamSpec() { Name = "IPAKSource", Value = iPakMapList[0].IpakDirLocation },
                    new PoshParamSpec() {Name = "ServerName", Value = cmpVmReq.RequestRecord.RequestName + "." + dMap.DomainFullName},
                    new PoshParamSpec() {Name = "TimeoutMinutes", Value = _IPAK_IIS_TIMEOUT_MINUTES.ToString()}
                };

                var seq = new SequenceSpec
                {
                    BreakOn = CmpInterfaceModel.Models.SequenceSpec.BreakOnEnum.Exception.ToString(),
                    Config = null,
                    Engine = CmpInterfaceModel.Models.SequenceSpec.SequenceEngineEnum.SMA.ToString(),
                    ExecuteInState = CmpInterfaceModel.Constants.StatusEnum.PostProcessing1.ToString(),
                    Waitmode = CmpInterfaceModel.Models.SequenceSpec.WaitmodeEnum.Asynchronous.ToString(),
                    ID = 0,
                    Locale = CmpInterfaceModel.Models.SequenceSpec.SequenceLocaleEnum.Remote.ToString(),
                    Name = "IPAK IIS Installation",
                    TagData = null,
                    ScriptList = null,
                    SmaConfig = new SmaConfigSpec()
                    {
                        SmaServerUrl = System.Configuration.ConfigurationManager.AppSettings["SMAService"],
                        RunbookName = "Install-WFIpak",
                        ParamList = paramList
                    }
                };

                return seq;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetIisSequenceSpec() : " + Utils.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        //*********************************************************************

        void FS_WritingEntity(object sender, System.Data.Services.Client.ReadingWritingEntityEventArgs e)
        {
            //Uri TheUrl = e.BaseUri;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        //*********************************************************************

        void FS_SendingRequest2(object sender, System.Data.Services.Client.SendingRequest2EventArgs e)
        {
            //Uri TheUrl = e.RequestMessage.Url;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets the host service name for the given cmpVmRequest
        /// </summary>
        /// <param name="cmpVmReq">The CMP VM request to get the 
        /// host service name for</param>
        /// <param name="addDigits">Option to append a "-01" to the 
        /// end of the host service name</param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        string GetHostServiceName(CmpVmRequest cmpVmReq, bool addDigits)
        {
            // return cmpVmReq.RequestRecord.RequestName;

            if (null == cmpVmReq.RequestRecord.AppID)
                throw new Exception("Wap request AppID == NULL");

            if (0 == cmpVmReq.RequestRecord.AppID.Length)
                throw new Exception("Wap request AppID == ''");

            var ServName = cmpVmReq.RequestRecord.AppID;

            //ServName = new string(ServName.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-').ToArray());
            ServName = new string(ServName.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());

            if (!Char.IsLetter(ServName.FirstOrDefault()))
                ServName = "S" + ServName;

            //*** always end with a two digit number, to support multiple host services per app (in the future)

            if (12 < ServName.Length)
                ServName = ServName.Substring(0, 12);

            if (addDigits)
                ServName = ServName + "-01";

            return ServName;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets the source image name for the CMP VM request
        /// </summary>
        /// <param name="cmpVmReq">The CMP VM request to get the source image 
        /// name for</param>
        /// <returns>the source image name for the given CMP VM Ruquest
        /// </returns>
        /// 
        //*********************************************************************

        private VmOs GetSourceImage(CmpVmRequest cmpVmReq)
        {
            //return "bd507d3a70934695bc2128e3e5a255ba__RightImage-Windows-2012-x64-iis8-v5.8.8.12";
            /*foreach (string[] OS in _VmOsMap)
                if (cmpVmReq._RequestRecord.OSName.Contains(OS[0]))
                    return OS[1];

            return _VmOsMap[0][1];*/

            //return cmpVmReq.RequestRecord.OSName;

            var oSMapList = FetchOsInfoList();

            Models.VmOs selectedOsMap = null;

            selectedOsMap = oSMapList.Where(o => o.Name.Contains(cmpVmReq.RequestRecord.OSName)).Select(p => p).FirstOrDefault();

            if (null == selectedOsMap)
                throw new Exception("INTERNAL: Specified source image name (" +
                    cmpVmReq.RequestRecord.OSName + ") not found in VmOsMap table");

            if (!selectedOsMap.IsCustomImage)
            {
                return selectedOsMap;                
            }

            var vmAzureRegionMap = FetchAzureRegionMap();

            foreach (var rm in vmAzureRegionMap)
                if (cmpVmReq.RequestRecord.AzureRegionName.Contains(rm.Name))
                {
                    selectedOsMap.AzureImageName = rm.OsImageContainer + "/" + selectedOsMap.AzureImageName;
                    return selectedOsMap;
                }

            throw new Exception("INTERNAL: Specified AzureRegionCode (" +
                cmpVmReq.RequestRecord.AzureRegionCode + ") not found in AzureRegionMap table");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets the VM size from the given CMP VM request
        /// </summary>
        /// <param name="cmpVmReq">The CMP VM request to get the VM size for
        /// </param>
        /// <returns>The SKU size from the given CMP VM request</returns>
        /// 
        //*********************************************************************

        string GetVmSize(CmpVmRequest cmpVmReq)
        {
            /*foreach (string[] Size in _VmSizeMap)
                if (cmpVmReq._RequestRecord.SKU_CustomerName.Contains(Size[0]))
                    return Size[1];

            return CmpInterfaceModel.Constants.VmSizeEnum.Small.ToString();*/

            return cmpVmReq.RequestRecord.SKU_CustomerName;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets the storage container from the given CMP VM request
        /// </summary>
        /// <param name="cmpVmReq">The CMP VM request to get the VM size for
        /// </param>
        /// <returns>The storage container from the given CMP VM request
        /// </returns>
        /// 
        //*********************************************************************

        string GetStorageContainer(CmpVmRequest cmpVmReq)
        {
            //return "http://markwestmsftinternalwest.blob.core.windows.net/vhds";
            //return "http://sg1usw.blob.core.windows.net/vhds";
            return CmpInterfaceModel.Constants.AUTOBLOBSTORELOCATION;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Get the VM administrator info from the given CMP VM request
        /// </summary>
        /// <param name="cmpVmReq">The CMP VM requqest to get the VM 
        /// administrator for</param>
        /// <returns>The VM administrator info from the given CMP VM request</returns>
        /// 
        //*********************************************************************

        VmUserInfo GetVmAdminInfo(CmpVmRequest cmpVmReq)
        {
            if (null == cmpVmReq.RequestRecord.ActiveDirectoryDomain)
            {
                if (null == cmpVmReq.RequestRecord.VmAdminName)
                    throw new Exception("Exception in GetVmAdminInfo() : VmAdminName = null");

                if (null == cmpVmReq.RequestRecord.VmAdminPassword)
                    throw new Exception("Exception in GetVmAdminInfo() : VmAdminPassword = null");

                return new VmUserInfo
                {
                    UserName = cmpVmReq.RequestRecord.VmAdminName,
                    Password = cmpVmReq.RequestRecord.VmAdminPassword
                };
            }
            else
            {
                return new VmUserInfo
                {
                    UserName = CmpInterfaceModel.Constants.AUTOLOCALADMINUSERNAME,
                    Password = CmpInterfaceModel.Constants.AUTOLOCALADMINPASSWORD
                };
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets the local user list for the given CMP VM request
        /// </summary>
        /// <param name="cmpVmReq">The CMP VM request to get the local user
        /// list from</param>
        /// <param name="defaultVMAdminMember">The default VM administrator
        /// member for the given CMP VM request</param>
        /// <returns>The local user list for the given CMP VM request</returns>
        /// 
        //*********************************************************************

        System.Collections.Generic.List<CmpInterfaceModel.Models.UserSpec> GetLocalUserList(CmpVmRequest cmpVmReq, string defaultVmAdminMember)
        {
            /* if (null == cmpVmReq.RequestRecord.ActiveDirectoryDomain)
                 return null;

             if (null == cmpVmReq.RequestRecord.VmAdminName)
                 return null;

             var ml = new List<CmpInterfaceModel.Models.UserSpec>(1);

             string[] user = cmpVmReq.RequestRecord.VmAdminName.Split('\\');

             if(2>user.Count())
                 throw new Exception("Exception in GetLocalUserList() : Entity name not provided in <DomainName>\\<UserName> format");

             var us = new CmpInterfaceModel.Models.UserSpec
             {
                 DomainName = user[0],
                 EntityName = user[1],
                 GroupToJoinName = "administrators"
             };

             ml.Add(us);
             return ml;*/
            try
            {
                var ml =
                    new List<CmpInterfaceModel.Models.UserSpec>(1);

                /*string[] userUser = myCapReq._RequestRecord.RequestAdmins.Split('\\');

                ml.Add(new CmpInterfaceModel.Models.UserSpec
                {
                    DomainName = userUser[0],
                    EntityName = userUser[1],
                    GroupToJoinName = "administrators"
                });*/

                if (null != cmpVmReq.RequestRecord.RequestAdmins) if (0 < cmpVmReq.RequestRecord.RequestAdmins.Length)
                    {
                        var userList = cmpVmReq.RequestRecord.RequestAdmins.Split(new char[] { ',', ';' });

                        foreach (var user in userList)
                        {
                            var configUser = user.Split('\\');

                            if (2 == configUser.Count())
                                ml.Add(new CmpInterfaceModel.Models.UserSpec
                                {
                                    DomainName = configUser[0],
                                    EntityName = configUser[1],
                                    GroupToJoinName = "administrators"
                                });
                        }
                    }

                if (null != defaultVmAdminMember) if (0 < defaultVmAdminMember.Length)
                    {
                        var userList = defaultVmAdminMember.Split(new char[] { ',', ';' });

                        foreach (var user in userList)
                        {
                            var configUser = user.Split('\\');

                            if (2 == configUser.Count())
                                ml.Add(new CmpInterfaceModel.Models.UserSpec
                                {
                                    DomainName = configUser[0],
                                    EntityName = configUser[1],
                                    GroupToJoinName = "administrators"
                                });
                        }
                    }

                return ml;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetLocalUserList() : " + Utils.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets the page file spec for the given CMP VM request
        /// </summary>
        /// <param name="cmpVmReq">The CMP VM request to get the page file 
        /// spec from</param>
        /// <returns>The page file spec for the given CMP VM request</returns>
        /// 
        //*********************************************************************

        CmpInterfaceModel.Models.PagefileSpec GetPageFileSpec(CmpVmRequest cmpVmReq)
        {
            //commeting out old logic and implemented latest pagefile standards
            //var pfs = new CmpInterfaceModel.Models.PagefileSpec
            //{
            //    DiskName = "E"
            //};

            //if (cmpVmReq.RequestRecord.SQLBuildOut)
            //{
            //    pfs.DiskName = "G";
            //}

            //if (cmpVmReq.RequestRecord.IISBuildOut)
            //{
            //    pfs.DiskName = "F";
            //}
            var possibleDriveLetters = "DEFGHIJKLMNOPQRSTUVWXYZ";

            var pfs = new CmpInterfaceModel.Models.PagefileSpec { DiskName = possibleDriveLetters[0].ToString() };

            if (null == cmpVmReq)
                return pfs;

            if (null == cmpVmReq.RequestRecord)
                return pfs;

            if (null == cmpVmReq.RequestRecord.StorageConfigXML)
                return pfs;

            var xDoc = new XmlDocument();
            xDoc.Load(new StringReader(cmpVmReq.RequestRecord.StorageConfigXML));
            var xDiveList = xDoc.SelectNodes("/Drives/Drive");

            if (null != xDiveList)
                possibleDriveLetters = xDiveList.Cast<XmlNode>().Where(
                    xDrive => null != xDrive["Letter"]).Aggregate(possibleDriveLetters,
                    (current, xDrive) => current.Replace(xDrive["Letter"].InnerText.ToUpper(), " "));

            // if  the mycap request is for D Series and 

            pfs.DiskName = possibleDriveLetters.Trim()[0].ToString();

            return pfs;
            //return pfs;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets a list of domain information from the CMP DB 
        /// </summary>
        /// <returns>A list of domain information from the CMP DB</returns>
        ///
        //*********************************************************************
        public List<Models.AdDomainMap> FetchDomainInfoList()
        {
            var vmrList = new List<Models.AdDomainMap>();

            try
            {
                using (var db = new Models.MicrosoftMgmtSvcCmpContext())
                {
                    //db.Database.Connection.ConnectionString = _ConnectionString;

                    var vmrQ = from rb in db.AdDomainMaps
                               orderby rb.Id
                               select rb;


                    foreach (var reqRecord in vmrQ)
                    {
                        using (var xk = new KryptoLib.X509Krypto())
                        {
                            reqRecord.JoinCredsPasword = xk.DecrpytKText(reqRecord.JoinCredsPasword);
                            vmrList.Add(reqRecord);
                        }
                    }
                    return vmrList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchDomainInfoList() : "
                    + CmpServiceLib.Utilities.UnwindExceptionMessages(ex));
            }
        }


        //*********************************************************************
        ///
        /// <summary>
        /// Gets the IPAK versions
        /// </summary>
        /// <param name="versionName">Version name for the IPAK version</param>
        /// <param name="adDirectory">Ad directory of the IPAK version </param>
        /// <returns>List of the IPAK versions</returns>
        /// 
        //*********************************************************************
        public List<Models.IpakVersionMap> FetchIpakVersionMap(
           string versionName, string adDirectory)
        {
            using (var db = new Models.MicrosoftMgmtSvcCmpContext())
            {
                //db.Database.Connection.ConnectionString = _ConnectionString;

                var query = from rb in db.IpakVersionMaps
                            where rb.VersionName == versionName & rb.AdDomain == adDirectory
                            select rb;

                if (!query.Any())
                {
                    query = from rb in db.IpakVersionMaps
                            where rb.VersionName == versionName & rb.AdDomain == "*"
                            select rb;

                    return query.ToList();
                }
            }

            return null;
        }


        //*********************************************************************
        ///
        /// <summary>
        /// Gets the Azure region map from the CMP database
        /// </summary>
        /// <returns>The Azure region map from the CMP database</returns>
        /// 
        //*********************************************************************

        public List<Models.AzureRegion> FetchAzureRegionMap()
        {
            var ret = new List<Models.AzureRegion>();
            try
            {
                using (var db = new Models.MicrosoftMgmtSvcCmpContext())
                {
                    var query = from rb in db.AzureRegions
                                select rb;

                    foreach (var reqRecord in query)
                        ret.Add(reqRecord);
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchAzureRegionMap() : "
                    + CmpServiceLib.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        /// <summary>
        /// Gets the a list of OS information from the CMP database
        /// </summary>
        /// <returns>The list of OS information from the CMP database</returns>
        /// 
        //*********************************************************************
        
        public List<Models.VmOs> FetchOsInfoList()
        {
            var vmrList = new List<Models.VmOs>();

            try
            {                
                using (var db = new MicrosoftMgmtSvcCmpContext())
                {
                     var cc = db.Database.Connection.ConnectionString;
                    var vmrQ = from rb in db.VmOs
                               orderby rb.Name
                               select rb;

                    vmrList.AddRange(vmrQ);
                    return vmrList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchOsInfoList() : "
                    + CmpServiceLib.Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Gets the domain info for the given CMP VM request
        /// </summary>
        /// <param name="cmpVmReq">The CMP VM request to get the domain info
        /// for</param>
        /// <returns>The domain info of the given CMP VM request</returns>
        /// 
        //*********************************************************************

        Models.AdDomainMap GetDomainInfo(CmpVmRequest cmpVmReq)
        {
            if (null == cmpVmReq.RequestRecord.ActiveDirectoryDomain)
                return null;

            var dMapList = FetchDomainInfoList();

            foreach (var dMap in dMapList)
                if (dMap.DomainFullName.ToLower().Equals(cmpVmReq.RequestRecord.ActiveDirectoryDomain.ToLower()))
                    return dMap;

            throw new Exception("INTERNAL: Unable to locate record in CmpMycap DomainMap table for domain : " +
                cmpVmReq.RequestRecord.ActiveDirectoryDomain);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// DEPRECATED - Gets the domain info for the given CMP VM request
        /// </summary>
        /// <param name="cmpVmReq">The CMP VM request to get the domain info
        /// for</param>
        /// <returns>The domain info of the given CMP VM request</returns>
        /// 
        //*********************************************************************
        CmpInterfaceModel.Models.Credentials GetDomainInfo_Old(CmpVmRequest cmpVmReq)
        {
            var dMapList = FetchDomainInfoList();
            CmpInterfaceModel.Models.Credentials DjCreds = null;

            foreach (var dMap in dMapList)
                if (dMap.DomainFullName.ToLower().Equals(cmpVmReq.RequestRecord.ActiveDirectoryDomain.ToLower()))
                    DjCreds = new CmpInterfaceModel.Models.Credentials
                    {
                        Domain = dMap.DomainFullName,
                        Password = dMap.JoinCredsPasword,
                        Username = dMap.JoinCredsUserName
                    };

            return DjCreds;

            //*** TODO: Fix This

            CmpInterfaceModel.Models.Credentials djCreds = null;

            if (null == cmpVmReq.RequestRecord.ActiveDirectoryDomain)
                return djCreds;

            if (cmpVmReq.RequestRecord.ActiveDirectoryDomain.ToLower().Equals("redmond"))
            {
                djCreds = new CmpInterfaceModel.Models.Credentials
                {
                    Domain = "",
                    Password = "",
                    Username = ""
                };
            }

            return djCreds;
        }
    }
}
