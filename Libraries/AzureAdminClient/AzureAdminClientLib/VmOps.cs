//*****************************************************************************
//
// File:
// Author: Mark west (mark.west@microsoft.com)
//Change history
// Bug 5508: 4/2/2014 meragh
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using CmpInterfaceModel;
using CmpInterfaceModel.Models;
using Microsoft.Azure;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Management.Compute;
using Microsoft.WindowsAzure.Management.Compute.Models;
using Microsoft.WindowsAzure.Management;
using DataVirtualHardDisk = Microsoft.WindowsAzure.Management.Compute.Models.DataVirtualHardDisk;
using DeploymentGetResponse = Microsoft.WindowsAzure.Management.Compute.Models.DeploymentGetResponse;
using OperationStatusResponse = Microsoft.Azure.OperationStatusResponse;
using ResourceExtensionParameterValue = Microsoft.WindowsAzure.Management.Compute.Models.ResourceExtensionParameterValue;
using ResourceExtensionReference = Microsoft.WindowsAzure.Management.Compute.Models.ResourceExtensionReference;
using Role = Microsoft.WindowsAzure.Management.Compute.Models.Role;

//using CmpInterfaceModel.Models;

namespace AzureAdminClientLib
{
    //*************************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*************************************************************************

    public class VmInfo
    {
        public string ServiceName;
        public string Name;
        public string DeploymentSlot;
        public string LabelB64;
        public string RoleName;
        public string ComputerName;
        public string AdminPassword;
        public string LocalPort;
        public string MediaLink;
        public string SourceImageName;
        public List<VhdInfo> DataDiskList;
        public HostedServiceInfo ServiceInfo = null;


        public bool CreateHostedService = false;

        public string LabelText
        {
            set { LabelB64 = Util.ToB64(value); }
            get { return Util.FromB64(LabelB64); }
        }
    }

    /*public class Vhdinfoy
    {
        public string Name;
    }*/

    //*************************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*************************************************************************

    public class VmOps
    {
        //const string URLTEMPLATE_STOPVM_ARM = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Compute/virtualMachines/{2}/stop?api-version={3}";

        const string URLTEMPLATE_DEALLOCATEVM_ARM = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Compute/virtualMachines/{2}/deallocate?api-version={3}";
        const string URLTEMPLATE_STARTVM_ARM = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Compute/virtualMachines/{2}/stART?api-version={3}";
        const string URLTEMPLATE_RESTARTVM_ARM = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Compute/virtualMachines/{2}/restart?api-version={3}";
        const string URLTEMPLATE_DELETEVM_ARM = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Compute/virtualMachines/{2}?api-version={3}";
        const string APIVERSION_VMOPS = "2015-06-15";

        const string URLTEMPLATE_GETOSIMAGES = "https://management.core.windows.net/{0}/services/images";
        const string URLTEMPLATE_CREATEVM = "https://management.core.windows.net/{0}/services/hostedservices/{1}/deployments";
        const string URLTEMPLATE_ADDVM = "https://management.core.windows.net/{0}/services/hostedservices/{1}/deployments/{2}/roles";

        const string URLTEMPLATE_FETCHVMINFO_ARM = @"https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Compute/virtualMachines/{2}?api-version={3}";
        const string URLTEMPLATE_FETCHVMINFOINSTANCEVIEW_ARM = @"https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Compute/virtualMachines/{2}/InstanceView?api-version={3}";
        const string URLTEMPLATE_ADDVM_ARM = "https://{0}/subscriptions/{1}/resourcegroups/{2}/providers/microsoft.resources/deployments/{3}?api-version={4}";

        const string AZURECREATEVMBODYSHELL = @"<?xml version=""1.0"" encoding=""utf-8""?><Deployment xmlns=""http://schemas.microsoft.com/windowsazure"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">{0}</Deployment>";
        const string AZUREADDVMBODYSHELL = @"<?xml version=""1.0"" encoding=""utf-8""?><PersistentVMRole xmlns=""http://schemas.microsoft.com/windowsazure"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">{0}</PersistentVMRole>";
        const string AZUREHSBODYSHELL = @"<?xml version='1.0' encoding='utf-8'?><CreateHostedService xmlns='http://schemas.microsoft.com/windowsazure'>{0}</CreateHostedService>";

        const string BODYTEMPLATE_CREATEVM =
          @"<?xml version=""1.0"" encoding=""utf-8""?><Deployment xmlns=""http://schemas.microsoft.com/windowsazure"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
          <Name>{Name}</Name>
          <DeploymentSlot>{DeploymentSlot}</DeploymentSlot>
          <Label>{Label}</Label>
          <RoleList>
            <Role i:type=""PersistentVMRole"">
              <RoleName>{RoleName}</RoleName>
              <OsVersion i:nil=""true""/>
              <RoleType>PersistentVMRole</RoleType>
              <ConfigurationSets>
                <ConfigurationSet i:type=""WindowsProvisioningConfigurationSet"">
                  <ConfigurationSetType>WindowsProvisioningConfiguration</ConfigurationSetType>
                  <ComputerName>{ComputerName}</ComputerName>
                  <AdminPassword>{AdminPassword}</AdminPassword>
                  <EnableAutomaticUpdates>true</EnableAutomaticUpdates>
                  <ResetPasswordOnFirstLogon>false</ResetPasswordOnFirstLogon>
                </ConfigurationSet>
                <ConfigurationSet i:type=""NetworkConfigurationSet"">
                  <ConfigurationSetType>NetworkConfiguration</ConfigurationSetType>
                  <InputEndpoints>
                    <InputEndpoint>
                      <LocalPort>{LocalPort}</LocalPort>
                      <Name>RemoteDesktop</Name>
                      <Protocol>tcp</Protocol>
                    </InputEndpoint>
                  </InputEndpoints>
                </ConfigurationSet>
              </ConfigurationSets>
              <DataVirtualHardDisks/>
              <Label></Label>
              <OSVirtualHardDisk>
                <MediaLink>{MediaLink}</MediaLink>
                <SourceImageName>{SourceImageName}</SourceImageName>
              </OSVirtualHardDisk>
            </Role>
          </RoleList>
        </Deployment>";

        const string BODYTEMPLATE_DATAVHD =
            @"<DataVirtualHardDisk>
             <Lun>{Lun}</Lun>
             <DiskLabel>{DiskLabel}</DiskLabel>
             <LogicalDiskSizeInGB>{LogicalDiskSizeInGB}</LogicalDiskSizeInGB>
             <MediaLink>{MediaLink}</MediaLink>
             </DataVirtualHardDisk>";

        const int HostedServiceCreationDwellTime = 2000;

        public IComputeManagementClient Client { get; private set; }
        public IConnection Connection { get; private set; }
        public IHostedServiceOps HostedOps { get; private set; }
        public IHttpInterface Interface { get; private set; }
        public IManagementClient ManagementClient { get; private set; }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// 
        //*********************************************************************

        /*public VmOps(IConnection connection)
        {
            Client = new ComputeManagementClient(connection.CertCloudCreds);
            Connection = connection;
            HostedOps = new HostedServiceOps(connection);
            Interface = new HttpInterface(connection);
            ManagementClient = new ManagementClient(connection.CertCloudCreds);
        }*/

        public VmOps(IConnection connection)
        {
            if (null != connection.CertCloudCreds)
            {
                Client = new ComputeManagementClient(connection.CertCloudCreds);
                ManagementClient = new ManagementClient(connection.CertCloudCreds);
            }

            Connection = connection;
            HostedOps = new HostedServiceOps(connection);
            Interface = new HttpInterface(connection);
        }

        public VmOps(IComputeManagementClient client, IConnection connection, IHostedServiceOps hostedServiceOps,
            IHttpInterface httpInterface, IManagementClient managementClient)
        {
            Client = client;
            Connection = connection;
            HostedOps = hostedServiceOps;
            Interface = httpInterface;
            ManagementClient = managementClient;
        }

        #region *** Not ARM ***************************************************

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureAdminClientLib.HttpResponse GetOsImageList()
        {
            var url = string.Format(URLTEMPLATE_GETOSIMAGES, Connection.SubcriptionID);
            return Interface.PerformRequest(HttpInterface.RequestType_Enum.GET, url);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Returns a list of detached disks for the current subscription
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public IEnumerable<VhdInfo> GetDetachedDisks()
        {
            try
            {
                /* TODO: joaldaba. Short circuit to see if VM dashboard works*/
                if (Client == null)
                {
                    List<VhdInfo> dummy = new List<VhdInfo>();
                    //dummy.Add(new VhdInfo {DiskName = "dummy"});
                    return dummy;
                }
                if (Client.VirtualMachineDisks == null)
                {
                    List<VhdInfo> dummy = new List<VhdInfo>();
                    //dummy.Add(new VhdInfo {DiskName = "dummy"});
                    return dummy;
                }

                var disks = Client.VirtualMachineDisks.ListDisks();
                return disks.Where(d => d.UsageDetails == null).Select(d => new VhdInfo
                    {
                        DiskName = d.Name,
                    });
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmOps.GetDetachedDisks() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmInfo"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureAdminClientLib.HttpResponse CreateVm(VmInfo vmInfo)
        {
            //***

            if (vmInfo.CreateHostedService)
            {
                var resp = HostedOps.CreateHostedService(vmInfo.ServiceInfo);
            }

            //***

            var url = string.Format(URLTEMPLATE_CREATEVM, Connection.SubcriptionID, vmInfo.ServiceName);
            var body = string.Copy(BODYTEMPLATE_CREATEVM);

            body = body.Replace("{Name}", vmInfo.Name);
            body = body.Replace("{DeploymentSlot}", vmInfo.DeploymentSlot);
            body = body.Replace("{Label}", vmInfo.LabelB64);
            body = body.Replace("{RoleName}", vmInfo.RoleName);
            body = body.Replace("{ComputerName}", vmInfo.ComputerName);
            body = body.Replace("{AdminPassword}", vmInfo.AdminPassword);
            body = body.Replace("{LocalPort}", vmInfo.LocalPort);
            body = body.Replace("{MediaLink}", vmInfo.MediaLink);
            body = body.Replace("{SourceImageName}", vmInfo.SourceImageName);

            if (null != vmInfo.DataDiskList)
                if (0 != vmInfo.DataDiskList.Count)
                {
                    var dataVhdBody = "<DataVirtualHardDisks>";

                    foreach (var vhd in vmInfo.DataDiskList)
                    {
                        var dataVhdBodyInner = string.Copy(BODYTEMPLATE_DATAVHD);

                        dataVhdBodyInner = dataVhdBodyInner.Replace("{Lun}", vhd.Lun.ToString());
                        dataVhdBodyInner = dataVhdBodyInner.Replace("{DiskLabel}", vhd.DiskLabel);
                        dataVhdBodyInner = dataVhdBodyInner.Replace("{LogicalDiskSizeInGB}", vhd.LogicalDiskSizeInGB.ToString());
                        dataVhdBodyInner = dataVhdBodyInner.Replace("{MediaLink}", vhd.MediaLink);

                        dataVhdBody += dataVhdBodyInner;
                    }

                    dataVhdBody += "</DataVirtualHardDisks>";
                    body = body.Replace("<DataVirtualHardDisks/>", dataVhdBody);
                }

            return Interface.PerformRequest(HttpInterface.RequestType_Enum.POST, url, body);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string BuildAzureDeployVmRequestBody(string requestBody)
        {
            var innerText = Utilities.GetXmlInnerText(requestBody, "AzureVmConfig");

            return string.Format(AZURECREATEVMBODYSHELL, innerText);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string BuildAzureAddVmRequestBody(string requestBody)
        {
            var innerText = Utilities.GetXmlInnerText(requestBody, "Role");

            return string.Format(AZUREADDVMBODYSHELL, innerText);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string BuildAzureHsRequestBody(string requestBody)
        {
            var innerText = Utilities.GetXmlInnerText(requestBody, "HostedServiceConfig");

            if (null == requestBody)
                return null;

            if (0 == requestBody.Length)
                return null;

            return string.Format(AZUREHSBODYSHELL, innerText);
        }

        //*********************************************************************
        ///
        ///  <summary>
        /// 
        ///  </summary>
        ///  <param name="statusCheckUrl"></param>
        /// <param name="vmConfig"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************
        public HttpResponse CheckVmProvisioningStatus(string statusCheckUrl, string vmConfig)
        {
            var nullUrl = false;

            if (null == statusCheckUrl)
                nullUrl = true;
            else if (0 == statusCheckUrl.Length)
                nullUrl = true;

            if (nullUrl)
        {
                var ret = new HttpResponse { HadError = true, Body = "statusCheckUrl is null or empty", Retry = false };
                return ret;
            }

            if (null != vmConfig)
            {
                var vmConf = VmConfig.Deserialize(vmConfig);

                if (null != vmConf.AzureArmConfig)
                {
                    var url =
                        string.Format(
                            @"https://management.azure.com/subscriptions/{0}/resourcegroups/{1}/providers/microsoft.resources/deployments/{2}?api-version={3}",
                            Connection.SubcriptionID, vmConf.HostedServiceConfig.ServiceName,
                            vmConf.AzureArmConfig.properties.template.variables.vmName, "2015-01-01");

                    var ret = Interface.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url, null);

                    if (ret.ProviderRequestState.Equals("Failed"))
                    {
                        ret = Interface.PerformRequestArm(HttpInterface.RequestType_Enum.GET, statusCheckUrl, null);

                        if (ret.HadError)
                            if (ret.Body.ToLower().Contains("query parameter value is invalid"))
                                ret.Retry = true;

                        /*Web Error making REST API call.
                        Message: The remote server returned an error: (400) Bad Request.
                        Response:
                        { "Code":"BadRequest","Message":"The $filter query parameter value is invalid."}*/
                    }

                    return ret;
                }
            }

            return Interface.PerformRequest(HttpInterface.RequestType_Enum.GET, statusCheckUrl, null);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmDepReq"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public HttpResponse CreateVm(VmDeploymentRequest vmDepReq)
        {
            string url = null;
            string body = null;
            HttpResponse Resp = null;

            //*** Which Azure API? ***
            var vmc = VmConfig.Deserialize(vmDepReq.Config);

            if (null == vmc.AzureArmConfig) //*** It's RDFE ***
            {
                //*** Check hosted service for existence of deployment ***

                List<string> deploynmentNameList;
                Resp = HostedOps.GetServiceDeploymentList(vmDepReq.TargetServiceName, "Production",
                    out deploynmentNameList);

                if (null == deploynmentNameList)
                {
                        url = string.Format(URLTEMPLATE_CREATEVM,
                            Connection.SubcriptionID, vmDepReq.TargetServiceName);
                    body = BuildAzureDeployVmRequestBody(vmDepReq.Config);
                }
                else
                {
                        url = string.Format(URLTEMPLATE_ADDVM,
                            Connection.SubcriptionID, vmDepReq.TargetServiceName,
                            deploynmentNameList[0]);
                    body = BuildAzureAddVmRequestBody(vmDepReq.Config);
                }

                Resp = Interface.PerformRequest(HttpInterface.RequestType_Enum.POST, url, body);
            }
            else //*** Its ARM ***
            {
                ReferenceExistingArmResources(vmc);

                url = string.Format(URLTEMPLATE_ADDVM_ARM,
                    Constants.ARMMANAGEMENTADDRESS, Connection.SubcriptionID, vmDepReq.TargetServiceName,
                    vmDepReq.TargetVmName, Constants.AZUREAPIVERSION);

                body = vmc.AzureArmConfig.SerializeJson();
                Resp = Interface.PerformRequestArm(HttpInterface.RequestType_Enum.PUT, url, body);
            }

            return Resp;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private DeploymentGetResponse GetAzureDeyployment(string serviceName, DeploymentSlot slot)
        {
            var client = new ComputeManagementClient(Connection.CertCloudCreds);
            try
            {
                return Client.Deployments.GetBySlot(serviceName, slot);
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("ResourceNotFound"))
                {
                    return null;
                }
                
                throw;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        //*** NOTE * Compute
        //*** NOTE * Network

        public List<Role> FetchVmList()
        {
            if(null != Connection.AdToken)
                return FetchVmListArm();

            try
            {
                var hostedServices = Client.HostedServices.List();
                var vmList = new List<Role>();

                foreach (var service in hostedServices)
                {
                    var deployment = GetAzureDeyployment(service.ServiceName, DeploymentSlot.Production);
                    if (deployment != null)
                    {
                        if (deployment.Roles.Count > 0)
                        {
                            foreach (var role in deployment.Roles)
                            {
                                if (role.RoleType == VirtualMachineRoleType.PersistentVMRole.ToString())
                                {
                                    role.Label = service.ServiceName;
                                    vmList.Add(role);
                                }
                            }
                        }
                    }
                }

                return vmList;
            }
            catch (Exception ex)
            {
                var M = ex.Message;
                throw;
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmName"></param>
        /// <param name="hostServiceName"></param>
        /// <param name="netServiceName"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        public int FetchServicePort(string vmName, string hostServiceName, string netServiceName)
        {
            try
            {
                HostedServiceListResponse.HostedService service;
                DeploymentGetResponse deployment;
                string ipPriv;

                var vm = FetchVm(vmName, hostServiceName, out service, out deployment, out ipPriv);

                if (null == vm)
                    throw new Exception("Unable to locate VM '" + vmName + "'");

                if (null == vm.ConfigurationSets)
                    throw new Exception("No configuration sets found for VM '" + vmName + "'");

                foreach (var cs in vm.ConfigurationSets)
                {
                    if (null == cs)
                        continue;

                    if (null == cs.InputEndpoints)
                        continue;

                    foreach (var ie in cs.InputEndpoints)
                        if (ie.Name.Equals(netServiceName, StringComparison.InvariantCultureIgnoreCase))
                            if (null == ie.Port)
                                return -1;
                            else
                                return (int)ie.Port;
                }

                return -1;
            }
            catch (Exception ex)
            {
                var M = ex.Message;
                throw;
            }
        }

        //*********************************************************************
        ///
        ///  <summary>
        /// 
        ///  </summary>
        ///  <param name="vmName"></param>
        /// <param name="serviceName"></param>
        /// <param name="service"></param>
        ///  <param name="deployment"></param>
        /// <param name="ipPriv"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        //*** NOTE * Compute

        Role FetchVm(string vmName, string serviceName,
            out HostedServiceListResponse.HostedService service, out DeploymentGetResponse deployment, out string ipPriv)
        {
            try
            {
                ipPriv = null;
                service = null;
                deployment = null;

                var hostedServices = Client.HostedServices.List();
                
                foreach (var service2 in hostedServices)
                {
                    if (null != serviceName)
                        if (!serviceName.Equals(service2.ServiceName, StringComparison.InvariantCultureIgnoreCase))
                            continue;

                    deployment = GetAzureDeyployment(service2.ServiceName, DeploymentSlot.Production);
                    if (deployment == null) continue;
                    if (deployment.Roles.Count <= 0) continue;

                    foreach (var role in deployment.Roles.Where(role => role.RoleType == VirtualMachineRoleType.PersistentVMRole.ToString()).Where(role => vmName.ToLower().Equals(role.RoleName.ToLower())))
                    {
                        service = service2;

                        if (null != deployment.RoleInstances)
                            foreach (var inst in deployment.RoleInstances)
                                if (null != inst.HostName)
                                    if (inst.HostName.Equals(vmName, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        ipPriv = inst.IPAddress;
                                        break;
                                    }

                        return role;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVm() : " + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        VirtualMachineUpdateParameters RoleToVmUpdateParams(Role role)
        {
            var parameters = new VirtualMachineUpdateParameters
            {
                AvailabilitySetName = role.AvailabilitySetName,
                DataVirtualHardDisks = role.DataVirtualHardDisks,
                Label = role.Label,
                OSVirtualHardDisk = role.OSVirtualHardDisk,
                ProvisionGuestAgent = role.ProvisionGuestAgent,
                ResourceExtensionReferences = role.ResourceExtensionReferences,
                RoleName = role.RoleName,
                RoleSize = role.RoleSize,
                ConfigurationSets = role.ConfigurationSets
            };

            if (null == parameters.ProvisionGuestAgent)
                parameters.ProvisionGuestAgent = false;

            if (!(bool)parameters.ProvisionGuestAgent)
            {
                parameters.ProvisionGuestAgent = true;
                parameters.ResourceExtensionReferences.Add(new ResourceExtensionReference()
                {
                    Name = "BGInfo",
                    Publisher = "Microsoft.Compute",
                    ReferenceName = "BGInfo",
                    ResourceExtensionParameterValues = new List<ResourceExtensionParameterValue>(),
                    State = "Enable",
                    Version = "1.*"
                });
            }

            return parameters;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="deploymentName"></param>
        /// <param name="virtualMachineName"></param>
        /// <param name="parameters"></param>
        /// 
        // http://msdn.microsoft.com/en-us/library/microsoft.windowsazure.management.compute.ivirtualmachineoperations.updateasync(v=azure.11).aspx
        // http://msdn.microsoft.com/en-us/library/microsoft.windowsazure.management.compute.models.virtualmachineupdateparameters_members(v=azure.11).aspx
        // http://msdn.microsoft.com/en-us/library/microsoft.windowsazure.management.compute.models.configurationset_members(v=azure.11).aspx
        //
        //*********************************************************************

        string UpdateVm(string serviceName, string deploymentName, string virtualMachineName,
            VirtualMachineUpdateParameters parameters)
        {
            try
            {
                var resp = Client.VirtualMachines.Update(serviceName,
                    deploymentName, virtualMachineName, parameters);
                
                if (resp.HttpStatusCode != HttpStatusCode.OK)
                    throw new Exception("HTTP communication Exception");


                if (resp.Status != OperationStatus.Succeeded)
                    throw new Exception(resp.Error.Message);

                return resp.RequestId;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmOps.UpdateVm() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmName"></param>
        /// 
        //*********************************************************************

        public static void DisableSmartCardAuth(string vmName)
        {
            // Invoke-WmiMethod -Class Win32_Process -Name Create -ComputerName $ComputerName 
            // -ArgumentList "reg.exe add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\policies\system 
            // /v scforceoption /t REG_DWORD /d 0 /f"

            RegistryHelper.SetRemoteRegistryDword(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\policies\system",
                "scforceoption", 0, vmName);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualMachineName"></param>
        /// <param name="hostServiceName"></param>
        /// Bug 5508: change made to look up azure and see if the ip azure assigned
        /// during deployment is available or pick anyfree ip and assign it 
        /// on failure will do the retry and pick new ip else will assign the 
        /// ip given by azure as static
        //*********************************************************************

        public string MakeIpStatic(string virtualMachineName, string hostServiceName)
        {
            var retryCount = 1;
            while (true)
            {

                for (var index = 0; ; index++)
                {
                try
                {
                    HostedServiceListResponse.HostedService service;
                    DeploymentGetResponse deployment;
                    string ipPriv;

                    var vm = FetchVm(virtualMachineName, hostServiceName,
                        out service, out deployment, out ipPriv);

                    if (null == vm)
                        throw new Exception("Unable to locate VM '" +
                            virtualMachineName + "'");
                   
              
                    var parameters = RoleToVmUpdateParams(vm);
                    if (retryCount >= 2)
                    { 
                        var vNetName = deployment.VirtualNetworkName;
                        var subnetName = parameters.ConfigurationSets[0].SubnetNames.FirstOrDefault();
                        var vNetInfo = new AzureAdminClientLib.VnetOps(Connection);
                        if (null == vNetInfo)
                            throw new Exception("Unable to get Vnet information'" +
                                virtualMachineName + "'");

                        var addressList = vNetInfo.GetAvailAddrList(vNetName, subnetName, "0");
                        if (addressList.Count == 0)
                            throw new Exception("NO available Ip address for this Vnet'" +
                              vNetName + "'");
                        if (!addressList.Contains(ipPriv))

                            ipPriv = addressList[0];

                    }
                    parameters.ConfigurationSets[0].StaticVirtualNetworkIPAddress = ipPriv;
                   
                    UpdateVm(service.ServiceName, deployment.Name,
                        virtualMachineName, parameters);

                    return ipPriv;
                }
            
                catch (Exception ex)
                {
                     if (ex.Message.Contains("Unable to allocate") & (2 > retryCount++))
                            Thread.Sleep((int)(1000 * Math.Pow(4.17, retryCount)));
                       
                        else
                            if (ex.Message.Contains("(503)"))
                        Thread.Sleep(11000);   
                            else
                    
                    if (index > 0)
                        throw new Exception("Exception in VmOps.MakeIpStatic() : " +
                    Utilities.UnwindExceptionMessages(ex));
                    break;
                
                }
            }
        }
        }

     
        
        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmName"></param>
        /// <param name="deploymentName"></param>
        /// <param name="hostserviceName"></param>
        /// 
        //*********************************************************************

        public void Restart(string vmName, string deploymentName, string hostserviceName)
        {
            try
            {
                Client.VirtualMachines.Restart(hostserviceName, deploymentName, vmName);
            }
            catch (Exception ex)
            {
                var M = ex.Message;
                throw;
            }
        }

        //*********************************************************************
        ///
        ///  <summary>
        /// 
        ///  </summary>
        ///  <param name="vmName"></param>
        /// <param name="serviceName"></param>
        /// 
        //*********************************************************************

        public HttpResponse Restart(string vmName, string serviceName)
        {
            try
            {
                if (IsArm)
                    return RestartVmArm(vmName, serviceName);

                HostedServiceListResponse.HostedService service;
                DeploymentGetResponse deployment;
                string ipPriv;

                if (null == FetchVm(vmName, serviceName, out service, out deployment, out ipPriv))
                    throw new Exception("Unable to locate VM '" + vmName + "'");

                // client.ro
                var resp = Client.VirtualMachines.Restart(service.ServiceName, deployment.Name, vmName);

                return new HttpResponse()
                {
                    HadError = false,
                    Body = "",
                    StatusCheckUrl = string.Format(URLTEMPLATE_FETCHVMINFOINSTANCEVIEW_ARM, Connection.SubcriptionID, serviceName, vmName, "2015-06-15")
                };
            }
            catch (Exception ex)
            {
                var M = ex.Message;
                throw;
            }
        }

        //*********************************************************************
        ///
        ///  <summary>
        /// 
        ///  </summary>
        ///  <param name="vmName"></param>
        /// <param name="serviceName"></param>
        /// 
        //*********************************************************************

        public VmRole GetVM(string vmName, string serviceName)
        {
            try
            {
                if (IsArm)
                    return GetVmArm(vmName, serviceName);

                HostedServiceListResponse.HostedService service;
                DeploymentGetResponse deploymentresponse;

                string ipPriv;

                var role = FetchVm(vmName, serviceName, out service, out deploymentresponse, out ipPriv);
                if (role == null)
                    throw new Exception("Unable to locate VM '" + vmName + "'");

                //  deployment = deploymentresponse;


                var DataVirtualHardDisks = role.DataVirtualHardDisks.Select(x => new CmpInterfaceModel.Models.DataVirtualHardDisk
                {
                    DiskName = x.Name,
                    DiskLabel = x.Label,
                    HostCaching = x.HostCaching,
                    LogicalDiskSizeInGB = x.LogicalDiskSizeInGB == null ? null : x.LogicalDiskSizeInGB.ToString(),
                    // Lun = x.LogicalUnitNumber.ToString(),
                    MediaLink = x.MediaLink == null ? null : x.MediaLink.ToString(),
                    //  SourceMediaLink = x.SourceMediaLink.ToString()

                }).ToList();


                var osVirtualDisk = new OsVirtualHardDisk
                {
                    DiskLabel = role.OSVirtualHardDisk.Label,
                    DiskName = role.OSVirtualHardDisk.Name,
                    HostCaching = role.OSVirtualHardDisk.HostCaching,
                    MediaLink = role.OSVirtualHardDisk.MediaLink == null ? null : role.OSVirtualHardDisk.MediaLink.ToString(),
                    OS = role.OSVirtualHardDisk.OperatingSystem,
                    SourceImageName = role.OSVirtualHardDisk.SourceImageName
                };

                var deployment = new Deployment
                {
                    PrivateID = deploymentresponse.PrivateId,
                    RoleInstanceList = deploymentresponse.RoleInstances.Where
                                       (x => x.InstanceName == vmName).Select(x => new DeploymentRoleInstance
                                       {
                                           IpAddress = x.IPAddress,
                                           RemoteAccessCertificateThumbprint = x.RemoteAccessCertificateThumbprint,
                                           InstanceName = x.RoleName,
                                           InstanceSize = x.InstanceSize,
                                           InstanceStatus = x.InstanceStatus == "RoleStateUnknown" ? x.PowerState.ToString() : x.InstanceStatus

                                       }).ToArray(),
                    Url = deploymentresponse.Uri.DnsSafeHost.ToString()

                };

                var subscriptionresponse = ManagementClient.Subscriptions.Get();

                var subscription = new SubscriptionInfo
                {
                    CurrentCoreCount = subscriptionresponse.CurrentCoreCount.ToString(),
                    MaximumCoreCount = subscriptionresponse.MaximumCoreCount.ToString(),
                    SubscriptionID = subscriptionresponse.SubscriptionID,
                    SubscriptionName = subscriptionresponse.SubscriptionName
                };

                var vmrole = new VmRole(role.RoleName, role.RoleSize, 
                    DataVirtualHardDisks, osVirtualDisk, deployment, subscription);

                return vmrole;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in AzureAdminClientLib.VmOps.GetVM() " + 
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///   <summary>
        ///  
        ///   </summary>
        ///   <param name="vmName"></param>
        ///  <param name="serviceName"></param>
        ///  <param name="deleteFromStorage"></param>
        /// <param name="throwIfNotFound"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        public HttpResponse Delete(string vmName,
            string serviceName, bool deleteFromStorage, bool throwIfNotFound)
        {
            try
            {
                if (IsArm)
                    return DeleteVmArm(vmName, serviceName, deleteFromStorage, throwIfNotFound);

                HostedServiceListResponse.HostedService service;
                DeploymentGetResponse deployment;
                string ipPriv;

                if (null == FetchVm(vmName, serviceName, out service, out deployment, out ipPriv))
                {
                    if (throwIfNotFound)
                        throw new Exception("Unable to locate VM '" + vmName + "'");

                    return new HttpResponse()
                    {
                        HadError = false,
                        Body = ""
                    };
                }

                OperationStatusResponse osr = null;
                try
                {
                    osr = Client.VirtualMachines.Delete(service.ServiceName,
                        deployment.Name, vmName, deleteFromStorage);

                    if (osr.Status == OperationStatus.Failed)
                        throw new Exception(osr.Error.Message);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("the only role present"))
                    {
                        osr = Client.Deployments.DeleteByName(
                            service.ServiceName, deployment.Name, deleteFromStorage);

                        if (osr.Status == OperationStatus.Failed)
                            throw new Exception(osr.Error.Message);
                    }
                    else
                        throw;
                }

                return new HttpResponse()
                {
                    HadError = false,
                    Body = "",
                    StatusCheckUrl = string.Format(URLTEMPLATE_FETCHVMINFOINSTANCEVIEW_ARM, Connection.SubcriptionID, serviceName, vmName, "2015-06-15")

                };
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmOps.Delete() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        ///  <summary>
        /// Start VM
        ///  </summary>
        ///  <param name="vmName"></param>
        /// <param name="serviceName"></param>
        /// ///  <returns>returns StatusResponse Object</returns>
        /// 
        //*********************************************************************
        public string Start(string vmName, string serviceName)
        {
            try
            {
                if (IsArm)
                    return StartVmArm(vmName, serviceName);

                HostedServiceListResponse.HostedService service;
                DeploymentGetResponse deployment;
                string ipPriv;

                if (null == FetchVm(vmName, serviceName, out service, out deployment, out ipPriv))
                    throw new Exception("Unable to locate VM '" + vmName + "'");

                var resp = Client.VirtualMachines.Start(service.ServiceName, deployment.Name, vmName);

                if (resp.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception("HTTP communication Exception");


                if (resp.Status != OperationStatus.Succeeded)
                    throw new Exception(resp.Error.Message);

                return resp.RequestId;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmOps.Start() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmName"></param>
        /// <param name="serviceName"></param>
        /// <param name="isDeallocate"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string StopVM(string vmName, string serviceName, bool isDeallocate)
        {
            if (IsArm)
                return StopVmArm(vmName, serviceName, isDeallocate);

            HostedServiceListResponse.HostedService service;
            DeploymentGetResponse deployment;
            string ipPriv;

            if (null == FetchVm(vmName, serviceName, out service, out deployment, out ipPriv))
                throw new Exception("Unable to locate VM '" + vmName + "'");

            var action = isDeallocate == true ? PostShutdownAction.StoppedDeallocated : PostShutdownAction.Stopped;

            var parameters = new VirtualMachineShutdownParameters
            {

                PostShutdownAction = action
            };

            var resp = Client.VirtualMachines.Shutdown(service.ServiceName, deployment.Name, vmName, parameters);

            if (resp.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception("HTTP communication Exception");


            if (resp.Status != OperationStatus.Succeeded)
                throw new Exception(resp.Error.Message);

            return resp.RequestId;
        }

        //*********************************************************************
        ///
        ///  <summary>
        /// Stop VM
        ///  </summary>
        ///  <param name="vmName">Virtual Machine Name</param>
        /// <param name="serviceName">Cloud Service Name</param>
        /// ///  <returns>StatusResponse object</returns>
        /// 
        //*********************************************************************
        public string Stop(string vmName, string serviceName)
        {
            try
            {
                return StopVM(vmName, serviceName, false);

            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmOps.Stop() : " +
                   Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        ///  <summary>
        /// Deallocate VM
        ///  </summary>
        ///  <param name="vmName">Virtual Machine Name</param>
        /// <param name="serviceName">Cloud Service Name</param>
        /// ///  <returns></returns>
        /// 
        //*********************************************************************
        public string Deallocate(string vmName, string serviceName)
        {
            try
            {
                return StopVM(vmName, serviceName, true);

            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmOps.Deallocate() : " +
                   Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///   <summary>
        ///  Resize VM
        ///   </summary>
        ///   <param name="vmName">Virtual Machine Name</param>
        ///  <param name="serviceName">Cloud ServiceName</param>
        ///  <param name="roleSize">the size of the Vm ie the input value should be among 
        ///  "ExtraSmall,Small,Medium,Large,ExtraLarge,A5,A6,A7,A8,A9,Basic_A0,Basic_A1,Basic_A2,Basic_A3,Basic_A4"</param>
        /// <param name="size"></param>
        /// ///  <returns></returns>
        ///  
        //*********************************************************************
        public string Resize(string vmName, string serviceName, string size)
        {
            try
            {

                HostedServiceListResponse.HostedService service;
                DeploymentGetResponse deployment;
                string ipPriv;

                var vm = FetchVm(vmName, serviceName,
                     out service, out deployment, out ipPriv);

                if (null == vm)
                    throw new Exception("Unable to locate VM '" +
                        vmName + "'");

                if (null == service)
                    throw new Exception("Unable to locate VM host service'" +
                        serviceName + "'");

                if (!HostedServiceOps.IsRoleSizeSupported(service, size))
                    throw new Exception(string.Format(
                        "VM host service: '{0}' does not support requested role size: '{1}'",
                        serviceName, size));

                vm.RoleSize = size.ToString();
                var parameters = RoleToVmUpdateParams(vm);


                return UpdateVm(service.ServiceName, deployment.Name,
                   vmName, parameters);


            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmOps.Resize() : " +
                   Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmName"></param>
        /// <param name="serviceName"></param>
        /// <param name="newName"></param>
        /// 
        //*********************************************************************

        public void Rename(string vmName, string serviceName, string newName)
        {
            try
            {
                HostedServiceListResponse.HostedService service;
                DeploymentGetResponse deployment;
                string ipPriv;

                var vm = FetchVm(vmName, serviceName,
                     out service, out deployment, out ipPriv);

                if (null == vm)
                    throw new Exception("Unable to locate VM '" +
                        vmName + "'");
                vm.RoleName = newName;
                var parameters = RoleToVmUpdateParams(vm);

                UpdateVm(service.ServiceName, deployment.Name,
                   vmName, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmOps.Rename() : " +
                   Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmName"></param>
        ///  <param name="serviceName"></param>
        /// <param name="roleSizeName"></param>
        /// <returns></returns>
        //*********************************************************************
        public int FetchDiskCount(string vmName, string serviceName, out string roleSizeName)
        {
            try
            {
                HostedServiceListResponse.HostedService service;
                DeploymentGetResponse deployment;
                string ipPriv;

                var vm = FetchVm(vmName, serviceName,
                     out service, out deployment, out ipPriv);

                if (null == vm)
                    throw new Exception("Unable to locate VM '" +
                        vmName + "'");

                roleSizeName = vm.RoleSize;

                return null == vm.DataVirtualHardDisks ? 0 : vm.DataVirtualHardDisks.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmOps.FetchDiskList() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        ///  <summary>
        /// Add Data Disks to a VM
        ///  </summary>
        ///  <param name="vmName"></param>
        /// <param name="serviceName"></param>
        /// <param name="disks">List of disks which needs to be added</param>
        /// ///  <returns></returns>
        /// 
        //*********************************************************************
        public string AddDisk(string vmName, string serviceName, List<VhdInfo> disks)
        {
            try
            {
                char[] drvLetters = { 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
                HostedServiceListResponse.HostedService service;
                DeploymentGetResponse deployment;
                string ipPriv;


                var vm = FetchVm(vmName, serviceName,
                     out service, out deployment, out ipPriv);

                if (null == vm)
                    throw new Exception("Unable to locate VM '" +
                        vmName + "'");

                foreach (var dsk in disks)
                {
                    var newDsk = new DataVirtualHardDisk();
                    newDsk.Label = BuildDiskLabel(vm.DataVirtualHardDisks, drvLetters, vmName);
                    if (vm.DataVirtualHardDisks.Count >= 1)
                    {
                        newDsk.LogicalUnitNumber = vm.DataVirtualHardDisks.Max(l => l.LogicalUnitNumber) + 1;
                    }
                    else
                    {
                        newDsk.LogicalUnitNumber = 1;
                    }
                    newDsk.LogicalDiskSizeInGB = dsk.LogicalDiskSizeInGB;
                    newDsk.HostCaching = dsk.HostCaching;
                    newDsk.MediaLink = new System.Uri(dsk.MediaLink + "/" + newDsk.Label + ".vhd");
                    vm.DataVirtualHardDisks.Add(newDsk);
                }

                var parameters = RoleToVmUpdateParams(vm);


                var requestId = UpdateVm(service.ServiceName, deployment.Name,
                   vmName, parameters);

                return string.Format("https://management.core.windows.net/{0}/operations/{1}",
                Connection.SubcriptionID, requestId);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmOps.AddDisk() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmName"></param>
        /// <param name="serviceName"></param>
        /// <param name="diskName"></param>
        //*********************************************************************
        public string AddExistingDisk(string vmName, string serviceName, string diskName)
        {
            try
            {
                // Find the disk in the subscription
                var foundDisk = Client.VirtualMachineDisks.GetDisk(diskName);

                if (foundDisk != null && !string.IsNullOrEmpty(foundDisk.Name))
                {
                    char[] drvLetters = { 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
                    HostedServiceListResponse.HostedService service;
                    DeploymentGetResponse deployment;
                    string ipPriv;


                    var vm = FetchVm(vmName, serviceName, out service, out deployment, out ipPriv);

                    if (null == vm)
                        throw new Exception("Unable to locate VM '" + vmName + "'");

                    // Add the disk to the VM
                    var diskToAdd = new DataVirtualHardDisk
                    {
                        Name = foundDisk.Name,
                        Label = BuildDiskLabel(vm.DataVirtualHardDisks, drvLetters, vmName),
                        LogicalUnitNumber = (vm.DataVirtualHardDisks.Max(d => d.LogicalUnitNumber) ?? 0) + 1,
                    };
                    vm.DataVirtualHardDisks.Add(diskToAdd);

                    var parameters = RoleToVmUpdateParams(vm);


                    var requestId = UpdateVm(service.ServiceName, deployment.Name,
                       vmName, parameters);

                    return string.Format("https://management.core.windows.net/{0}/operations/{1}",
                        Connection.SubcriptionID, requestId);
                }
                else
                {
                    throw new Exception("Disk with name  " + diskName + " not found in the subscription");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmOps.AddExistingDisk() : " + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmName"></param>
        /// <param name="serviceName"></param>
        /// <param name="diskName"></param>
        /// <param name="deleteFromStorage"></param>
        /// 
        //*********************************************************************

        public string DetachDisk(string vmName, string serviceName, string diskName, bool deleteFromStorage)
        {
            try
            {
                // Get VM info
                HostedServiceListResponse.HostedService service;
                DeploymentGetResponse deployment;
                string ipPriv;
                var role = FetchVm(vmName, serviceName, out service, out deployment, out ipPriv);

                var disk = role.DataVirtualHardDisks.Single(d => d.Name.Trim().ToUpper() == diskName.Trim().ToUpper());

                var osr = Client.VirtualMachineDisks.DeleteDataDisk(
                    service.ServiceName, deployment.Name, role.RoleName, 
                    disk.LogicalUnitNumber ?? 0, deleteFromStorage);

                return string.Format("https://management.core.windows.net/{0}/operations/{1}",
                    Connection.SubcriptionID, osr.RequestId);

            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmOps.DetachDisk() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualMachineName"></param>
        /// <param name="hostServiceName"></param>
        /// <param name="diskName"></param>
        /// <param name="newSizeInGb"></param>
        /// 
        //*********************************************************************

        public void GrowDiskSize(string virtualMachineName, string hostServiceName, string diskName, int newSizeInGb)
        {
            try
            {
                // Get VM info
                HostedServiceListResponse.HostedService service;
                DeploymentGetResponse deployment;
                string ipPriv;
                var role = FetchVm(virtualMachineName, hostServiceName,
                    out service, out deployment, out ipPriv);

                // Capture Disk info
                var diskToGrow =
                    role.DataVirtualHardDisks.Single(d => d.Name.Trim().ToUpper() ==
                        diskName.Trim().ToUpper());

                // Detach the disk
                this.DetachDisk(virtualMachineName, hostServiceName, diskToGrow.Name, false);

                // Note: b-kasawa: Exceptions in 'Finally' block swallow up the ones in the 'Catch' block. Hence handled specially using var growDiskException
                Exception growDiskException = null;
                try
                {
                    // Grow the disk

                }
                catch (Exception ex)
                {
                    growDiskException = ex;
                }
                finally // Attach the disk back. 
                {
                    try
                    {
                        // Attach the disk back
                        this.AddExistingDisk(virtualMachineName, hostServiceName, diskToGrow.Name);
                    }
                    catch (Exception e)
                    {
                        if (growDiskException != null)
                            throw new Exception("Exception in VmOps.GrowDiskSize() : Both GrowDisk and AttachDisk operations encountered exceptions.", growDiskException);
                        else
                            throw new Exception("Exception in VmOps.GrowDiskSize() : Disk grown successfully but couldn't be attached back to the VM.", e);   
                    }

                    if (growDiskException != null)
                        throw growDiskException;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmOps.GrowDiskSize() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="drives"></param>
        /// <param name="drvLetters"></param>
        /// <param name="vmName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string BuildDiskLabel(IList<DataVirtualHardDisk> drives, char[] drvLetters, string vmName)
        {
            foreach (var disk in drives)
            {
                drvLetters = drvLetters.Except(disk.Label.Split('-')[1]).ToArray<char>();
            }
            return vmName + "-" + drvLetters.First();
        }
        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static bool DoesNameResolve(string name)
        {
            try
            {
                var addrList = Dns.GetHostAddresses(name);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region *** ARM *******************************************************

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        private bool IsArm
        {
            get
            {
                var azureApi =
                    Microsoft.Azure.CloudConfigurationManager.GetSetting("AzureApi") as string;

                return null != azureApi && azureApi.Equals("ARM");
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmc"></param>
        /// 
        //*********************************************************************

        private void ReferenceExistingArmResources(VmConfig vmc)
        {
            //return; //* TODO * Remove after this method is completed

            var needToRemoveVnetRef = false;
            var config = vmc.AzureArmConfig;
            var armResourceList = new List<ArmResource>(config.properties.template.resources.Length);

            //*** step 1: look for a VNet that matches the one requested

            foreach (var resource in config.properties.template.resources)
            {
                switch (resource.type)
                {
                    case "Microsoft.Storage/storageAccounts":
                        armResourceList.Add(resource);
                        break;
                    case "Microsoft.Network/publicIPAddresses":
                        armResourceList.Add(resource);
                        break;
                    case "Microsoft.Network/networkInterfaces":
                        armResourceList.Add(resource);
                        break;
                    case "Microsoft.Network/virtualNetworks":

                        var hso = new VnetOps(Connection);
                        var res = hso.FetchVnetInfoArm(
                            vmc.HostedServiceConfig.ServiceName,
                            config.properties.template.variables.virtualNetworkName);

                        //* if we didn't find the VNet, we need to leave it in the  resource list
                        if (null == res)
                            armResourceList.Add(resource);
                        else // otherwise, don't add it, and mark the reference for deletion
                            needToRemoveVnetRef = true;

                        break;
                    default:
                        armResourceList.Add(resource);
                        break;
                }
            }

            if (needToRemoveVnetRef)
            {
                config.properties.template.resources = armResourceList.ToArray();

                //*** step 2: remove the VNet reference

                foreach (var resource in config.properties.template.resources)
                {
                    switch (resource.type)
                    {
                        case "Microsoft.Network/networkInterfaces":

                            // remove "[concat('Microsoft.Network/virtualNetworks/', variables('virtualNetworkName'))]"
                            // from "Microsoft.Network/networkInterfaces" : dependsOn

                            var dependsOnList = new List<string>(resource.dependsOn.Length);

                            dependsOnList.AddRange(resource.dependsOn.Where(dependsOn =>
                                !dependsOn.Contains("Microsoft.Network/virtualNetworks/")));

                            resource.dependsOn = dependsOnList.ToArray();

                            break;
                    }
                }
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmName"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string StartVmArm(string vmName, string serviceName)
        {
            var requestUrl = string.Format(URLTEMPLATE_STARTVM_ARM,
                Connection.SubcriptionID, serviceName, vmName, APIVERSION_VMOPS);

            var hi = new HttpInterface(Connection);
            var resp = hi.PerformRequestArm(HttpInterface.RequestType_Enum.POST, requestUrl);

            if (resp.HadError)
                throw new Exception("Error in StartVmArm ARM REST API call: " + resp.Body);

            return resp.StatusCheckUrl;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmName"></param>
        /// <param name="serviceName"></param>
        /// <param name="isDeallocate"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string StopVmArm(string vmName, string serviceName, bool isDeallocate)
        {
            //var requestUrl = string.Format(isDeallocate ?
            //    URLTEMPLATE_STOPVM_ARM : URLTEMPLATE_DEALLOCATEVM_ARM,
            //    Connection.SubcriptionID, serviceName, vmName, APIVERSION_VMOPS);

            var requestUrl = string.Format(URLTEMPLATE_DEALLOCATEVM_ARM,
                Connection.SubcriptionID, serviceName, vmName, APIVERSION_VMOPS);

            var hi = new HttpInterface(Connection);
            var resp = hi.PerformRequestArm(HttpInterface.RequestType_Enum.POST, requestUrl);

            if (resp.HadError)
                throw new Exception("Error in StopVmArm ARM REST API call: " + resp.Body);

            return resp.StatusCheckUrl;
        }

        //*********************************************************************
        ///
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="vmName"></param>
        ///  <param name="serviceName"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************
        private HttpResponse RestartVmArm(string vmName, string serviceName)
        {
            var requestUrl = string.Format(URLTEMPLATE_RESTARTVM_ARM,
                Connection.SubcriptionID, serviceName, vmName, APIVERSION_VMOPS);

            var hi = new HttpInterface(Connection);
            var resp = hi.PerformRequestArm(HttpInterface.RequestType_Enum.POST, requestUrl);

            if (resp.HadError)
                throw new Exception("Error in RestartVmArm ARM REST API call: " + resp.Body);

            //resp.StatusCheckUrl = string.Format(URLTEMPLATE_FETCHVMINFOINSTANCEVIEW_ARM, Connection.SubcriptionID,
            //    serviceName, vmName, "2015-06-15");

            return resp;
        }

        //*********************************************************************
        /// 
        ///   <summary>
        ///  
        ///   </summary>
        ///   <param name="vmName"></param>
        ///  <param name="serviceName"></param>
        ///  <param name="deleteFromStorage"></param>
        /// <param name="throwIfNotFound"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        private HttpResponse DeleteVmArm(string vmName,
            string serviceName, bool deleteFromStorage, bool throwIfNotFound)
        {
            var requestUrl = string.Format(URLTEMPLATE_DELETEVM_ARM,
                Connection.SubcriptionID, serviceName, vmName, APIVERSION_VMOPS);

            var hi = new HttpInterface(Connection);
            var resp = hi.PerformRequestArm(HttpInterface.RequestType_Enum.DELETE, requestUrl);

            if (resp.HadError)
                throw new Exception("Error in DeleteVmArm ARM REST API call: " + resp.Body);

            return resp;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmInfoJson"></param>
        /// <param name="resourceGroupName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public string FetchVmIpAddressArm(string vmInfoJson, string resourceGroupName)
        {
            var nicNameList = FetchArmVmNicIds(vmInfoJson);

            if (null == nicNameList)
                throw new Exception("VM contains no network interfaces");

            if (0 == nicNameList.Count)
                throw new Exception("VM contains no network interfaces");

            var nicAddrName = FetchArmNicAddrId(Connection.AdToken, Connection.SubcriptionID, resourceGroupName, nicNameList[0]);

            if (null == nicNameList)
                throw new Exception("VM contains no network interfaces");

            var ipAddress = FetchArmIpAddr(Connection.AdToken, Connection.SubcriptionID, resourceGroupName, nicAddrName);

            return ipAddress;
        }

        public string FetchVmPublicFqdnArm(string vmInfoJson, string resourceGroupName)
        {
            var nicNameList = FetchArmVmNicIds(vmInfoJson);

            if (null == nicNameList)
                throw new Exception("VM contains no network interfaces");

            if (0 == nicNameList.Count)
                throw new Exception("VM contains no network interfaces");

            var nicAddrName = FetchArmNicAddrId(Connection.AdToken, Connection.SubcriptionID, resourceGroupName, nicNameList[0]);

            if (null == nicNameList)
                throw new Exception("VM contains no network interfaces");

            var publicFqdn = FetchArmPublicFqdn(Connection.AdToken, Connection.SubcriptionID, resourceGroupName, nicAddrName);

            return publicFqdn;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aadToken"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="resourceGroupName"></param>
        /// <param name="ipAddrName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string FetchArmIpAddr(AuthenticationResult aadToken,
            string subscriptionId, string resourceGroupName, string ipAddrName)
        {
            var nicListOut = new List<string>();
            try
            {
                var vm = FetchArmIpAddrInfo(aadToken, subscriptionId, resourceGroupName, ipAddrName);

                if (null == vm)
                    throw new Exception("Given Public IP Address: '" + ipAddrName + "' not found");

                var jvList = Utilities.FetchJsonValue(vm, "properties");

                if (null == jvList)
                    return null;

                jvList = Utilities.FetchJsonValue(jvList.ToString(), "ipAddress");

                if (null == jvList)
                    return null;

                return jvList.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchArmIpAddr() :" + ex.Message);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aadToken"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="resourceGroupName"></param>
        /// <param name="ipAddrName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string FetchArmPublicFqdn(AuthenticationResult aadToken,
            string subscriptionId, string resourceGroupName, string ipAddrName)
        {
            var nicListOut = new List<string>();
            try
            {
                var vm = FetchArmIpAddrInfo(aadToken, subscriptionId, resourceGroupName, ipAddrName);

                if (null == vm)
                    throw new Exception("Given Public IP Address: '" + ipAddrName + "' not found");

                var jvList = Utilities.FetchJsonValue(vm, "properties");

                if (null == jvList)
                    return null;

                jvList = Utilities.FetchJsonValue(jvList.ToString(), "dnsSettings");

                if (null == jvList)
                    return null;

                jvList = Utilities.FetchJsonValue(jvList.ToString(), "fqdn");

                if (null == jvList)
                    return null;

                return jvList.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchArmPublicFqdn() :" + ex.Message);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aadToken"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="resourceGroupName"></param>
        /// <param name="ipAddrName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string FetchArmIpAddrInfo(AuthenticationResult aadToken,
            string subscriptionId, string resourceGroupName, string ipAddrName)
        {
            try
            {
                var url =
                    string.Format(@"https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Network/publicIPAddresses/{2}?api-version={3}",
                        subscriptionId, resourceGroupName, ipAddrName, "2015-06-15");

                var hi = new HttpInterface(Connection);
                var resp = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url);

                if (!resp.HadError)
                    return resp.Body;

                throw new Exception(resp.Body);

            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchArmIpAddrInfo() :" + ex.Message);
            }
        }

        private string FetchSubscriptionNameArm(string subscriptionId)
        {
            try
            {
                var url =
                    string.Format(@"https://management.azure.com/subscriptions/{0}?api-version={1}",
                        subscriptionId, "2015-11-01");

                var hi = new HttpInterface(Connection);
                var resp = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url);

                if (!resp.HadError)
                    return Utilities.FetchJsonValue(resp.Body, "displayName") as string;

                throw new Exception(resp.Body);

            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchSubscriptionInfoArm() :" + ex.Message);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmInfoJson"></param>
        /// <returns></returns>
        ///
        //*********************************************************************

        private List<string> FetchArmVmNicIds(string vmInfoJson)
        {
            var nicListOut = new List<string>();
            try
            {
                var jvList = Utilities.FetchJsonValue(vmInfoJson, "properties");

                if (null == jvList)
                    return null;

                jvList = Utilities.FetchJsonValue(jvList.ToString(), "networkProfile");

                if (null == jvList)
                    return null;

                jvList = Utilities.FetchJsonValue(jvList.ToString(), "networkInterfaces");

                if (null == jvList)
                    return null;

                var nicList = jvList as Newtonsoft.Json.Linq.JArray;

                if (null == nicList)
                    return null;

                foreach (var nic in nicList)
                {
                    var id = Utilities.FetchJsonValue(nic.ToString(), "id");
                    if (null != id)
                    {
                        var idParts = id.ToString().Split(new char[] { '/' });
                        nicListOut.Add(idParts[idParts.Count() - 1]);
                    }
                }

                return nicListOut;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchArmVmNicIds() :" + ex.Message);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aadToken"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="resourceGroupName"></param>
        /// <param name="nicName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string FetchArmNicAddrId(AuthenticationResult aadToken,
            string subscriptionId, string resourceGroupName, string nicName)
        {
            var nicListOut = new List<string>();
            try
            {
                var vm = FetchArmNicInfo(aadToken, subscriptionId, resourceGroupName, nicName);

                if (null == vm)
                    throw new Exception("Given NIC: '" + nicName + "' not found");

                var jvList = Utilities.FetchJsonValue(vm, "properties");

                if (null == jvList)
                    return null;

                jvList = Utilities.FetchJsonValue(jvList.ToString(), "ipConfigurations");

                var ipConfigList = jvList as Newtonsoft.Json.Linq.JArray;

                if (null == ipConfigList)
                    return null;

                if (0 == ipConfigList.Count)
                    return null;

                jvList = Utilities.FetchJsonValue(ipConfigList[0].ToString(), "properties");

                if (null == jvList)
                    return null;

                jvList = Utilities.FetchJsonValue(jvList.ToString(), "publicIPAddress");

                if (null == jvList)
                    return null;

                var id = Utilities.FetchJsonValue(jvList.ToString(), "id");

                if (null == id)
                    return null;

                var idParts = id.ToString().Split(new char[] { '/' });
                return idParts[idParts.Count() - 1];
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchArmNicAddrId() :" + ex.Message);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aadToken"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="resourceGroupName"></param>
        /// <param name="nicName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string FetchArmNicInfo(AuthenticationResult aadToken,
            string subscriptionId, string resourceGroupName, string nicName)
        {
            try
            {
                var url =
                    string.Format(@"https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Network/networkInterfaces/{2}?api-version={3}",
                        subscriptionId, resourceGroupName, nicName, "2015-06-15");

                var hi = new HttpInterface(Connection);
                var resp = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url);

                if (!resp.HadError)
                    return resp.Body;

                throw new Exception(resp.Body);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchArmNicInfo() :" + ex.Message);
            }
        }

        private List<CmpInterfaceModel.Models.DataVirtualHardDisk> ExtractDataDiskInfo(string vmInfoJson)
        {
            try
            {
                var dataVirtualHardDisks = new List<CmpInterfaceModel.Models.DataVirtualHardDisk>();

                var jdata = Utilities.FetchJsonValue(vmInfoJson,
                    new string[] { "properties", "storageProfile", "dataDisks" });

                if (null == jdata as Newtonsoft.Json.Linq.JArray)
                    return dataVirtualHardDisks;

                var dvhds = jdata as Newtonsoft.Json.Linq.JArray;

                dataVirtualHardDisks.AddRange(dvhds.Select(dvhd => new CmpInterfaceModel.Models.DataVirtualHardDisk
                {
                    DiskName = dvhd["name"].ToString(),
                    DiskLabel = null,
                    HostCaching = dvhd["caching"].ToString(),
                    LogicalDiskSizeInGB = dvhd["diskSizeGB"].ToString(),
                    Lun = dvhd["lun"].ToString(),
                    MediaLink = Utilities.FetchJsonValue(dvhd.ToString(), new string[] { "vhd", "uri" }) as string,
                    //  SourceMediaLink = x.SourceMediaLink.ToString()
                }));

                return dataVirtualHardDisks;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ExtractDataDiskInfo() :" + Utilities.UnwindExceptionMessages(ex));
            }
        }

        public VmRole ExtractVmInfo(string vmInfoJson, string vmName, string resourceGroupName)
        {
            try
            {
                //ExtractDataDiskInfo(vmInfoJson);

                var osVirtualDisk = new OsVirtualHardDisk
                {
                    DiskLabel = "C",
                    DiskName =
                        Utilities.FetchJsonValue(vmInfoJson,
                            new string[] { "properties", "storageProfile", "osDisk", "name" }) as string,
                    HostCaching =
                        Utilities.FetchJsonValue(vmInfoJson,
                            new string[] { "properties", "storageProfile", "osDisk", "caching" }) as string,
                    MediaLink =
                        Utilities.FetchJsonValue(vmInfoJson,
                            new string[] { "properties", "storageProfile", "osDisk", "vhd", "uri" }) as string,
                    OS =
                        Utilities.FetchJsonValue(vmInfoJson,
                            new string[] { "properties", "storageProfile", "osDisk", "osType" }) as string,
                    SourceImageName = null,
                    RemoteSourceImageLink = null
                };

                var name = Utilities.FetchJsonValue(vmInfoJson, new string[] { "name" }) as string;
                var size = Utilities.FetchJsonValue(vmInfoJson, new string[] { "properties", "hardwareProfile", "vmSize" }) as string;

                var deployment = new Deployment
                {
                    PrivateID = Utilities.FetchJsonValue(vmInfoJson, new string[] { "properties", "vmId" }) as string,
                    RoleInstanceList = new List<DeploymentRoleInstance>
                    {
                        new DeploymentRoleInstance
                        {
                            IpAddress = FetchVmIpAddressArm(vmInfoJson, resourceGroupName),
                            RemoteAccessCertificateThumbprint = null,
                            InstanceName = name,
                            InstanceSize = size,
                            //InstanceStatus = Utilities.FetchJsonValue(vmInfoJson, new string[] {"properties", "provisioningState"}) as string,
                            InstanceStatus = GetVmInstanceStatus(vmName, resourceGroupName),
                            GuestAgentStatus = null,
                            HostName = Utilities.FetchJsonValue(vmInfoJson, new string[] {"properties", "osProfile", "computerName"})
                                    as string,
                            //InstanceFaultDomain = null,
                            InstanceStateDetails = null,
                            //InstanceUpgradeDomain = null,
                            PowerState = null,
                            ResourceExtensionStatusList = null,
                            RoleName = name
                        }
                    }.ToArray(),
                    Url = FetchVmPublicFqdnArm(vmInfoJson, resourceGroupName)
                };

                var subscription = new SubscriptionInfo
                {
                    CurrentCoreCount = "0",
                    MaximumCoreCount = "100",
                    SubscriptionID = Connection.SubcriptionID,
                    SubscriptionName = FetchSubscriptionNameArm(Connection.SubcriptionID)
                };

                var vmrole = new VmRole(name, size,
                    ExtractDataDiskInfo(vmInfoJson), osVirtualDisk, deployment, subscription);

                return vmrole;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetVmArm() :" + 
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///   <summary>
        ///  
        ///   </summary>
        ///   <param name="vmName"></param>
        /// <param name="resourceGroupName"></param>
        ///  
        //*********************************************************************
        public VmRole GetVmArm(string vmName, string resourceGroupName)
        {
            try
            {
                var url = string.Format(URLTEMPLATE_FETCHVMINFO_ARM,
                    Connection.SubcriptionID, resourceGroupName, vmName, "2015-06-15");

                var hi = new HttpInterface(Connection);
                var resp = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url);

                if (resp.HadError)
                    throw new Exception("Exception in Azure call within GetVmArm() :" + resp.Body);

                var vmInfoJson = resp.Body;

                //ExtractDataDiskInfo(vmInfoJson);

                var osVirtualDisk = new OsVirtualHardDisk
                {
                    DiskLabel = "C",
                    DiskName =
                        Utilities.FetchJsonValue(vmInfoJson,
                            new string[] { "properties", "storageProfile", "osDisk", "name" }) as string,
                    HostCaching =
                        Utilities.FetchJsonValue(vmInfoJson,
                            new string[] { "properties", "storageProfile", "osDisk", "caching" }) as string,
                    MediaLink =
                        Utilities.FetchJsonValue(vmInfoJson,
                            new string[] { "properties", "storageProfile", "osDisk", "vhd", "uri" }) as string,
                    OS =
                        Utilities.FetchJsonValue(vmInfoJson,
                            new string[] { "properties", "storageProfile", "osDisk", "osType" }) as string,
                    SourceImageName = null,
                    RemoteSourceImageLink = null
                };

                var name = Utilities.FetchJsonValue(vmInfoJson, new string[] { "name" }) as string;
                var size = Utilities.FetchJsonValue(vmInfoJson, new string[] { "properties", "hardwareProfile", "vmSize" }) as string;

                var deployment = new Deployment
                {
                    PrivateID = Utilities.FetchJsonValue(vmInfoJson, new string[] { "properties", "vmId" }) as string,
                    RoleInstanceList = new List<DeploymentRoleInstance>
                    {
                        new DeploymentRoleInstance
                        {
                            IpAddress = FetchVmIpAddressArm(vmInfoJson, resourceGroupName),
                            RemoteAccessCertificateThumbprint = null,
                            InstanceName = name,
                            InstanceSize = size,
                            //InstanceStatus = Utilities.FetchJsonValue(vmInfoJson, new string[] {"properties", "provisioningState"}) as string,
                            InstanceStatus = GetVmInstanceStatus(vmName, resourceGroupName),
                            GuestAgentStatus = null,
                            HostName = Utilities.FetchJsonValue(vmInfoJson, new string[] {"properties", "osProfile", "computerName"})
                                    as string,
                            //InstanceFaultDomain = null,
                            InstanceStateDetails = null,
                            //InstanceUpgradeDomain = null,
                            PowerState = null,
                            ResourceExtensionStatusList = null,
                            RoleName = name
                        }
                    }.ToArray(),
                    Url = FetchVmPublicFqdnArm(vmInfoJson, resourceGroupName)
                };

                //*** TODO * markwes * use region of given VM
                var azureRegion = "westus";

                var subOps = new SubscriptionOps(Connection);
                var coreUsage = subOps.FetchResourceUsage(azureRegion, "cores");

                var subscription = new SubscriptionInfo
                {
                    CurrentCoreCount = coreUsage.Value.ToString(),
                    MaximumCoreCount = coreUsage.Limit.ToString(),
                    SubscriptionID = Connection.SubcriptionID,
                    SubscriptionName = FetchSubscriptionNameArm(Connection.SubcriptionID)
                };

                var vmrole = new VmRole(name, size,
                    ExtractDataDiskInfo(vmInfoJson), osVirtualDisk, deployment, subscription);

                return vmrole;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetVmArm() :" + 
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmName"></param>
        /// <param name="resourceGroupName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string GetVmInstanceStatus(string vmName, string resourceGroupName)
        {
            var url = string.Format(URLTEMPLATE_FETCHVMINFOINSTANCEVIEW_ARM,
                  Connection.SubcriptionID, resourceGroupName, vmName, "2015-06-15");

            var hi = new HttpInterface(Connection);
            var resp = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url);

            if (resp.HadError)
                throw new Exception("Exception in Azure call within GetVmInstanceStatus() :" + resp.Body);

            try
            {
                var statusCollection = DataContracts.Status.DeserializeJsonInstanceViewStatus(resp.Body);
                DataContracts.Status s = statusCollection.Statuses.FirstOrDefault(x => x.Code.IndexOf("PowerState", StringComparison.InvariantCultureIgnoreCase) >= 0);
                
                if (s == null) return "Could not retrieve VM status at this time";

                string powerState = s.Code.Split('/')[1];

                switch (powerState)
                {
                    case "running":
                        powerState = "Running";
                        break;
                    case "deallocated":
                        powerState = "Stopped / Deallocated";
                        break;
                    default:
                        powerState = powerState.Substring(0, 1).ToUpper() + powerState.Substring(1, powerState.Length);
                        break;
                }

                return powerState;

            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetVmInstanceStatus() :" + 
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmRole"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private Role Convert(VmRole vmRole)
        {
            return new Role()
            {
                AvailabilitySetName = null, 
                ConfigurationSets = null, 
                DataVirtualHardDisks = null, 
                DefaultWinRmCertificateThumbprint = null, 
                Label = CmpInterfaceModel.Utilities.StringToB64(vmRole.RoleName), 
                MediaLocation = null, 
                OSVersion = null, 
                OSVirtualHardDisk = new OSVirtualHardDisk()
                {
                    HostCaching = vmRole.OSVirtualHardDisk.HostCaching,
                    Label = vmRole.OSVirtualHardDisk.DiskLabel,
                    Name = vmRole.OSVirtualHardDisk.DiskName, 
                    //IOType = null
                    MediaLink = (null != vmRole.OSVirtualHardDisk.MediaLink)? new Uri(vmRole.OSVirtualHardDisk.MediaLink) : null,
                    OperatingSystem = vmRole.OSVirtualHardDisk.OS,
                    RemoteSourceImageLink = (null != vmRole.OSVirtualHardDisk.RemoteSourceImageLink)? new Uri(vmRole.OSVirtualHardDisk.RemoteSourceImageLink) : null, 
                    //ResizedSizeInGB = null,
                    SourceImageName = vmRole.OSVirtualHardDisk.SourceImageName
                }, 
                ProvisionGuestAgent = true, 
                ResourceExtensionReferences = null,
                RoleName = vmRole.RoleName, 
                RoleSize = vmRole.RoleSize, 
                RoleType = "PersistentVMRole", 
                VMImageInput = null, 
                VMImageName = null
            };
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceGroupName"></param>
        /// 
        //*********************************************************************

        public List<Role> FetchVmListArm(string resourceGroupName)
        {
            string URLTEMPLATE_FETCHVMLISTINRESGROUP_ARM =
                "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Compute/virtualmachines?api-version={2}";

            var vmList = new List<Role>();

            try
            {
                var url = string.Format(URLTEMPLATE_FETCHVMLISTINRESGROUP_ARM,
                    Connection.SubcriptionID, resourceGroupName, "2015-06-15");

                var hi = new HttpInterface(Connection);
                var resp = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url);

                if (resp.HadError)
                    throw new Exception("Exception in Azure :" + resp.Body);

                var vmListJson = Utilities.FetchJsonValue(resp.Body, "value") as Newtonsoft.Json.Linq.JArray;

                if (null != vmListJson)
                    foreach (var vmJson in vmListJson)
                    {
                        var name = Utilities.FetchJsonValue(vmJson.ToString(), "name") as string;
                        vmList.Add(Convert(ExtractVmInfo(vmJson.ToString(), name, resourceGroupName)));
                    }

                return vmList;
            }
            catch (Exception ex)
            {
                throw new Exception("excpetion in FetchVmListArm() : " + 
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Role> FetchVmListArm()
        {
            // https://msdn.microsoft.com/en-us/library/azure/mt163572.aspx

            try
            {
                var hso = new HostedServiceOps(Connection);
                var resourceGroups = hso.FetchResourceGroupList();
                var vmList = new List<Role>();

                foreach (var rg in resourceGroups)
                {
                    vmList.AddRange(FetchVmListArm(rg.Name));
                }

                return vmList;
            }
            catch (Exception ex)
            {
                throw new Exception("excpetion in FetchVmListArm() : " + 
                    Utilities.UnwindExceptionMessages(ex));
            }
        }


        #endregion
    }
}
