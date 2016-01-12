//*****************************************************************************
//
// File:
// Author: Mark west (mark.west@microsoft.com)
//
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using CmpInterfaceModel;
using Microsoft.WindowsAzure.Management.Compute.Models;

namespace AzureAdminClientLib
{
    public class AzureResourceGroup
    {
        public string Id;
        public string Name;
        public string Location;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/windowsazure", IsNullable = false)]
    public partial class Deployment
    {

        private string nameField;

        private string deploymentSlotField;

        private string privateIDField;

        private string statusField;

        private string labelField;

        private string urlField;

        private string configurationField;

        private DeploymentRoleInstance[] roleInstanceListField;

        private byte upgradeDomainCountField;

        private DeploymentRole[] roleListField;

        private object sdkVersionField;

        private bool lockedField;

        private bool rollbackAllowedField;

        private string virtualNetworkNameField;

        private System.DateTime createdTimeField;

        private System.DateTime lastModifiedTimeField;

        private object extendedPropertiesField;

        private DeploymentPersistentVMDowntime persistentVMDowntimeField;

        private DeploymentVirtualIPs virtualIPsField;

        private string internalDnsSuffixField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string DeploymentSlot
        {
            get
            {
                return this.deploymentSlotField;
            }
            set
            {
                this.deploymentSlotField = value;
            }
        }

        /// <remarks/>
        public string PrivateID
        {
            get
            {
                return this.privateIDField;
            }
            set
            {
                this.privateIDField = value;
            }
        }

        /// <remarks/>
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public string Label
        {
            get
            {
                return this.labelField;
            }
            set
            {
                this.labelField = value;
            }
        }

        /// <remarks/>
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        public string Configuration
        {
            get
            {
                return this.configurationField;
            }
            set
            {
                this.configurationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("RoleInstance", IsNullable = false)]
        public DeploymentRoleInstance[] RoleInstanceList
        {
            get
            {
                return this.roleInstanceListField;
            }
            set
            {
                this.roleInstanceListField = value;
            }
        }

        /// <remarks/>
        public byte UpgradeDomainCount
        {
            get
            {
                return this.upgradeDomainCountField;
            }
            set
            {
                this.upgradeDomainCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Role", IsNullable = false)]
        public DeploymentRole[] RoleList
        {
            get
            {
                return this.roleListField;
            }
            set
            {
                this.roleListField = value;
            }
        }

        /// <remarks/>
        public object SdkVersion
        {
            get
            {
                return this.sdkVersionField;
            }
            set
            {
                this.sdkVersionField = value;
            }
        }

        /// <remarks/>
        public bool Locked
        {
            get
            {
                return this.lockedField;
            }
            set
            {
                this.lockedField = value;
            }
        }

        /// <remarks/>
        public bool RollbackAllowed
        {
            get
            {
                return this.rollbackAllowedField;
            }
            set
            {
                this.rollbackAllowedField = value;
            }
        }

        /// <remarks/>
        public string VirtualNetworkName
        {
            get
            {
                return this.virtualNetworkNameField;
            }
            set
            {
                this.virtualNetworkNameField = value;
            }
        }

        /// <remarks/>
        public System.DateTime CreatedTime
        {
            get
            {
                return this.createdTimeField;
            }
            set
            {
                this.createdTimeField = value;
            }
        }

        /// <remarks/>
        public System.DateTime LastModifiedTime
        {
            get
            {
                return this.lastModifiedTimeField;
            }
            set
            {
                this.lastModifiedTimeField = value;
            }
        }

        /// <remarks/>
        public object ExtendedProperties
        {
            get
            {
                return this.extendedPropertiesField;
            }
            set
            {
                this.extendedPropertiesField = value;
            }
        }

        /// <remarks/>
        public DeploymentPersistentVMDowntime PersistentVMDowntime
        {
            get
            {
                return this.persistentVMDowntimeField;
            }
            set
            {
                this.persistentVMDowntimeField = value;
            }
        }

        /// <remarks/>
        public DeploymentVirtualIPs VirtualIPs
        {
            get
            {
                return this.virtualIPsField;
            }
            set
            {
                this.virtualIPsField = value;
            }
        }

        /// <remarks/>
        public string InternalDnsSuffix
        {
            get
            {
                return this.internalDnsSuffixField;
            }
            set
            {
                this.internalDnsSuffixField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleInstance
    {

        private string roleNameField;

        private string instanceNameField;

        private string instanceStatusField;

        private byte instanceUpgradeDomainField;

        private byte instanceFaultDomainField;

        private string instanceSizeField;

        private object instanceStateDetailsField;

        private string ipAddressField;

        private string powerStateField;

        private string hostNameField;

        private string remoteAccessCertificateThumbprintField;

        private DeploymentRoleInstanceGuestAgentStatus guestAgentStatusField;

        private object resourceExtensionStatusListField;

        /// <remarks/>
        public string RoleName
        {
            get
            {
                return this.roleNameField;
            }
            set
            {
                this.roleNameField = value;
            }
        }

        /// <remarks/>
        public string InstanceName
        {
            get
            {
                return this.instanceNameField;
            }
            set
            {
                this.instanceNameField = value;
            }
        }

        /// <remarks/>
        public string InstanceStatus
        {
            get
            {
                return this.instanceStatusField;
            }
            set
            {
                this.instanceStatusField = value;
            }
        }

        /// <remarks/>
        public byte InstanceUpgradeDomain
        {
            get
            {
                return this.instanceUpgradeDomainField;
            }
            set
            {
                this.instanceUpgradeDomainField = value;
            }
        }

        /// <remarks/>
        public byte InstanceFaultDomain
        {
            get
            {
                return this.instanceFaultDomainField;
            }
            set
            {
                this.instanceFaultDomainField = value;
            }
        }

        /// <remarks/>
        public string InstanceSize
        {
            get
            {
                return this.instanceSizeField;
            }
            set
            {
                this.instanceSizeField = value;
            }
        }

        /// <remarks/>
        public object InstanceStateDetails
        {
            get
            {
                return this.instanceStateDetailsField;
            }
            set
            {
                this.instanceStateDetailsField = value;
            }
        }

        /// <remarks/>
        public string IpAddress
        {
            get
            {
                return this.ipAddressField;
            }
            set
            {
                this.ipAddressField = value;
            }
        }

        /// <remarks/>
        public string PowerState
        {
            get
            {
                return this.powerStateField;
            }
            set
            {
                this.powerStateField = value;
            }
        }

        /// <remarks/>
        public string HostName
        {
            get
            {
                return this.hostNameField;
            }
            set
            {
                this.hostNameField = value;
            }
        }

        /// <remarks/>
        public string RemoteAccessCertificateThumbprint
        {
            get
            {
                return this.remoteAccessCertificateThumbprintField;
            }
            set
            {
                this.remoteAccessCertificateThumbprintField = value;
            }
        }

        /// <remarks/>
        public DeploymentRoleInstanceGuestAgentStatus GuestAgentStatus
        {
            get
            {
                return this.guestAgentStatusField;
            }
            set
            {
                this.guestAgentStatusField = value;
            }
        }

        /// <remarks/>
        public object ResourceExtensionStatusList
        {
            get
            {
                return this.resourceExtensionStatusListField;
            }
            set
            {
                this.resourceExtensionStatusListField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleInstanceGuestAgentStatus
    {

        private decimal protocolVersionField;

        private System.DateTime timestampField;

        private string guestAgentVersionField;

        private string statusField;

        private DeploymentRoleInstanceGuestAgentStatusFormattedMessage formattedMessageField;

        /// <remarks/>
        public decimal ProtocolVersion
        {
            get
            {
                return this.protocolVersionField;
            }
            set
            {
                this.protocolVersionField = value;
            }
        }

        /// <remarks/>
        public System.DateTime Timestamp
        {
            get
            {
                return this.timestampField;
            }
            set
            {
                this.timestampField = value;
            }
        }

        /// <remarks/>
        public string GuestAgentVersion
        {
            get
            {
                return this.guestAgentVersionField;
            }
            set
            {
                this.guestAgentVersionField = value;
            }
        }

        /// <remarks/>
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public DeploymentRoleInstanceGuestAgentStatusFormattedMessage FormattedMessage
        {
            get
            {
                return this.formattedMessageField;
            }
            set
            {
                this.formattedMessageField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleInstanceGuestAgentStatusFormattedMessage
    {

        private string languageField;

        private string messageField;

        /// <remarks/>
        public string Language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
            }
        }

        /// <remarks/>
        public string Message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRole
    {

        private string roleNameField;

        private object osVersionField;

        private string roleTypeField;

        private DeploymentRoleConfigurationSets configurationSetsField;

        private DeploymentRoleResourceExtensionReferences resourceExtensionReferencesField;

        private DeploymentRoleDataVirtualHardDisk[] dataVirtualHardDisksField;

        private DeploymentRoleOSVirtualHardDisk oSVirtualHardDiskField;

        private string roleSizeField;

        private string defaultWinRmCertificateThumbprintField;

        private bool provisionGuestAgentField;

        /// <remarks/>
        public string RoleName
        {
            get
            {
                return this.roleNameField;
            }
            set
            {
                this.roleNameField = value;
            }
        }

        /// <remarks/>
        public object OsVersion
        {
            get
            {
                return this.osVersionField;
            }
            set
            {
                this.osVersionField = value;
            }
        }

        /// <remarks/>
        public string RoleType
        {
            get
            {
                return this.roleTypeField;
            }
            set
            {
                this.roleTypeField = value;
            }
        }

        /// <remarks/>
        public DeploymentRoleConfigurationSets ConfigurationSets
        {
            get
            {
                return this.configurationSetsField;
            }
            set
            {
                this.configurationSetsField = value;
            }
        }

        /// <remarks/>
        public DeploymentRoleResourceExtensionReferences ResourceExtensionReferences
        {
            get
            {
                return this.resourceExtensionReferencesField;
            }
            set
            {
                this.resourceExtensionReferencesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("DataVirtualHardDisk", IsNullable = false)]
        public DeploymentRoleDataVirtualHardDisk[] DataVirtualHardDisks
        {
            get
            {
                return this.dataVirtualHardDisksField;
            }
            set
            {
                this.dataVirtualHardDisksField = value;
            }
        }

        /// <remarks/>
        public DeploymentRoleOSVirtualHardDisk OSVirtualHardDisk
        {
            get
            {
                return this.oSVirtualHardDiskField;
            }
            set
            {
                this.oSVirtualHardDiskField = value;
            }
        }

        /// <remarks/>
        public string RoleSize
        {
            get
            {
                return this.roleSizeField;
            }
            set
            {
                this.roleSizeField = value;
            }
        }

        /// <remarks/>
        public string DefaultWinRmCertificateThumbprint
        {
            get
            {
                return this.defaultWinRmCertificateThumbprintField;
            }
            set
            {
                this.defaultWinRmCertificateThumbprintField = value;
            }
        }

        /// <remarks/>
        public bool ProvisionGuestAgent
        {
            get
            {
                return this.provisionGuestAgentField;
            }
            set
            {
                this.provisionGuestAgentField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleConfigurationSets
    {

        private DeploymentRoleConfigurationSetsConfigurationSet configurationSetField;

        /// <remarks/>
        public DeploymentRoleConfigurationSetsConfigurationSet ConfigurationSet
        {
            get
            {
                return this.configurationSetField;
            }
            set
            {
                this.configurationSetField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleConfigurationSetsConfigurationSet
    {

        private string configurationSetTypeField;

        private DeploymentRoleConfigurationSetsConfigurationSetSubnetNames subnetNamesField;

        private string staticVirtualNetworkIPAddressField;

        /// <remarks/>
        public string ConfigurationSetType
        {
            get
            {
                return this.configurationSetTypeField;
            }
            set
            {
                this.configurationSetTypeField = value;
            }
        }

        /// <remarks/>
        public DeploymentRoleConfigurationSetsConfigurationSetSubnetNames SubnetNames
        {
            get
            {
                return this.subnetNamesField;
            }
            set
            {
                this.subnetNamesField = value;
            }
        }

        /// <remarks/>
        public string StaticVirtualNetworkIPAddress
        {
            get
            {
                return this.staticVirtualNetworkIPAddressField;
            }
            set
            {
                this.staticVirtualNetworkIPAddressField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleConfigurationSetsConfigurationSetSubnetNames
    {

        private string subnetNameField;

        /// <remarks/>
        public string SubnetName
        {
            get
            {
                return this.subnetNameField;
            }
            set
            {
                this.subnetNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleResourceExtensionReferences
    {

        private DeploymentRoleResourceExtensionReferencesResourceExtensionReference resourceExtensionReferenceField;

        /// <remarks/>
        public DeploymentRoleResourceExtensionReferencesResourceExtensionReference ResourceExtensionReference
        {
            get
            {
                return this.resourceExtensionReferenceField;
            }
            set
            {
                this.resourceExtensionReferenceField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleResourceExtensionReferencesResourceExtensionReference
    {

        private string referenceNameField;

        private string publisherField;

        private string nameField;

        private string versionField;

        private object resourceExtensionParameterValuesField;

        private string stateField;

        /// <remarks/>
        public string ReferenceName
        {
            get
            {
                return this.referenceNameField;
            }
            set
            {
                this.referenceNameField = value;
            }
        }

        /// <remarks/>
        public string Publisher
        {
            get
            {
                return this.publisherField;
            }
            set
            {
                this.publisherField = value;
            }
        }

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        public object ResourceExtensionParameterValues
        {
            get
            {
                return this.resourceExtensionParameterValuesField;
            }
            set
            {
                this.resourceExtensionParameterValuesField = value;
            }
        }

        /// <remarks/>
        public string State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleDataVirtualHardDisk
    {

        private string hostCachingField;

        private string diskLabelField;

        private string diskNameField;

        private byte lunField;

        private ushort logicalDiskSizeInGBField;

        private string mediaLinkField;

        /// <remarks/>
        public string HostCaching
        {
            get
            {
                return this.hostCachingField;
            }
            set
            {
                this.hostCachingField = value;
            }
        }

        /// <remarks/>
        public string DiskLabel
        {
            get
            {
                return this.diskLabelField;
            }
            set
            {
                this.diskLabelField = value;
            }
        }

        /// <remarks/>
        public string DiskName
        {
            get
            {
                return this.diskNameField;
            }
            set
            {
                this.diskNameField = value;
            }
        }

        /// <remarks/>
        public byte Lun
        {
            get
            {
                return this.lunField;
            }
            set
            {
                this.lunField = value;
            }
        }

        /// <remarks/>
        public ushort LogicalDiskSizeInGB
        {
            get
            {
                return this.logicalDiskSizeInGBField;
            }
            set
            {
                this.logicalDiskSizeInGBField = value;
            }
        }

        /// <remarks/>
        public string MediaLink
        {
            get
            {
                return this.mediaLinkField;
            }
            set
            {
                this.mediaLinkField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentRoleOSVirtualHardDisk
    {

        private string hostCachingField;

        private string diskNameField;

        private string mediaLinkField;

        private string sourceImageNameField;

        private string osField;

        /// <remarks/>
        public string HostCaching
        {
            get
            {
                return this.hostCachingField;
            }
            set
            {
                this.hostCachingField = value;
            }
        }

        /// <remarks/>
        public string DiskName
        {
            get
            {
                return this.diskNameField;
            }
            set
            {
                this.diskNameField = value;
            }
        }

        /// <remarks/>
        public string MediaLink
        {
            get
            {
                return this.mediaLinkField;
            }
            set
            {
                this.mediaLinkField = value;
            }
        }

        /// <remarks/>
        public string SourceImageName
        {
            get
            {
                return this.sourceImageNameField;
            }
            set
            {
                this.sourceImageNameField = value;
            }
        }

        /// <remarks/>
        public string OS
        {
            get
            {
                return this.osField;
            }
            set
            {
                this.osField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentPersistentVMDowntime
    {

        private System.DateTime startTimeField;

        private System.DateTime endTimeField;

        private string statusField;

        /// <remarks/>
        public System.DateTime StartTime
        {
            get
            {
                return this.startTimeField;
            }
            set
            {
                this.startTimeField = value;
            }
        }

        /// <remarks/>
        public System.DateTime EndTime
        {
            get
            {
                return this.endTimeField;
            }
            set
            {
                this.endTimeField = value;
            }
        }

        /// <remarks/>
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentVirtualIPs
    {

        private DeploymentVirtualIPsVirtualIP virtualIPField;

        /// <remarks/>
        public DeploymentVirtualIPsVirtualIP VirtualIP
        {
            get
            {
                return this.virtualIPField;
            }
            set
            {
                this.virtualIPField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class DeploymentVirtualIPsVirtualIP
    {

        private string addressField;

        private bool isDnsProgrammedField;

        private string nameField;

        /// <remarks/>
        public string Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        public bool IsDnsProgrammed
        {
            get
            {
                return this.isDnsProgrammedField;
            }
            set
            {
                this.isDnsProgrammedField = value;
            }
        }

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }



    //*************************************************************************
    //*************************************************************************

    //*************************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*************************************************************************

    public class HostedServiceInfo
    {
        public string ServiceName;
        public string LabelB64;
        public string Description;
        public string Location;

        public string LabelText 
        {
            set{LabelB64 = Util.ToB64(value);}
            get{ return Util.FromB64(LabelB64);}
        }
    }

    public interface IHostedServiceOps
    {
        AzureAdminClientLib.HttpResponse GetHostedServiceList();
        AzureAdminClientLib.HttpResponse GetServiceDeploymentList(string serviceName, string slotName, out List<string> deploymentNames);
        AzureAdminClientLib.HttpResponse GetServiceDeploymentInstanceList(string serviceName, string slotName, out List<string> deploymentNames);
        AzureAdminClientLib.HttpResponse GetServiceDeployment(string serviceName, string deployment);
        HostedServiceOps.ServiceAvailabilityEnum CheckAvailability(string name,
            int deploymentCountLimit, out string affinityGroupName);
        AzureAdminClientLib.HttpResponse CreateHostedService(HostedServiceInfo hsInfo);
        AzureAdminClientLib.HttpResponse CreateHostedService(string config);
    }

    //*************************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*************************************************************************

    public class HostedServiceOps : IHostedServiceOps
    {
        public enum ServiceAvailabilityEnum { Unknown, Available, Unavailable, AlredayOwnIt, DepolymentLimitFull }

        const string URLTEMPLATE_GETHSLIST = "https://management.core.windows.net/{0}/services/hostedservices";
        const string URLTEMPLATE_GETDEPOLYMENTLIST = "https://management.core.windows.net/{0}/services/hostedservices/{1}/deploymentslots/{2}";
        const string URLTEMPLATE_GETDEPOLYMENT = "https://management.core.windows.net/{0}/services/hostedservices/{1}/deployments/{2}";
        const string URLTEMPLATE_CREATEHS = "https://management.core.windows.net/{0}/services/hostedservices";
        const string URLTEMPLATE_ISAVAIL = "https://management.core.windows.net/{0}/services/hostedservices/operations/isavailable/{1}";

        const string URLTEMPLATE_GETRGLIST = "https://management.azure.com/subscriptions/{0}/resourcegroups?api-version={1}";
        const string URLTEMPLATE_FETCHRG = "https://management.azure.com/subscriptions/{0}/resourcegroups/{1}?api-version={2}";
        const string URLTEMPLATE_CREATERG = "https://management.azure.com/subscriptions/{0}/resourcegroups/{1}?api-version={2}";
        
        const string BODYTEMPLATE_CREATEHS =
            "<?xml version='1.0' encoding='utf-8'?><CreateHostedService xmlns='http://schemas.microsoft.com/windowsazure'><ServiceName>{ServiceName}</ServiceName><Label>{LabelB64}</Label><Description>{Description}</Description><Location>{Location}</Location></CreateHostedService>";

        private IConnection Connection { get; set; }
        private readonly XNamespace _azureNameSpace = "http://schemas.microsoft.com/windowsazure";
        private const string ARM_API_VERSION = "2015-01-01";

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// 
        //*********************************************************************

        public HostedServiceOps(IConnection connection)
        {
            Connection = connection;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureAdminClientLib.HttpResponse GetHostedServiceList()
        {
            var url = string.Format(URLTEMPLATE_GETHSLIST, Connection.SubcriptionID);
            var hi = new HttpInterface(Connection);
            return hi.PerformRequest(HttpInterface.RequestType_Enum.GET, url, null);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureAdminClientLib.HttpResponse GetResourceGroupList()
        {
            var url = string.Format(URLTEMPLATE_GETRGLIST, 
                Connection.SubcriptionID, ARM_API_VERSION);
            var hi = new HttpInterface(Connection);

            return hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url, null);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rgName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureAdminClientLib.HttpResponse GetResourceGroup(string rgName)
        {
            var url = string.Format(URLTEMPLATE_FETCHRG, 
                Connection.SubcriptionID, rgName, ARM_API_VERSION);
            var hi = new HttpInterface(Connection);

            var ret = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url, null);

            if (ret.Body.Contains("(404) Not Found"))
                return null;

            return ret;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="slotName"></param>
        /// <param name="deploymentNames"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureAdminClientLib.HttpResponse GetServiceDeploymentList(
            string serviceName, string slotName, out List<string> deploymentNames)
        {
            var url = string.Format(URLTEMPLATE_GETDEPOLYMENTLIST, 
                Connection.SubcriptionID, serviceName, slotName);
            var hi = new HttpInterface(Connection);
            var resp = hi.PerformRequest(HttpInterface.RequestType_Enum.GET, url, null);

            if (resp.HadError)
            {
                deploymentNames = null;
            }
            else
            {
                deploymentNames = new List<string>();
                var deploymentName = Utilities.GetXmlInnerText(resp.Body, "Name");
                deploymentNames.Add(deploymentName);
            }

            return resp;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="slotName"></param>
        /// <param name="deploymentNames"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureAdminClientLib.HttpResponse GetServiceDeploymentInstanceList(string serviceName, string slotName, out List<string> deploymentNames)
        {
            var url = string.Format(URLTEMPLATE_GETDEPOLYMENTLIST, Connection.SubcriptionID, serviceName, slotName);
            var hi = new HttpInterface(Connection);
            var resp = hi.PerformRequest(HttpInterface.RequestType_Enum.GET, url, null);

            if (resp.HadError)
            {
                deploymentNames = null;
            }
            else
            {
                deploymentNames = new List<string>();

                var doc = new XmlDocument();
                doc.LoadXml(resp.Body);

                if (null == doc.DocumentElement)
                    return new HttpResponse { Body = "No document element on response", HadError = true };

                var cn = doc.DocumentElement.ChildNodes;

                foreach (XmlNode rootNode in cn)
                    if (rootNode.Name.Equals("RoleInstanceList"))
                        foreach (XmlNode roleInstance in rootNode.ChildNodes)
                            deploymentNames.Add(Utilities.GetXmlInnerText(roleInstance.InnerXml,"RoleName"));
            }

            return resp;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="deployment"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureAdminClientLib.HttpResponse GetServiceDeployment(string serviceName, string deployment)
        {
            var url = string.Format(URLTEMPLATE_GETDEPOLYMENT, Connection.SubcriptionID, serviceName, deployment);
            var hi = new HttpInterface(Connection);
            return hi.PerformRequest(HttpInterface.RequestType_Enum.GET, url, null);
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="name"></param>
        /// <param name="deploymentCountLimit"></param>
        /// <param name="affinityGroupName"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        public ServiceAvailabilityEnum CheckAvailability( string name, 
            int deploymentCountLimit, out string affinityGroupName)
        {
            //*** ARM Support ***
            if (null != Connection.AdToken)
                return CheckResourceGroupAvailability(name,
                    deploymentCountLimit, out affinityGroupName);

            affinityGroupName = null;
            var url = string.Format(URLTEMPLATE_ISAVAIL, Connection.SubcriptionID, name);
            var hi = new HttpInterface(Connection);
            var hr = hi.PerformRequest(HttpInterface.RequestType_Enum.GET, url, null);

            var result = Utilities.GetXmlInnerText(hr.Body, "Result");

            if (result.Equals("false"))
            {
                var hr2 = GetHostedServiceList();

                if (hr2.Body.Contains("<ServiceName>" + name + "</ServiceName>"))
                {
                    List<string> deploymentNameList;
                    GetServiceDeploymentInstanceList(name, "production", out deploymentNameList);


                    if (null != deploymentNameList) if (deploymentCountLimit < deploymentNameList.Count+1)
                        return ServiceAvailabilityEnum.DepolymentLimitFull;

                    var hostedServiceDoc = XDocument.Parse(hr2.Body);

                    var hostedServiceList = 
                        from item in hostedServiceDoc.Descendants(this._azureNameSpace + "HostedService")
                            select new
                            {
                                Name = item.Element(this._azureNameSpace + "ServiceName"),
                                HostedServiceProperties = item.Element(this._azureNameSpace + "HostedServiceProperties"),
                            };

                    foreach (var hostedService in hostedServiceList)
                        if (hostedService.Name != null)
                            if (hostedService.Name.Value.Equals(name,StringComparison.InvariantCultureIgnoreCase))
                                if (hostedService.HostedServiceProperties.Element(this._azureNameSpace + "AffinityGroup") != null)
                                    affinityGroupName =
                                        hostedService.HostedServiceProperties.Element(this._azureNameSpace + "AffinityGroup")
                                            .Value;

                    return ServiceAvailabilityEnum.AlredayOwnIt;
                }

                return ServiceAvailabilityEnum.Unavailable;
            }

            return ServiceAvailabilityEnum.Available;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="deploymentCountLimit"></param>
        /// <param name="affinityGroupName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public ServiceAvailabilityEnum CheckResourceGroupAvailability(string name,
            int deploymentCountLimit, out string affinityGroupName)
        {
            //*** TODO * markwes * check if resgroup name is available (not taken)
            //*** TODO * markwes * check if deploymentCountLimit is exceeded

            affinityGroupName = null;

            var rgList = GetResourceGroupList();
            var resGrouplist = Utilities.FetchJsonValue(rgList.Body, "value") as Newtonsoft.Json.Linq.JArray;

            if(null != resGrouplist)
                foreach (var resGroup in resGrouplist)
                {
                    var foundName = Utilities.FetchJsonValue(resGroup.ToString(), "name") as string;
                    if (null == foundName) continue;
                    if(foundName.Equals(name))
                        return ServiceAvailabilityEnum.AlredayOwnIt;
                }

            return ServiceAvailabilityEnum.Available;
        }

        public List<AzureResourceGroup> FetchResourceGroupList()
        {
            // https://msdn.microsoft.com/en-us/library/azure/dn790529.aspx

            var resourceGroups = new List<AzureResourceGroup>();

            try
            {
                var rgList = GetResourceGroupList();
                var resGrouplist = Utilities.FetchJsonValue(rgList.Body, "value") as Newtonsoft.Json.Linq.JArray;

                if (null != resGrouplist)
                    foreach (var resGroup in resGrouplist)
                    {
                        var foundName = Utilities.FetchJsonValue(resGroup.ToString(), "name") as string;

                        resourceGroups.Add(new AzureResourceGroup()
                        {
                            Id = Utilities.FetchJsonValue(resGroup.ToString(), "id") as string,
                            Name = Utilities.FetchJsonValue(resGroup.ToString(), "name") as string,
                            Location = Utilities.FetchJsonValue(resGroup.ToString(), "location") as string,
                        });
                    }

                return resourceGroups;
            }
            catch (Exception ex)
            {
                throw new Exception("excpetion in FetchResourceGroupList() : " + ex.Message);
            }
        }


        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hsInfo"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureAdminClientLib.HttpResponse CreateHostedService(HostedServiceInfo hsInfo)
        {
            //*** ARM Support ***
            if (null != Connection.AdToken)
                return CreateResourceGroup(hsInfo.ServiceName, hsInfo.Location, false);

            var url = string.Format(URLTEMPLATE_CREATEHS, Connection.SubcriptionID);
            var body = string.Copy(BODYTEMPLATE_CREATEHS);

            body = body.Replace("{ServiceName}", hsInfo.ServiceName);
            body = body.Replace("{LabelB64}", hsInfo.LabelB64);
            body = body.Replace("{Description}", hsInfo.Description);
            body = body.Replace("{Location}", hsInfo.Location);

            var hi = new HttpInterface(Connection);
            return hi.PerformRequest(HttpInterface.RequestType_Enum.POST, url, body);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureAdminClientLib.HttpResponse CreateHostedService(string config)
        {
            try
            {
                //*** ARM Support ***
                if (null != Connection.AdToken)
                {
                    var serviceName = Utilities.GetXmlInnerText(config,"ServiceName");
                    var location = Utilities.GetXmlInnerText(config, "Location");

                    if (null == serviceName)
                        throw new Exception("Missing 'ServiceName' value in given config");
                    if (null == location)
                        throw new Exception("Missing 'Location' value in given config");

                    return CreateResourceGroup(serviceName, location, false);
                }

                var url = string.Format(URLTEMPLATE_CREATEHS, Connection.SubcriptionID);
                var hi = new HttpInterface(Connection);
                var resp = 
                    hi.PerformRequest(HttpInterface.RequestType_Enum.POST, url, config);

                if (resp.HadError)
                {
                    var sn = Utilities.GetXmlInnerText(config, "ServiceName");
                    resp.Body = "Error while creating Azure Hosted Service '" + sn + "' : " + resp.Body;
                }

                return resp;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in HostedServiceOps.CreateHostedService() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="rgName"></param>
        ///  <param name="location"></param>
        /// <param name="throwIfExists"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************
        public AzureAdminClientLib.HttpResponse CreateResourceGroup(string rgName, 
            string location, bool throwIfExists)
        {
            try
            {
                var resp = GetResourceGroup(rgName);

                if (null != resp)
                {
                    if (throwIfExists)
                        throw new Exception("Requested resource group already exists in given subscription");
                    else
                        return resp;
                }

                var url = string.Format(URLTEMPLATE_CREATERG, 
                    Connection.SubcriptionID, rgName, ARM_API_VERSION);
                var body = "{\"location\": \"" + location + "\"}";

                var hi = new HttpInterface(Connection);
                resp = hi.PerformRequestArm(HttpInterface.RequestType_Enum.PUT, url, body);

                if (resp.HadError)
                {
                    resp.Body = "Error while creating Azure Hosted Service '" + rgName + "' : " + resp.Body;
                }

                return resp;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in HostedServiceOps.CreateResourceGroup() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="roleSize"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static bool IsRoleSizeSupported(
            HostedServiceListResponse.HostedService service, string roleSize)
        {
            return service.ComputeCapabilities.VirtualMachinesRoleSizes.Any(
                rs => rs.Equals(roleSize, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
