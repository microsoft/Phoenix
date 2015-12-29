using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AzureAdminClientLib;
using CmpInterfaceModel;
using CmpInterfaceModel.Models;
using CmpServiceLib.Models;
using VmDeploymentRequest = CmpServiceLib.Models.VmDeploymentRequest;

namespace CmpServiceLib.Stages
{
    public class QcVmRequestPassedStage : Stage
    {
        public Func<string, string> BuildAzureHsRequestBody { get; set; }

        #region CheckServiceNameAvailability

        public delegate HostedServiceOps.ServiceAvailabilityEnum CheckServiceNameAvailabilityDelegate(
            IEnumerable<ServiceProviderAccount> servProdAcctList, string hsName, string affinityGroupName,
            string location, int diskCount, out ServiceProviderAccount useThisServProdAcct,
            out string useThisServiceName, out string useThisAffinityGroupName);

        public CheckServiceNameAvailabilityDelegate CheckServiceNameAvailability { get; set; }

        #endregion

        public Func<VmDeploymentRequest, string> GetTargetServiceProviderAccountResourceGroup { get; set; }

        public int HostedServiceCreationDwellTime { get; set; }

        public int PlacementBusyDwelltime { get; set; }

        public int PlacementDwelltime { get; set; }

        public Func<VmDeploymentRequest, string, Connection, ServiceProviderAccount, HttpResponse> SetPlacement { get; set; }

        public override object Execute()
        {
            if (AllQueuesBlocked)
                return 0;

            //*** leave if Azure containers are not yet synchronized
            if (!ContainersSynchronized)
                return 0;

            var haltSequence = false;
            var appIdList = new HashSet<string>();

            var cdb = new CmpDb(CmpDbConnectionString);
            List<Models.VmDeploymentRequest> vmReqList = null;

            try
            {
                vmReqList = cdb.FetchVmDepRequests(
                    Constants.StatusEnum.QcVmRequestPassed.ToString(), true);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ProcessQcVmRequestPassed() " +
                    CmpCommon.Utilities.UnwindExceptionMessages(ex));
            }

            foreach (var vmReq in vmReqList)
            {
                try
                {
                    var vmCfg = CmpInterfaceModel.Models.VmConfig.Deserialize(vmReq.Config);

                    //*** Which Azure API are we looking at here? ***
                    if (null == vmCfg.AzureApiConfig)
                        vmCfg.AzureApiConfig = null == vmCfg.AzureArmConfig
                            ? new AzureApiSpec() { Platform = Constants.AzureApiType.RDFE.ToString() }
                            : new AzureApiSpec() { Platform = Constants.AzureApiType.ARM.ToString() };

                    //*** Don't try ops on the same service twice in the same run ***
                    if (vmCfg.AzureApiConfig.Platform.Equals(Constants.AzureApiType.RDFE.ToString()))
                    {
                        if (appIdList.Contains(vmReq.ParentAppID))
                            continue;

                        appIdList.Add(vmReq.ParentAppID);
                    }

                    //*** Get the target service provider account resource group ***
                    vmReq.ServiceProviderResourceGroup = GetTargetServiceProviderAccountResourceGroup(vmReq);

                    //*** Get the list of service provider accounts associated with the resource group
                    var servProvAcctList = ServProvAccount.GetAzureServiceAccountList(
                        vmReq.ServiceProviderResourceGroup, CmpDbConnectionString);

                    //*** Were any SPs found? ***
                    if (0 == servProvAcctList.Count)
                        throw new Exception(string.Format("No Service provider Account found with group name '{0}'", 
                            vmReq.ServiceProviderResourceGroup));

                    //*** Get the target service provider account ID ***
                    //vmReq.ServiceProviderAccountID = GetTargetServiceProviderAccountID(vmReq);

                    //*** Get account info ***
                    //if (null == vmReq.ServiceProviderAccountID)
                    //    throw new Exception("ServiceProviderAccountID == NULL");

                    //AzureAdminClientLib.Connection connection =
                    //    ServProvAccount.GetAzureServiceAccountConnection((int)vmReq.ServiceProviderAccountID, _CmpDbConnectionString);

                    var hsBody = BuildAzureHsRequestBody(vmReq.Config);

                    if (null == hsBody)
                    {
                        vmReq.StatusCode = Constants.StatusEnum.ReadyForUploadingServiceCert.ToString();
                        vmReq.ExceptionMessage = "No Service Specified";
                        vmReq.ServiceProviderStatusCheckTag = "";
                    }
                    else
                    {
                        AzureAdminClientLib.HttpResponse resp = null;
                        Models.ServiceProviderAccount servProdAccount = null;
                        AzureAdminClientLib.Connection connection = null;
                        string useThisServiceName = null;
                        string useThisAffinityGroupName = null;

                        //*** Count disks ***

                        var diskCount = 1;

                        if (null != vmCfg.DiskSpecList)
                            diskCount += vmCfg.DiskSpecList.Count;

                        //*** Do we need to create the hosted service? ***

                        var serviceName = Utilities.GetXmlInnerText(hsBody, "ServiceName");

                        if (null == serviceName)
                            throw new Exception("ServiceName = NULL");

                        if (0 == serviceName.Length)
                            throw new Exception("ServiceName.Length = 0");

                        var avail = HostedServiceOps.ServiceAvailabilityEnum.Unknown;

                        //* Pivot on API here. If RDFE, look for the host service. If ARM, create the 
                        //* service (or find if alreday exists) and say we already own it.

                        if (vmCfg.AzureApiConfig.Platform.Equals(Constants.AzureApiType.RDFE.ToString()))
                        {
                            avail = CheckServiceNameAvailability(servProvAcctList,
                                serviceName, null, vmCfg.HostedServiceConfig.Location,
                                diskCount, out servProdAccount, out useThisServiceName,
                                out useThisAffinityGroupName);
                        }
                        else
                        {
                            //*** TODO * Use placement, just take the first one for now
                            servProdAccount = servProvAcctList[0];
                            vmReq.ServiceProviderAccountID = servProdAccount.ID;

                            connection = ServProvAccount.GetAzureServiceAccountConnection(
                                servProdAccount.AccountID, servProdAccount.CertificateThumbprint,
                                servProdAccount.AzureADTenantId, servProdAccount.AzureADClientId, servProdAccount.AzureADClientKey);

                            var hso = new AzureAdminClientLib.HostedServiceOps(connection);
                            resp = hso.CreateResourceGroup(serviceName, 
                                vmCfg.AzureArmConfig.properties.template.variables.location, false);

                            avail = HostedServiceOps.ServiceAvailabilityEnum.AlredayOwnIt;
                            useThisServiceName = serviceName;
                            useThisAffinityGroupName = null;
                        }

                        vmReq.TargetServicename = useThisServiceName;
                        vmReq.Config = vmReq.Config.Replace("<ServiceName>" + serviceName + "</ServiceName>",
                            "<ServiceName>" + useThisServiceName + "</ServiceName>");

                        switch (avail)
                        {
                            case AzureAdminClientLib.HostedServiceOps.ServiceAvailabilityEnum.Unavailable:
                                resp = new AzureAdminClientLib.HttpResponse { HadError = true, Body = "Service Name Not Available", HTTP = "" };
                                break;

                            case AzureAdminClientLib.HostedServiceOps.ServiceAvailabilityEnum.AlredayOwnIt:

                                //*** Iffn it's ARM, we don't be needn to set placement
                                if (vmCfg.AzureApiConfig.Platform.Equals(Constants.AzureApiType.ARM.ToString()))
                                    break;

                                connection = ServProvAccount.GetAzureServiceAccountConnection(
                                    servProdAccount.AccountID, servProdAccount.CertificateThumbprint,
                                    servProdAccount.AzureADTenantId, servProdAccount.AzureADClientId, servProdAccount.AzureADClientKey);

                                resp = SetPlacement(vmReq, useThisAffinityGroupName, connection, servProdAccount);

                                if (resp.HadError)
                                {
                                    if (resp.Retry)
                                    {
                                        Thread.Sleep(PlacementBusyDwelltime);
                                        continue;
                                    }

                                    throw new Exception("Unable to find/create placement : " + resp.Body);
                                }

                                resp = new AzureAdminClientLib.HttpResponse
                                {
                                    HadError = false, Body = "Already Own Service Name", StatusCheckUrl = ""
                                };

                                break;

                            case AzureAdminClientLib.HostedServiceOps.ServiceAvailabilityEnum.Available:

                                // Go through each of the service provider accounts (a.k.a. subscriptions) and
                                // populate the Azure properties. Currently this is just the core counts.
                                // This needs to be done before calling the Min() function, otherwise all the values
                                // will be zero and the comparison won't be of much value.

                                if (null == servProdAccount)
                                {
                                    foreach (var oneProviderAccount in servProvAcctList)
                                        oneProviderAccount.LoadAzureProperties();

                                    // Get the provider account with the maximum percentage of available cores
                                    servProdAccount = servProvAcctList.Max();
                                }

                                connection = ServProvAccount.GetAzureServiceAccountConnection(
                                    servProdAccount.AccountID, servProdAccount.CertificateThumbprint,
                                    servProdAccount.AzureADTenantId, servProdAccount.AzureADClientId, servProdAccount.AzureADClientKey);

                                var hso = new AzureAdminClientLib.HostedServiceOps(connection);

                                resp = SetPlacement(vmReq, null, connection, servProdAccount);

                                if (resp.HadError)
                                {
                                    if (resp.Retry)
                                    {
                                        Thread.Sleep(PlacementBusyDwelltime);
                                        continue;
                                    }

                                    throw new Exception("Unable to find/create placement : " + resp.Body);
                                }

                                hsBody = BuildAzureHsRequestBody(vmReq.Config);
                                resp = hso.CreateHostedService(hsBody);
                                break;
                        }

                        if (resp.HadError)
                        {
                            if (resp.HTTP.Contains("409"))
                            {
                                vmReq.StatusCode = Constants.StatusEnum.ReadyForUploadingServiceCert.ToString();
                                vmReq.ExceptionMessage = "";
                                vmReq.StatusMessage = "Service already exists";
                            }
                            else
                            {
                                haltSequence = true;
                                vmReq.ExceptionMessage = resp.Body;
                                vmReq.CurrentStateStartTime = DateTime.UtcNow;

                                if (avail == HostedServiceOps.ServiceAvailabilityEnum.Unavailable)
                                {
                                    vmReq.StatusMessage = "Rejected";
                                    vmReq.StatusCode = Constants.StatusEnum.Rejected.ToString();
                                    Utilities.SetVmReqExceptionType(vmReq,
                                        CmpInterfaceModel.Constants.RequestExceptionTypeCodeEnum.Customer);
                                }
                                else
                                {
                                    vmReq.StatusMessage = "Exception";
                                    vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                                    Utilities.SetVmReqExceptionType(vmReq,
                                        CmpInterfaceModel.Constants.RequestExceptionTypeCodeEnum.Admin);
                                }
                            }
                        }
                        else
                        {
                            vmReq.StatusCode = Constants.StatusEnum.ReadyForUploadingServiceCert.ToString();
                            vmReq.ExceptionMessage = "";
                            vmReq.StatusMessage = resp.Body;
                            vmReq.ServiceProviderStatusCheckTag = resp.StatusCheckUrl;
                            System.Threading.Thread.Sleep(HostedServiceCreationDwellTime);
                        }
                    }

                    if (!haltSequence)
                    {
                        vmReq.CurrentStateStartTime = DateTime.UtcNow;
                    }

                    cdb.SetVmDepRequestStatus(vmReq, null);

                    Thread.Sleep(PlacementDwelltime);
                }
                catch (Exception ex)
                {
                    if (null != vmReq)
                    {
                        if (ex.Message.Contains("(503)"))
                            continue;
                        else
                        {
                            vmReq.StatusCode = Constants.StatusEnum.Exception.ToString();
                            vmReq.ExceptionMessage = "Exception in ProcessQcVmRequestPassed() : " +
                                                     Utilities.UnwindExceptionMessages(ex);
                            vmReq.StatusMessage = "Exception";
                            vmReq.CurrentStateStartTime = DateTime.UtcNow;
                            Utilities.SetVmReqExceptionType(vmReq,
                                CmpInterfaceModel.Constants.RequestExceptionTypeCodeEnum.Admin);
                            cdb.SetVmDepRequestStatus(vmReq, null);
                            Thread.Sleep(PlacementBusyDwelltime);
                        }
                    }
                }
            }

            return 0;
        }
    }
}