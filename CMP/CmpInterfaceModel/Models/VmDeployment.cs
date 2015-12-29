using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml.Serialization;

namespace CmpInterfaceModel.Models
{
    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public partial class VmDeploymentRequest
    {
        public int ID { get; set; }

        public string RequestName { get; set; }
        public string RequestDescription { get; set; }
        public string ParentAppName { get; set; }
        public string ParentAppID { get; set; }
        public string RequestType { get; set; }//public enum RequestTypeEnum { NewVM, Migration, etc }
        public string TargetServiceProviderType { get; set; }//public enum TargetServiveProviderTypeEnum { Azure, Vmm, etc }
        public int TargetServiceProviderAccountID { get; set; }
        public string TargetServiceName { get; set; }

        public string TargetLocation { get; set; }
        public string TargetLocationType { get; set; }//public enum TargetLocationTypeEnum { Region, AffinityGroup, Vnet }

        public string TargetAccount { get; set; }
        public string TargetAccountType { get; set; }//public enum TargetAccountTypeEnum { Azure, MSIT }
        public string TargetAccountCreds { get; set; }

        public string VmSize { get; set; }//public enum VmSizeEnum { VmSizeCustom, ExtraSmall, Small, Medium, Large, ExtraLarge, A6, A7 }

        public string TagData { get; set; }
        public int TagID { get; set; }
        public string Config { get; set; }

        public string TargetVmName { get; set; }
        public string SourceServerName { get; set; }
        public string SourceServerRegion { get; set; }
        public string SourceVhdFilesCSV { get; set; }

        public string WhoRequested { get; set; }
        public DateTime WhenRequested { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionTypeCode { get; set; }
        public DateTime LastStatusUpdate { get; set; }
        public string Status { get; set; }
        public string StatusMessage { get; set; }
        public string ValidationResults { get; set; }
        public int AftsID { get; set; }
        public bool Active { get; set; }
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class VmConfig
    {
        public AzureVmDeploymentArm AzureArmConfig { get; set; }
        public AzureVmDeployment AzureVmConfig { get; set; }
        public HostedService HostedServiceConfig { get; set; }
        public List<CertificateFile> ServiceCertList { get; set; }
        public List<DiskSpec> DiskSpecList { get; set; }
        public List<UserSpec> UserSpecList { get; set; }
        public PagefileSpec PageFileConfig { get; set; }
        public PlacementSpec Placement { get; set; }
        public List<SequenceSpec> ScriptList { get; set; }
        public InfoFromVmSpec InfoFromVM { get; set; }
        public PostInfoFromVmSpec PostInfoFromVM { get; set; }
        public ValidateSpec ValidationConfig { get; set; }
        public CmdbSpec CmdbConfig { get; set; }
        public ApplicationSpec ApplicationConfig { get; set; }
        public SourceImageSpec SourceImageConfig { get; set; }
        public AzureApiSpec AzureApiConfig { get; set; }


        //CmpInterfaceModel.Models.MigrationConfig

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public string Serialize()
        {
            return Utilities.Serialize(typeof(VmConfig), this);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static VmConfig Deserialize(string input)
        {
            try
            {
                return Utilities.DeSerialize(typeof(VmConfig), input, true) as VmConfig;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmConfig.Deserialize() : Unable to deserialize given VM Config structure, may be malformed : " 
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        public static VmConfig Deserialize(string input, bool UnwindException)
        {
            try
            {
                return Utilities.DeSerialize(typeof(VmConfig), input,UnwindException) as VmConfig;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmConfig.Deserialize() : Unable to deserialize given VM Config structure, may be malformed : "
                    + Utilities.UnwindExceptionMessages(ex));
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

        public static VmConfig CreateSampleArm()
        {
            var vmDepReqList = new List<VmDeploymentRequest>();
            var vmDepReq = new VmDeploymentRequest { ID = 1 };

            /*var armParams = new ArmParameters()
            {
                adminPassword = new ArmAdminpassword(){value = "123abc!"},
                adminUsername = new ArmAdminusername(){value = "markw"},
                dnsNameForPublicIP = new ArmDnsnameforpublicip() { value = "armTestN27" },
                newStorageAccountName = new ArmNewstorageaccountname(){value = "armTestN27"},
                windowsOSVersion = new ArmWindowsosversion() { value = "2012-R2-Datacenter" }
            };*/

            var armVars = new ArmVariables()
            {
                location = "West US",
                imagePublisher = "MicrosoftWindowsServer",
                imageOffer = "WindowsServer",
                OSDiskName = "osdiskforwindowssimple",
                nicName = "myvmnic",
                addressPrefix = "10.0.0.0/16",
                subnetName = "Subnet",
                subnetPrefix = "10.0.0.0/24",
                storageAccountType = "Standard_LRS",
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
            };

            var armResources = new ArmResource[5];
            int index = 0;

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
                            properties = new ArmSubnetProperties(){addressPrefix = "[variables('subnetPrefix')]"}
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
                                    new Listener() { Protocol = "http" }
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
                        }
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

            //var serialized = vmDepArm.SerializeJson();

            return new VmConfig { AzureArmConfig = vmDepArm };
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*************************************************************************

        public static string CreateSampleArmString()
        {
            return CreateSampleArm().AzureArmConfig.SerializeJson();
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*************************************************************************

        public static VmConfig CreateSampleRdfe()
        {
            var vmDepReqList = new List<VmDeploymentRequest>();
            var vmDepReq = new VmDeploymentRequest { ID = 1 };

            var vmDep = new AzureVmDeployment { Name = "TheName", 
                Label = "TheLabel", DeploymentSlot = "Production", VirtualNetworkName = "TheVM" };

            //*** 

            var dvd = new DataVirtualHardDisk { DiskLabel = "DL",
                                                                DiskName = "DN",
                                                                HostCaching = "HostCaching",
                                                                LogicalDiskSizeInGB = "100",
                                                                Lun = "1",
                                                                MediaLink = "ML"
            };
            var dvdList = new List<DataVirtualHardDisk>();
            dvdList.Add(dvd);

            var ovd = new OsVirtualHardDisk { DiskLabel = "DL",
                                                            DiskName = "DN",
                                                            HostCaching = "HostCaching",
                                                            MediaLink = "ML",
                                                            SourceImageName = "SIN"
            };

            //***

            var dns = new DnsServer { Name = "DnsName", Address = "DnsAddress" };
            vmDep.Dns = new DnsServerList();
            vmDep.Dns.DnsServers = new List<DnsServer>();
            vmDep.Dns.DnsServers.Add(dns);

            //***

            var creds = new Credentials { Domain = "CredDomain", 
                Username = "CredUser", Password = "CredPassword" };
            var DJ = new DomainJoin { JoinDomain = "TheDomain", 
                MachineObjectOU = "MOU", Credentials = creds };

            var CertSet = new CertificateSetting { StoreLocation = "StoreLoc", 
                StoreName = "StoreName", Thumbprint = "TP" };
            var CertSetList = new List<CertificateSetting>();
            CertSetList.Add(CertSet);

            //Listener List = new Listener { CertificateThumbprint = "CertThumb", Type = "T" };
            var List = new Listener { CertificateThumbprint = "CertThumb", Protocol = "Https" };
            var ListList = new List<Listener>();
            ListList.Add(List);
            var WRM = new WinRM { Listeners = ListList };

            ConfigurationSet CS = new WindowsProvisioningConfigurationSet { AdminPassword = "AP", 
                AdminUsername = "AUN", ComputerName = "CN", 
                ConfigurationSetType = "WindowsProvisioningConfiguration", 
                DomainJoin = DJ, EnableAutomaticUpdates = "True",
                StoredCertificateSettings = CertSetList,
                TimeZone = "TZ",
                WinRM = WRM
            };
            var CsList = new List<ConfigurationSet>();
            CsList.Add(CS);

            //***

            var LBP = new LoadBalancerProbe { IntervalInSeconds = "100", 
                Path = "ThePath", Port = "4321", Protocol = "Prot", TimeoutInSeconds = "200" };
            var IE = new InputEndpoint { LoadBalancedEndpointSetName = "LBESN", 
                LoadBalancerProbe = LBP, LocalPort = "LP", Name = "N", Port = "1234", Protocol = "Prot" };
            var IeList = new List<InputEndpoint>();
            IeList.Add(IE);

            var SN = new SubnetNames { SubnetName = "XX" };
            var SubNameList = new List<SubnetNames>();
            SubNameList.Add(SN);

            /*ConfigurationSet CS2 = new NetworkConfigurationSet { ConfigurationSetType = "NetworkConfiguration", 
                SubnetNames = SubNameList, InputEndpoints = IeList };
            CsList.Add(CS2);*/

            //***

            Role R = new PersistentVMRole
            {
                AvailabilitySetName = "AsName",
                ConfigurationSets = CsList, 
                DataVirtualHardDisks = dvdList, 
                OSVirtualHardDisk = ovd, 
                RoleName = "RN", 
                RoleSize = "RS", 
                RoleType = "RT" 
            }
            ;
            vmDep.RoleList = new List<Role>();
            vmDep.RoleList.Add(R);

            var VMC = new VmConfig { AzureVmConfig = vmDep };

            return VMC;
        }
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class SourceImageSpec
    {
        public string OS { get; set; } //*** ["Windows","Linux"]
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class AzureApiSpec
    {
        public string Platform { get; set; } //*** ["RDFE","ARM"]
    }
    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class Credentials
    {
        public string Domain;
        public string Username;
        public string Password;
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class DomainJoin
    {
        public Credentials Credentials;
        public string JoinDomain;
        public string MachineObjectOU;
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class CertificateSetting
    {
        public string StoreLocation;
        public string StoreName;
        public string Thumbprint;
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class Listener
    {
        //public string Type;
        public string Protocol;
        public string CertificateThumbprint;
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class WinRM
    {
        public List<Listener> Listeners;
    }

    //*************************************************************************
    /// <summary>
    /// Linux Classes
    /// </summary>
    //*************************************************************************

    public class PublicKey
    {
        //public string Type;
        public string Fingerprint;
        public string Path;
    }

    public class KeyPair
    {
        //public string Type;
        public string Fingerprint;
        public string Path;
    }

    public class SSH
    {
        public List<PublicKey> PublicKeys;
        public List<KeyPair> KeyPairs;
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class SubnetNames
    {
        public string SubnetName { get; set; }
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class ComponentSetting
    {
        public string SettingName { get; set; }
        public string Content { get; set; }
    }

    public class UnattendComponent
    {
        public string ComponentName { get; set; }
        public List<ComponentSetting> ComponentSettings { get; set; }

    }

    public class UnattendPass
    {
        public string PassName { get; set; }
        public List<UnattendComponent> Components { get; set; }
    }

    public class AdditionalUnattendContent
    {
        public List<UnattendPass> Passes { get; set; }
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public abstract class ConfigurationSet
    {
        public string ConfigurationSetType { get; set; }
        public DomainJoin DomainJoin { get; set; } //*** always set = NULL for Linux
    }

    public class WindowsProvisioningConfigurationSet : ConfigurationSet
    {
        public string ComputerName { get; set; }
        public string AdminPassword { get; set; }
        public string EnableAutomaticUpdates { get; set; }
        public string TimeZone { get; set; }
        //public DomainJoin DomainJoin { get; set; }
        public List<CertificateSetting> StoredCertificateSettings { get; set; }
        public WinRM WinRM { get; set; }
        public string AdminUsername { get; set; }
        public AdditionalUnattendContent AdditionalUnattendContent { get; set; }
    }

    public class LinuxProvisioningConfigurationSet : ConfigurationSet
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string DisableSshPasswordAuthentication { get; set; }
        public SSH SSH { get; set; }
    }

    public class NetworkConfigurationSet : ConfigurationSet
    {
        public List<InputEndpoint> InputEndpoints { get; set; }
        public SubnetNames SubnetNames { get; set; }
        public string StaticVirtualNetworkIPAddress { get; set; }
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class LoadBalancerProbe
    {
        public string Path { get; set; }
        public string Port { get; set; }
        public string Protocol { get; set; }
        public string IntervalInSeconds { get; set; }
        public string TimeoutInSeconds { get; set; }
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class InputEndpoint
    {
        public string LoadBalancedEndpointSetName { get; set; }
        public string LocalPort { get; set; }
        public string Name { get; set; }
        public string Port { get; set; }
        public LoadBalancerProbe LoadBalancerProbe { get; set; }
        public string Protocol { get; set; }
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class DataVirtualHardDisk
    {
        public string HostCaching { get; set; }
        public string DiskLabel { get; set; }
        public string DiskName { get; set; }
        public string Lun { get; set; }
        public string LogicalDiskSizeInGB { get; set; }
        public string MediaLink { get; set; }
        public string SourceMediaLink { get; set; }
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class OsVirtualHardDisk
    {
        public string HostCaching { get; set; }
        public string DiskLabel { get; set; }
        public string DiskName { get; set; }
        public string MediaLink { get; set; }
        public string SourceImageName { get; set; }
        public string OS { get; set; }
        public string RemoteSourceImageLink { get; set; }
    }


    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************
    public class SubscriptionInfo
    {
        public string  CurrentCoreCount { get; set; }

        public string MaximumCoreCount { get; set; }


        public string SubscriptionID { get; set; }

        public string SubscriptionName { get; set; }
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public abstract class Role
    {
        /*public string RoleName { get; set; }
        public string RoleType { get; set; }
        public List<ConfigurationSet> ConfigurationSets { get; set; }
        public string AvailabilitySetName { get; set; }
        public List<DataVirtualHardDisk> DataVirtualHardDisks { get; set; }
        public OsVirtualHardDisk OSVirtualHardDisk { get; set; }
        public string RoleSize { get; set; }*/
    }

    [XmlInclude(typeof(WindowsProvisioningConfigurationSet)), XmlInclude(typeof(NetworkConfigurationSet))] 
    public class PersistentVMRole : Role
    {
        public string RoleName { get; set; }
        public string RoleType { get; set; }
        public List<ConfigurationSet> ConfigurationSets { get; set; }
        public string AvailabilitySetName { get; set; }
        public List<DataVirtualHardDisk> DataVirtualHardDisks { get; set; }
        public OsVirtualHardDisk OSVirtualHardDisk { get; set; }
        public string RoleSize { get; set; }
        public string ProvisionGuestAgent { get; set; }
        public string Label { get; set; }
        public List<ResourceExtensionReference> ResourceExtensionReferences { get; set; }

        /*<ResourceExtensionReferences>
            <ResourceExtensionReference>
              <ReferenceName>name-of-reference</ReferenceName>
              <Publisher>name-of-publisher</Publisher>
              <Name>name-of-extension</Name>
              <Version>version-of-extension</Version>
              <ResourceExtensionParameterValues>
                <ResourceExtensionParameterValue>
                  <Key>name-of-parameter-key</Key>
                  <Value>parameter-value</Value>
                  <Type>type-of-parameter</Type>
                </ResourceExtensionParameterValue>
              </ResourceExtensionParameterValues>
              <State>state-of-resource</State>
              <Certificates>
                <Certificate>
                  <Thumbprint>certificate-thumbprint</Thumbprint>
                  <ThumbprintAlgorithm>certificate-algorithm</ThumbprintAlgorithm>
                </Certificate>
              </Certificates>
            </ResourceExtensionReference>
        </ResourceExtensionReferences>

         * //This code came from "VirtualMachineUpdateParameters RoleToVmUpdateParams(Role role)" in VmOps.cs
         
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
            }*/
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class ResourceExtensionReference
    {
        public string Name { get; set; }
        public string Publisher { get; set; }
        public string ReferenceName { get; set; }
        public List<ResourceExtensionParameterValue> ResourceExtensionParameterValues { get; set; }
        public string State { get; set; }
        public string Version { get; set; }
        public List<Certificate> Certificates { get; set; }
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class Certificate
    {
        public string Thumbprint { get; set; }
        public string ThumbprintAlgorithm { get; set; }
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class ResourceExtensionParameterValue
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class DnsServer
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class DnsServerList
    {
        public List<DnsServer> DnsServers { get; set; }
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    [XmlInclude(typeof(PersistentVMRole))] 
    public class AzureVmDeployment
    {
        public string Name { get; set; }
        public string DeploymentSlot { get; set; }
        public string Label { get; set; }
        public List<Role> RoleList { get; set; }
        public string VirtualNetworkName { get; set; }
        public DnsServerList Dns { get; set; }
    }    
}

/*
<Deployment xmlns="http://schemas.microsoft.com/windowsazure" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
<Name>deployment-name</Name>
<DeploymentSlot>Staging|Production</DeploymentSlot>
<Label>deployment-label</Label>      
<RoleList>
<Role>
<RoleName>name-of-the-role</RoleName>
<RoleType>PersistentVMRole</RoleType>      
<ConfigurationSets>
<ConfigurationSet>
  <!—Include either a WindowsProvisioningConfigurationSet or a LinuxProvisioningConfigurationSet, but not both -->
  <ConfigurationSetType>WindowsProvisioningConfiguration</ConfigurationSetType>
  <ComputerName>computer-name</ComputerName>
  <AdminPassword>administrator-password-for-the-vm</AdminPassword> 
  <EnableAutomaticUpdates>true|false</EnableAutomaticUpdates>  
  <TimeZone>time-zone</TimeZone>
  <DomainJoin>
    <Credentials>
      <Domain>domain-to-join</Domain>
      <Username>user-name-in-the-domain</Username>
      <Password>password-for-the-user-name</Password>
    </Credentials>
    <JoinDomain>domain-to-join</JoinDomain>
    <MachineObjectOU>distinguished-name-of-the-ou</MachineObjectOU>
  </DomainJoin>
  <StoredCertificateSettings>
    <CertificateSetting>
      <StoreLocation>LocalMachine</StoreLocation>
      <StoreName>name-of-store-on-the-machine</StoreName>
      <Thumbprint>certificate-thumbprint</Thumbprint>
    </CertificateSetting>
  </StoredCertificateSettings>
  <WinRM>
    <Listeners>
      <Listener>
        <Protocol>Http</Protocol>
      </Listener>
      <Listener>
        <Protocol>Https</Protocol>
        <CertificateThumbprint>certificate-thumbprint</CertificateThumbprint>
      </Listener>
    </Listeners>
  </WinRM>
  <AdminUsername>name-of-administrator-account</AdminUsername>
</ConfigurationSet>
<!—Include either a WindowsProvisioningConfigurationSet or a LinuxProvisioningConfigurationSet, but not both -->
<ConfigurationSet> 
  <ConfigurationSetType>LinuxProvisioningConfiguration</ConfigurationSetType>
  <HostName>host-name-for-the-vm</HostName>
  <UserName>new-user-name</UserName> 
  <UserPassword>password-for-the-new-user</UserPassword> 
  <DisableSshPasswordAuthentication>true|false</DisableSshPasswordAuthentication>           
  <SSH>
    <PublicKeys>
      <PublicKey>
        <FingerPrint>certificate-fingerprint</FingerPrint>
        <Path>SSH-public-key-storage-location</Path>     
      </PublicKey>
    </PublicKeys>
    <KeyPairs>
      <KeyPair>
        <FingerPrint>certificate-fingerprint</FinguerPrint>
        <Path>SSH-public-key-storage-location</Path>
      </KeyPair>
    </KeyPairs>
  </SSH>
</ConfigurationSet>        
<ConfigurationSet> 
  <ConfigurationSetType>NetworkConfiguration</ConfigurationSetType>
  <InputEndpoints>
    <InputEndpoint>
      <LoadBalancedEndpointSetName>name-of-load-balanced-set</LoadBalancedEndpointSetName>
      <LocalPort>local-port-number</LocalPort>
      <Name>endpoint-name</Name>
      <Port>external-port-number</Port>
      <LoadBalancerProbe>
        <Path>path-of-probe</Path>
        <Port>port-assigned-to-probe</Port>
        <Protocol>TCP|UDP</Protocol>
        <IntervalInSeconds>interval-of-probe</IntervalInSeconds>
        <TimeoutInSeconds>timeout-for-probe</TimeoutInSeconds>
      </LoadBalancerProbe>
      <Protocol>TCP|UDP</Protocol>                    
    </InputEndpoint>
  </InputEndpoints>
  <SubnetNames>
    <SubnetName>name-of-subnet</SubnetName>
  </SubnetNames>        
</ConfigurationSet>
</ConfigurationSets>
<AvailabilitySetName>name-of-availability-set</AvailabilitySetName>
<DataVirtualHardDisks>
<DataVirtualHardDisk>
  <HostCaching>ReadOnly|ReadWrite</HostCaching> 
  <DiskLabel>data-disk-label</DiskLabel>            
  <DiskName>new-or-existing-disk-name</DiskName>
  <Lun>logical-unit-number</Lun>
  <LogicalDiskSizeInGB>size-in-gb-of-the-data-data-disk</LogicalDiskSizeInGB>            
  <MediaLink>url-of-the-blob-containing-the-data-disk</MediaLink>
</DataVirtualHardDisk>
</DataVirtualHardDisks>
<OSVirtualHardDisk>
<HostCaching>ReadOnly|ReadWrite</HostCaching>    
<DiskLabel>os-disk-label</DiskLabel>
<DiskName>new-or-existing-disk-name</DiskName>                    
<MediaLink>url-of-the-blob-containing-the-os-disk</MediaLink>
<SourceImageName>name-of-the-image-to-use-for-disk-creation</SourceImageName>
</OSVirtualHardDisk>      
<RoleSize>ExtraSmall|Small|Medium|Large|ExtraLarge</RoleSize>      
</Role>
</RoleList>
<VirtualNetworkName>MyProdictionNetwork</VirtualNetworkName>
<Dns>
<DnsServers>
<DnsServer>
<Name>dns-name</Name>
<Address>dns-ip-address</Address>
</DnsServer>
</DnsServers>
</Dns>
</Deployment>
*/
