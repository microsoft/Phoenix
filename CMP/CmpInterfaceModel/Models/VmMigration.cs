using System;
using System.Collections.Generic;

namespace CmpInterfaceModel.Models
{
    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public partial class VmMigrationRequest
    {
        public int ID { get; set; }
        public int VmDeploymentRequestID { get; set; }
        public string VmSize { get; set; }//public enum VmSizeEnum { VmSizeCustom, ExtraSmall, Small, Medium, Large, ExtraLarge, A6, A7 }
        public string TagData { get; set; }
        public int TagID { get; set; }
        public string Config { get; set; }
        public string TargetVmName { get; set; }
        public string SourceServerName { get; set; }
        public string SourceVhdFilesCSV { get; set; }
        public string ExceptionMessage { get; set; }
        public DateTime LastStatusUpdate { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string AgentRegion { get; set; }
        public string AgentName { get; set; }
        public DateTime CurrentStateStartTime { get; set; }
        public int CurrentStateTryCount { get; set; }
        public string Warnings { get; set; }
        public bool Active { get; set; }
    }

    //*************************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*************************************************************************

    public class MigrationConfig
    {
        public AzureVmDeployment AzureVmConfig { get; set; }
        public List<DiskSpec> DiskSpecList { get; set; }
        public PlacementSpec Placement { get; set; }
        public InfoFromVmSpec InfoFromVm { get; set; }
        public PostInfoFromVmSpec PostInfoFromVm { get; set; }
        public CmdbSpec CmdbConfig { get; set; }
        //public VmSetting MigrationVmSetting { get; set; }

        public ValidateSpec ValidationConfig { get; set; }

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
            return Utilities.Serialize(typeof(MigrationConfig), this);
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

        public static MigrationConfig Deserialize(string input)
        {
            try
            {
                return Utilities.DeSerialize(typeof(MigrationConfig), input, true) as MigrationConfig;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in MigrationConfig.Deserialize() : Unable to deserialize given VM Config structure, may be malformed : " 
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        public static MigrationConfig Deserialize(string input, bool UnwindException)
        {
            try
            {
                return Utilities.DeSerialize(typeof(MigrationConfig), input, UnwindException) as MigrationConfig;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in MigrationConfig.Deserialize() : Unable to deserialize given VM Config structure, may be malformed : "
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

        /*public static VmConfig CreateSample()
        {
            List<MigrationAgentRequest> VmDepReqList = new List<MigrationAgentRequest>();
            MigrationAgentRequest VmDepReq = new MigrationAgentRequest { ID = 1 };

            AzureVmDeployment VmDep = new AzureVmDeployment { Name = "TheName", 
                Label = "TheLabel", DeploymentSlot = "Production", VirtualNetworkName = "TheVM" };

            //*** 

            DataVirtualHardDisk DVD = new DataVirtualHardDisk { DiskLabel = "DL",
                                                                DiskName = "DN",
                                                                HostCaching = "HostCaching",
                                                                LogicalDiskSizeInGB = "100",
                                                                Lun = "1",
                                                                MediaLink = "ML"
            };
            List<DataVirtualHardDisk> DvdList = new List<DataVirtualHardDisk>();
            DvdList.Add(DVD);

            OsVirtualHardDisk OVD = new OsVirtualHardDisk { DiskLabel = "DL",
                                                            DiskName = "DN",
                                                            HostCaching = "HostCaching",
                                                            MediaLink = "ML",
                                                            SourceImageName = "SIN"
            };

            //***

            DnsServer DNS = new DnsServer { Name = "DnsName", Address = "DnsAddress" };
            VmDep.Dns = new DnsServerList();
            VmDep.Dns.DnsServers = new List<DnsServer>();
            VmDep.Dns.DnsServers.Add(DNS);

            //***

            Credentials Creds = new Credentials { Domain = "CredDomain", 
                Username = "CredUser", Password = "CredPassword" };
            DomainJoin DJ = new DomainJoin { JoinDomain = "TheDomain", 
                MachineObjectOU = "MOU", Credentials = Creds };

            CertificateSetting CertSet = new CertificateSetting { StoreLocation = "StoreLoc", 
                StoreName = "StoreName", Thumbprint = "TP" };
            List<CertificateSetting> CertSetList = new List<CertificateSetting>();
            CertSetList.Add(CertSet);

            //Listener List = new Listener { CertificateThumbprint = "CertThumb", Type = "T" };
            Listener List = new Listener { CertificateThumbprint = "CertThumb", Protocol = "Https" };
            List<Listener> ListList = new List<Listener>();
            ListList.Add(List);
            WinRM WRM = new WinRM { Listeners = ListList };

            ConfigurationSet CS = new WindowsProvisioningConfigurationSet { AdminPassword = "AP", 
                AdminUsername = "AUN", ComputerName = "CN", 
                ConfigurationSetType = "WindowsProvisioningConfiguration", 
                DomainJoin = DJ, EnableAutomaticUpdates = "True",
                StoredCertificateSettings = CertSetList,
                TimeZone = "TZ",
                WinRM = WRM
            };
            List<ConfigurationSet> CsList = new List<ConfigurationSet>();
            CsList.Add(CS);

            //***

            LoadBalancerProbe LBP = new LoadBalancerProbe { IntervalInSeconds = "100", 
                Path = "ThePath", Port = "4321", Protocol = "Prot", TimeoutInSeconds = "200" };
            InputEndpoint IE = new InputEndpoint { LoadBalancedEndpointSetName = "LBESN", 
                LoadBalancerProbe = LBP, LocalPort = "LP", Name = "N", Port = "1234", Protocol = "Prot" };
            List<InputEndpoint> IeList = new List<InputEndpoint>();
            IeList.Add(IE);

            SubnetNames SN = new SubnetNames { SubnetName = "XX" };
            List<SubnetNames> SubNameList = new List<SubnetNames>();
            SubNameList.Add(SN);

            //ConfigurationSet CS2 = new NetworkConfigurationSet { ConfigurationSetType = "NetworkConfiguration", 
            //    SubnetNames = SubNameList, InputEndpoints = IeList };
            //CsList.Add(CS2);

            //***

            Role R = new PersistentVMRole
            {
                AvailabilitySetName = "AsName",
                ConfigurationSets = CsList, 
                DataVirtualHardDisks = DvdList, OSVirtualHardDisk = OVD, RoleName = "RN", 
                RoleSize = "RS", RoleType = "RT" 
            }
            ;
            VmDep.RoleList = new List<Role>();
            VmDep.RoleList.Add(R);

            VmConfig VMC = new VmConfig { AzureVmConfig = VmDep };

            return VMC;
        }*/
    }
  
}

