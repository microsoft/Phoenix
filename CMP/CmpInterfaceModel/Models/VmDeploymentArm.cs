using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;

namespace CmpInterfaceModel.Models
{
    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class AzureVmDeploymentArm
    {
        public ArmRootProperties properties { get; set; }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inVal"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        static string CleanupFormat(string inVal)
        {
            var outVal = inVal.Replace(@"\/", @"/");
            return outVal.Replace(@"""schema"":", @"""$schema"":");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public string SerializeJson()
        {
            return CleanupFormat(Utilities.SerializeJson<AzureVmDeploymentArm>(this));
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

        public static AzureVmDeploymentArm DeserializeJson(string input)
        {
            try
            {
                return Utilities.DeSerializeJson<AzureVmDeploymentArm>(input);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmConfig.Deserialize() : Unable to deserialize given VM Config structure, may be malformed : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmRootProperties
    {
        public string mode { get; set; }
        public ArmTemplate template { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmTemplate
    {
        public string schema { get; set; }
        public string contentVersion { get; set; }
        //public string mode { get; set; }
        //public ArmParameters parameters { get; set; }
        public ArmVariables variables { get; set; }
        public ArmResource[] resources { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    /*public class ArmParameters
    {
        public ArmNewstorageaccountname newStorageAccountName { get; set; }
        public ArmAdminusername adminUsername { get; set; }
        public ArmAdminpassword adminPassword { get; set; }
        public ArmDnsnameforpublicip dnsNameForPublicIP { get; set; }
        public ArmWindowsosversion windowsOSVersion { get; set; }
    }*/

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmNewstorageaccountname
    {
        public string value { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmAdminusername
    {
        public string value { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmAdminpassword
    {
        public string value { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmDnsnameforpublicip
    {
        public string value { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmWindowsosversion
    {
        public string value { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmVariables
    {
        public string location { get; set; }
        public string imagePublisher { get; set; }
        public string imageOffer { get; set; }
        public string OSDiskName { get; set; }
        public string nicName { get; set; }
        public string addressPrefix { get; set; }
        public string subnetName { get; set; }
        public string subnetPrefix { get; set; }
        public string storageAccountType { get; set; }
        public string publicIPAddressName { get; set; }
        public string publicIPAddressType { get; set; }
        public string vmStorageAccountContainerName { get; set; }
        public string vmName { get; set; }
        public string vmSize { get; set; }
        public string virtualNetworkName { get; set; }
        public string vnetID { get; set; }
        public string subnetRef { get; set; }

        //***

        public string newStorageAccountName { get; set; }
        public string adminUsername { get; set; }
        public string adminPassword { get; set; }
        public string dnsNameForPublicIP { get; set; }
        public string windowsOSVersion { get; set; }
        public string nsgName { get; set; }
        public string nsgId { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    [DataContract]
    public class ArmResource
    {
        [DataMember(EmitDefaultValue = false)]
        public string type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string apiVersion { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string location { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ArmTags tags { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ArmResourceProperties properties { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string[] dependsOn { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmTags
    {
        public string displayName { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public interface ArmResourcePropertiesInt
    { }

    public class ArmResourcePropertiesStorageAccount : ArmResourcePropertiesInt
    {
        public string accountType { get; set; }
    }

    public class ArmResourcePropertiesPublicIPAddress : ArmResourcePropertiesInt
    {
        public string publicIPAllocationMethod { get; set; }
        public ArmDnssettings dnsSettings { get; set; }
    }

    public class ArmResourcePropertiesVirtualNetwork : ArmResourcePropertiesInt
    {
        public ArmAddressspace addressSpace { get; set; }
        public ArmSubnet[] subnets { get; set; }
    }

    public class ArmResourcePropertiesNetworkInterface : ArmResourcePropertiesInt
    {
        public ArmIpconfiguration[] ipConfigurations { get; set; }
    }

    public class ArmResourcePropertiesVirtualMachine : ArmResourcePropertiesInt
    {
        public ArmHardwareprofile hardwareProfile { get; set; }
        public ArmOsprofile osProfile { get; set; }
        public ArmStorageprofile storageProfile { get; set; }
        public ArmNetworkprofile networkProfile { get; set; }
    }

    [DataContract]
    public class ArmResourceProperties
    {
        [DataMember(EmitDefaultValue = false)]
        public string accountType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string publicIPAllocationMethod { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ArmDnssettings dnsSettings { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ArmAddressspace addressSpace { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ArmSubnet[] subnets { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ArmIpconfiguration[] ipConfigurations { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Securityrule[] securityRules { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ArmHardwareprofile hardwareProfile { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ArmOsprofile osProfile { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ArmStorageprofile storageProfile { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ArmNetworkprofile networkProfile { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmDnssettings
    {
        public string domainNameLabel { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmAddressspace
    {
        public string[] addressPrefixes { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmHardwareprofile
    {
        public string vmSize { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmOsprofile
    {
        public string computername { get; set; }
        public string adminUsername { get; set; }
        public string adminPassword { get; set; }
        public string customData { get; set; }
        public Windowsconfiguration windowsConfiguration { get; set; }
        public Secret[] secrets { get; set; }
    }

    public class Windowsconfiguration
    {
        public bool provisionVMAgent { get; set; }
        public Winrm winRM { get; set; }
        public Additionalunattendcontent additionalUnattendContent { get; set; }
        public bool enableAutomaticUpdates { get; set; }
    }

    public class Winrm
    {
        public Listener[] listeners { get; set; }
    }

    public class Additionalunattendcontent
    {
        public string pass { get; set; }
        public string component { get; set; }
        public string settingName { get; set; }
        public string content { get; set; }
    }

    public class Secret
    {
        public Sourcevault sourceVault { get; set; }
        public Vaultcertificate[] vaultCertificates { get; set; }
    }

    public class Sourcevault
    {
        public string id { get; set; }
    }

        public class Vaultcertificate
    {
        public string certificateUrl { get; set; }
        public string certificateStore { get; set; }
    }
    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmStorageprofile
    {
        public ArmStorageprofileImagereference imageReference { get; set; }
        public ArmStorageprofileOsdisk osDisk { get; set; }
        public ArmStorageprofileDatadisk[] dataDisks { get; set; }
    }

    public class ArmStorageprofileDatadisk
    {
        public string name { get; set; }
        public string diskSizeGB { get; set; }
        public int lun { get; set; }
        public ArmStorageprofileDatadiskVhd1 vhd { get; set; }
        public string createOption { get; set; }
    }

    public class ArmStorageprofileDatadiskVhd1
    {
        public string uri { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmStorageprofileImagereference
    {
        public string publisher { get; set; }
        public string offer { get; set; }
        public string sku { get; set; }
        public string version { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmStorageprofileOsdisk
    {
        public string name { get; set; }
        public ArmStorageprofileOsdiskVhd vhd { get; set; }
        public string caching { get; set; }
        public string createOption { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmStorageprofileOsdiskVhd
    {
        public string uri { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmNetworkprofile
    {
        public ArmNetworkprofileNetworkinterface[] networkInterfaces { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmNetworkprofileNetworkinterface
    {
        public string id { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmSubnet
    {
        public string name { get; set; }
        public ArmSubnetProperties properties { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmSubnetProperties
    {
        public string addressPrefix { get; set; }
        public ArmNetworkSecurityGroup networksecuritygroup { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmNetworkSecurityGroup
    {
        public string id { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmIpconfiguration
    {
        public string name { get; set; }
        public ArmIpconfigurationProperties properties { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmIpconfigurationProperties
    {
        public string privateIPAllocationMethod { get; set; }
        public ArmIpconfigurationPropertiesPublicipaddress publicIPAddress { get; set; }
        public ArmIpconfigurationPropertiesSubnet subnet { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmIpconfigurationPropertiesPublicipaddress
    {
        public string id { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class ArmIpconfigurationPropertiesSubnet
    {
        public string id { get; set; }
    }

    //*************************************************************************
    //*************************************************************************
    //*************************************************************************

    public class AzureVmDeploymentResponseArm
    {
        public string id { get; set; }
        public string name { get; set; }
        public AzureVmDeploymentResponseArmProperties properties { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class AzureVmDeploymentResponseArmProperties
    {
        public string mode { get; set; }
        public string provisioningState { get; set; }
        public DateTime timestamp { get; set; }
        public string duration { get; set; }
        public string correlationId { get; set; }
        public AzureVmDeploymentResponseArmProvider[] providers { get; set; }
        public AzureVmDeploymentResponseArmDependency[] dependencies { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class AzureVmDeploymentResponseArmProvider
    {
        public string _namespace { get; set; }
        public AzureVmDeploymentResponseArmResourcetype[] resourceTypes { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class AzureVmDeploymentResponseArmResourcetype
    {
        public string resourceType { get; set; }
        public string[] locations { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class AzureVmDeploymentResponseArmDependency
    {
        public AzureVmDeploymentResponseArmDependson[] dependsOn { get; set; }
        public string id { get; set; }
        public string resourceType { get; set; }
        public string resourceName { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class AzureVmDeploymentResponseArmDependson
    {
        public string id { get; set; }
        public string resourceType { get; set; }
        public string resourceName { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class Securityrule
    {
        public string name { get; set; }
        public SecurityruleProperties properties { get; set; }
    }

    //*********************************************************************
    /// <summary>
    /// 
    /// </summary>
    //*********************************************************************

    public class SecurityruleProperties
    {
        public string description { get; set; }
        public string protocol { get; set; }
        public string sourcePortRange { get; set; }
        public string destinationPortRange { get; set; }
        public string sourceAddressPrefix { get; set; }
        public string destinationAddressPrefix { get; set; }
        public string access { get; set; }
        public int priority { get; set; }
        public string direction { get; set; }
    }
}
