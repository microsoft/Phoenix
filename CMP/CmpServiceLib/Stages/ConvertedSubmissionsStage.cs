using System;
using System.Collections.Generic;
using AzureAdminClientLib;
using CmpInterfaceModel;
using CmpInterfaceModel.Models;
using CmpServiceLib.Models;
using VmDeploymentRequest = CmpServiceLib.Models.VmDeploymentRequest;

namespace CmpServiceLib.Stages
{
    public class ConvertedSubmissionsStage : Stage
    {
        public string AftsDbConnectionString { get; set; }

        public int BlobsPerContainerLimit { get; set; }

        public string DefaultVhdContainerName { get; set; }

        public bool EnforceAppAgAffinity { get; set; }

        #region GetHostService

        public delegate HttpResponse GetHostServiceDelegate(
            VmDeploymentRequest vmReq, List<ServiceProviderAccount> servProvAcctList,
            out ServiceProviderAccount servProdAccount);

        public GetHostServiceDelegate GetHostService { get; set; }

        #endregion

        public Func<string, string> GetSafeHostServiceName { get; set; }

        public Func<VmDeploymentRequest, string> GetTargetServiceProviderAccountResourceGroup { get; set; }

        public int VmsPerVnetLimit { get; set; }

        public override object Execute()
        {
            const bool aftsDeleteSourceAfterTransfer = true;
            const bool aftsOverwriteDestinationBlob = true;

            //*** leave if Azure containers are not yet synchronized
            if (!ContainersSynchronized)
                return 0;

            if (AllQueuesBlocked)
                return 0;

            var cdb = new CmpDb(CmpDbConnectionString);

            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.Converted.ToString(), true);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessorVm.ProcessConvertedSubmissions() : " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }

            foreach (var vmReq in vmReqList)
            {
                try
                {
                    if (vmReq.RequestType.Equals(Constants.RequestTypeEnum.NewVM.ToString()))
                        continue;

                    //*** Temporary ***
                    vmReq.Config = vmReq.Config.Replace("<IsOS>True</IsOS>", "<IsOS>true</IsOS>");
                    vmReq.Config = vmReq.Config.Replace("<IsOS>False</IsOS>", "<IsOS>false</IsOS>");

                    if (!vmReq.Config.Contains("<IsOS>true</IsOS>"))
                        throw new Exception("Migration request does not contain a disk spec for an OS disk.");

                    //*** Get the target service provider account resource group ***
                    vmReq.ServiceProviderResourceGroup = GetTargetServiceProviderAccountResourceGroup(vmReq);

                    //*** Temporary ***
                    //vmReq.ServiceProviderResourceGroup = "DEV02";

                    //*** Get the list of service provider accounts associated with the resource group
                    var servProvAcctList = ServProvAccount.GetAzureServiceAccountList(
                        vmReq.ServiceProviderResourceGroup, CmpDbConnectionString);

                    var vmc = VmConfig.Deserialize(vmReq.Config);

                    if (null == vmc)
                        vmc = new VmConfig();

                    //*** Service Name ***

                    if (null == vmc.HostedServiceConfig)
                        vmc.HostedServiceConfig = new HostedService();

                    //*** TODO *** Handle a non CMDB case here

                    vmReq.TargetServicename = GetSafeHostServiceName(vmReq.ParentAppID);
                    vmc.HostedServiceConfig.ServiceName = vmReq.TargetServicename;
                    vmc.HostedServiceConfig.Label = Util.ToB64(vmc.HostedServiceConfig.ServiceName);
                    vmc.HostedServiceConfig.Description = vmc.CmdbConfig.ApplicationName;
                    //vmc.HostedServiceConfig.AffinityGroup = Constants.AUTOAFFINITYGROUP;
                    vmc.HostedServiceConfig.ExtendedProperties = null;

                    //*** Role Size ***

                    if (null == vmc.AzureVmConfig)
                        vmc.AzureVmConfig = new AzureVmDeployment();

                    if (null == vmc.AzureVmConfig.RoleList)
                        vmc.AzureVmConfig.RoleList = new List<Role>();

                    if (0 == vmc.AzureVmConfig.RoleList.Count)
                        vmc.AzureVmConfig.RoleList.Add(new CmpInterfaceModel.Models.PersistentVMRole());


                    //*** TODO *** Handle a non CMDB case here

                    ((PersistentVMRole)vmc.AzureVmConfig.RoleList[0]).RoleSize = vmc.CmdbConfig.AzureComputeSku;

                    vmReq.Config = vmc.Serialize();

                    //string storageAccountKey = "";

                    Models.ServiceProviderAccount servProdAccount;

                    var resp = GetHostService(vmReq, servProvAcctList, out servProdAccount);

                    if (resp.HadError)
                    {
                        if (resp.Retry)
                            continue;

                        throw new Exception("Unable to find/create a host service : " + resp.Body);
                    }

                    var cc = new AzureSubscription();
                    //cc.LoadStorageAccounts(servProdAccount.AccountID, servProdAccount.CertificateThumbprint);

                    //*****************

                    var vmCfg = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                    if (null == vmCfg.Placement)
                        throw new Exception("Placement element not found in vmReq.Config");

                    //servProdAccount.AzStorageContainerUrl = vmCfg.Placement.StorageContainerUrl;

                    //*****************

                    //var storageAcct = cc.GetStorageAccount(servProdAccount.AccountID, servProdAccount.CertificateThumbprint,
                    //    servProdAccount.AzStorageContainerUrl);

                    //********************************
                    //********************************

                    var diskCount = 1;
                    if (null != vmCfg.DiskSpecList)
                        diskCount += vmCfg.DiskSpecList.Count;

                    var connection =
                        ServProvAccount.GetAzureServiceAccountConnection(
                        servProdAccount.AccountID, servProdAccount.CertificateThumbprint,
                        servProdAccount.AzureADTenantId, servProdAccount.AzureADClientId, servProdAccount.AzureADClientKey);

                    var so = new AzureAdminClientLib.StorageOps(connection);

                    var container = so.GetLeastUsedContainer(DefaultVhdContainerName,
                    null, vmCfg.HostedServiceConfig.Location, BlobsPerContainerLimit,
                    VmsPerVnetLimit, diskCount, EnforceAppAgAffinity);

                    if (null == container)
                        throw new Exception("No suitable containers found in subscription. Suitable containers must be named '" +
                            DefaultVhdContainerName + "' and must be in an AG with an InUse VNet");

                    var storageAcct = container.StorageAccount;

                    if (null == storageAcct)
                        throw new Exception(string.Format("No storage account associated with storage container '{0}' in subscription '{1}' in ServiceProviderResourceGroup '{2}'",
                            container.Url, servProdAccount.Name, vmReq.ServiceProviderResourceGroup));

                    //*** Populate placement ***

                    vmCfg.Placement.StorageContainerUrl = container.Url;
                    vmCfg.Placement.AffinityGroup = container.StorageAccount.AffinityGroup;
                    vmCfg.Placement.Location = container.StorageAccount.Location;
                    vmCfg.Placement.VNet = container.StorageAccount.VirtualNetworksAvailable[0].Name;
                    vmCfg.Placement.Subnet = servProdAccount.AzSubnet;
                    vmCfg.Placement.DiskCount = diskCount;

                    vmReq.Config = vmCfg.Serialize();


                    var cmp = new CmpService(EventLog, CmpDbConnectionString, AftsDbConnectionString);
                    cmp.InsertAftsRequest(vmReq, container.Url, storageAcct.PrimaryAccessKey,
                        aftsDeleteSourceAfterTransfer, aftsOverwriteDestinationBlob);

                    vmReq.CurrentStateStartTime = DateTime.UtcNow;
                    vmReq.ExceptionMessage = "";
                    vmReq.ExceptionTypeCode = "";
                    vmReq.StatusCode = Constants.StatusEnum.ReadyForTransfer.ToString();
                    vmReq.StatusMessage = "Ready for transfer to Azure storage";
                    vmReq.ServiceProviderAccountID = servProdAccount.ID;

                    so.ReserveContainerSpace(container.Url, diskCount, vmReq.ID, vmReq);

                    cdb.SetVmDepRequestStatus(vmReq, null);
                }
                catch (Exception ex)
                {
                    vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                    vmReq.ExceptionMessage = "Exception in ProcessorVm.ProcessConvertedSubmissions() : " +
                        Utilities.UnwindExceptionMessages(ex);
                    vmReq.StatusMessage = vmReq.ExceptionMessage;
                    Utilities.SetVmReqExceptionType(vmReq,
                        Constants.RequestExceptionTypeCodeEnum.Admin);
                    cdb.SetVmDepRequestStatus(vmReq, null);
                }
            }

            return 0;
        }
    }
}