using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    /// <summary>
    /// Represents a CMP VM build request to build a VM
    /// </summary>
    /* todo: refactor this class */
    public class CmpVmBuildRequest
    {
        /// <summary>
        /// Order ID for the CMP build request
        /// </summary>
        public int OrderID { get; set; }
        
        /// <summary>
        /// ID for the CMP build request
        /// </summary>
        public int RequestID { get; set; }
        
        /// <summary>
        /// Request name for the CMP build request
        /// </summary>
        public string RequestName { get; set; }
        
        //public string RequestAdmins { get; set; }

        /// <summary>
        /// Name for the VM admin
        /// </summary>
        public string VmAdminName { get; set; }
     
        public string VmAdminPassword { get; set; }

        /// <summary>
        /// Resource group for the VM to be created
        /// </summary>
        public string ResourceGroup { get; set; }

        /// <summary>
        /// Request status code the VM to be created
        /// </summary>
        public string RequestStatusCode { get; set; }

        /// <summary>
        /// Name of status for the VM to be created
        /// </summary>
        public string RequestStatusName { get; set; }

        /// <summary>
        /// Type code for the VM to be created
        /// </summary>
        public string RequestTypeCode { get; set; }

        /// <summary>
        /// Name of the request type for the VM to be created
        /// </summary>
        public string RequestTypeName { get; set; }
        
        /// <summary>
        /// Type code of the system type  for the VM to be created
        /// </summary>
        public string SystemTypeCode { get; set; }

        /// <summary>
        /// Name of the system type for the VM to be created
        /// </summary>
        public string SystemTypeName { get; set; }

        /// <summary>
        /// Role code for the VM to be created
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// Role name for the VM to be created
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Environment code for the VM to be created
        /// </summary>
        public string EnvironmentCode { get; set; }

        /// <summary>
        /// Name of the environment for the VM to be created
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        /// ID of the datacenter for the VM to be created
        /// </summary>
        public string DatacenterID { get; set; }

        /// <summary>
        /// Name of the datacenter for the VM to be created
        /// </summary>
        public string DatacenterName { get; set; }

        /// <summary>
        /// ID of the VBOS location for the VM to be created
        /// </summary>
        public string VBOSLocationID { get; set; }

        /// <summary>
        /// Name of the VBOS location for the VM to be created
        /// </summary>
        public string VBOSLocationName { get; set; }

        /// <summary>
        /// Asset ID for the VM to be created
        /// </summary>
        public string AssetID { get; set; }

        /// <summary>
        /// Customer name SKU for the VM to be created
        /// </summary>
        public string SKU_CustomerName { get; set; }

        /// <summary>
        /// Name of the GFS SKU for the VM to be created
        /// </summary>
        public string SKU_GFSName { get; set; }

        /// <summary>
        /// Proc SKU for the VM to be created
        /// </summary>
        public Nullable<int> SKU_Procs { get; set; }

        /// <summary>
        /// Memory SKU for the VM to be created
        /// </summary>
        public Nullable<int> SKU_Memory { get; set; }

        /// <summary>
        /// ID of the template for the VM to be created
        /// </summary>
        public int TemplateID { get; set; }

        /// <summary>
        /// Name of the template for the VM to be created
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Template SLA for the VM to be created
        /// </summary>
        public Nullable<int> TemplateSLA { get; set; }

        /// <summary>
        /// Type code of the template for the VM to be created
        /// </summary>
        public string TemplateTypeCode { get; set; }

        /// <summary>
        /// Name of the template type for the VM to be created
        /// </summary>
        public string TemplateTypeName { get; set; }

        /// <summary>
        /// Operating system code for the VM to be created
        /// </summary>
        public string OSCode { get; set; }

        /// <summary>
        /// Name of the operating system for the VM to be created
        /// </summary>
        public string OSName { get; set; }

        /// <summary>
        /// ID of the timezone for the VM to be created
        /// </summary>
        public string TimeZoneID { get; set; }

        /// <summary>
        /// Name of the timezone for the VM to be created
        /// </summary>
        public string TimeZoneName { get; set; }

        /// <summary>
        /// UTC Offset of the timezone for the VM to be created
        /// </summary>
        public string TimeZoneUTCOffset { get; set; }

        /// <summary>
        /// Active directory domain for the VM to be created
        /// </summary>
        public string ActiveDirectoryDomain { get; set; }

        /// <summary>
        /// Name of the cluster for the VM to be created
        /// </summary>
        public string ClusterName { get; set; }

        /// <summary>
        /// Type code of the cluster for the VM to be created
        /// </summary>
        public string ClusterTypeCode { get; set; }

        /// <summary>
        /// Name of the cluster for the VM to be created
        /// </summary>
        public string ClusterTypeName { get; set; }

        /// <summary>
        /// Count of cluster nodes for the VM to be created
        /// </summary>
        public Nullable<int> ClusterNodeCount { get; set; }

        /// <summary>
        /// Cluster nodes for the VM to be created
        /// </summary>
        public string ClusterNodes { get; set; }

        /// <summary>
        /// VLAN code for the VM to be created
        /// </summary>
        public string VLANCode { get; set; }

        /// <summary>
        /// Name of the VLAN for the VM to be created
        /// </summary>
        public string VLANName { get; set; }

        /// <summary>
        /// XML configuration of NIC1 for the VM to be created
        /// </summary>
        public string NIC1_Config { get; set; }

        /// <summary>
        /// XML configuration of NIC2 for the VM to be created
        /// </summary>
        public string NIC2_Config { get; set; }

        /// <summary>
        /// NIC1 static IP address?
        /// </summary>
        /* todo: rename property */
        public string NIC1_Static { get; set; }

        /// <summary>
        /// NIC2 static IP address?
        /// </summary>
        /*todo: rename property */
        public string NIC2_Static { get; set; }

        /// <summary>
        /// Network string for the VM to be created
        /// </summary>
        /* todo: rename property */
        public string NetworkString { get; set; }

        /// <summary>
        /// Placement compute string for the VM to be created
        /// </summary>
        public string PlacementComputeString { get; set; }

        /// <summary>
        /// Placement storage string for the VM to be created
        /// </summary>
        public string PlacementStorageString { get; set; }

        /// <summary>
        /// Servers to avoid for placement of the VM to be created
        /// </summary>
        public string PlacementAvoidServers { get; set; }

        /// <summary>
        /// Configuration string for storage
        /// </summary>
        public string StorageConfigString { get; set; }

        /// <summary>
        /// XML configuration of storage for the VM to be created
        /// </summary>
        public string StorageConfigXML { get; set; }

        /// <summary>
        /// Blade name for the VM to be created
        /// </summary>
        /* todo: rename property */
        public string BladeName { get; set; }

        /// <summary>
        /// Build issue string for the VM to be created
        /// </summary>
        public string BuildIssueString { get; set; }

        /// <summary>
        /// Name of the build issue for the VM to be created
        /// </summary>
        public string BuildIssueName { get; set; }

        /// <summary>
        /// Build team for the VM to be created
        /// </summary>
        public string BuildTeam { get; set; }

        /// <summary>
        /// Name of the logging server for the VM to be created
        /// </summary>
        public string LoggingServerName { get; set; }

        /// <summary>
        /// Agile labs account owner for the VM to be created
        /// </summary>
        public string AgileLabsAccountOwner { get; set; }

        /// <summary>
        /// Agile lab region for the VM to be created
        /// </summary>
        public string AgileLabsRegions { get; set; }
        
        /// <summary>
        /// Azure region code for the VM to be created
        /// </summary>
        public string AzureRegionCode { get; set; }

        /// <summary>
        /// Name of the region name for the VM to be created
        /// </summary>
        public string AzureRegionName { get; set; }

        /// <summary>
        /// Azure features for the VM to be created
        /// </summary>
        public string AzureFeatures { get; set; }

        /// <summary>
        /// Name of the server to migrate from for the VM to be created
        /// </summary>
        public string MigrateFromName { get; set; }

        /// <summary>
        /// Asset ID of the server to migrate from for the VM to be created
        /// </summary>
        public string MigrateFromAssetID { get; set; }

        /// <summary>
        /// Flag whether or not to build out IIS for the VM to be created
        /// </summary>
        public bool IISBuildOut { get; set; }

        /// <summary>
        /// Service role of IIS for the VM to be created
        /// </summary>
        public string IISServiceRole { get; set; }

        /// <summary>
        /// Web root drive of IIS for the VM to be created
        /// </summary>
        public string IISWebRootDrive { get; set; }

        /// <summary>
        /// Web log drive of IIS for the VM to be created
        /// </summary>
        public string IISWebLogDrive { get; set; }

        /// <summary>
        /// Flag whether or not to build out SQL for the VM to be created
        /// </summary>
        public bool SQLBuildOut { get; set; }

        /// <summary>
        /// SQL instance name for the VM to be created
        /// </summary>
        public string SQLInstanceName { get; set; }

        /// <summary>
        /// SQL collation for the VM to be created
        /// </summary>
        /*todo: rename property*/
        public string SQLCollation { get; set; }

        /// <summary>
        /// Version code of SQL for the VM to be created
        /// </summary>
        public string SQLVersionCode { get; set; }

        /// <summary>
        /// Name of SQL version for the VM to be created
        /// </summary>
        public string SQLVersionName { get; set; }

        /// <summary>
        /// Non-standard SQL version for the VM to be created
        /// </summary>
        public string SQLNonStandardVersion { get; set; }

        /// <summary>
        /// SQL administrator group for the VM to be created
        /// </summary>
        public string SQLAdminGroup { get; set; }

        /// <summary>
        /// Flag of whether or not to enable SSAS on SQL for the VM to be created
        /// </summary>
        /*todo: rename this property*/
        public bool SQLEnableSSAS { get; set; }

        /// <summary>
        /// SQL SSAS mode for the VM to be created
        /// </summary>
        /* todo: rename this property*/
        public string SQLSSASMode { get; set; }

        /// <summary>
        /// Flag of whether or not to enable SQL replication for the VM to be created
        /// </summary>
        public bool SQLEnableReplication { get; set; }

        /// <summary>
        /// Flag of whether or not to enable SSIS on SQL for the VM to be created
        /// </summary>
        /* todo: rename this property */
        public bool SQLEnableSSIS { get; set; }
        
        /// <summary>
        /// Binaries drive for SQL for the VM to be created
        /// </summary>
        public string SQLBinariesDrive { get; set; }

        /// <summary>
        /// Backup drive for SQL for the VM to be created
        /// </summary>
        public string SQLBackupDrive { get; set; }

        /// <summary>
        /// Data drive for SQL for the VM to be created
        /// </summary>
        public string SQLDataDrive { get; set; }

        /// <summary>
        /// Drive for SQL logs for the VM to be created
        /// </summary>
        public string SQLLogDrive { get; set; }

        /// <summary>
        /// Temporary SQL database drive for the VM to be created
        /// </summary>
        public string SQLTempDBDrive { get; set; }

        /// <summary>
        /// Priority code for the CMP VM build request
        /// </summary>
        public string PriorityCode { get; set; }

        /// <summary>
        /// Name of the priority code for the CMP VM build reuqest
        /// </summary>
        public string PriorityName { get; set; }

        /// <summary>
        /// Triage code for the CMP VM build request
        /// </summary>
        public string TriageCode { get; set; }

        /// <summary>
        /// Name of the triage for the CMP VM build request
        /// </summary>
        public string TriageName { get; set; }

        /// <summary>
        /// Name of who is assigned to the CMP VM build request
        /// </summary>
        public string AssignedTo { get; set; }

        /// <summary>
        /// Name of who created the CMP VM build request
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Name of who last updated the CMP VM build request
        /// </summary>
        public string LastUpdatedBy { get; set; }

        /// <summary>
        /// Date the CMP VM build request was delivered
        /// </summary>
        public Nullable<System.DateTime> DateDelivered { get; set; }

        /// <summary>
        /// Estimated date the CMP VM build request will be delivered
        /// </summary>
        public Nullable<System.DateTime> DateETA { get; set; }

        /// <summary>
        /// Date the CMP VM build request was requested
        /// </summary>
        public System.DateTime DateRequested { get; set; }

        /// <summary>
        /// Date the CMP VM build request was submitted
        /// </summary>
        public System.DateTime DateSubmitted { get; set; }

        /// <summary>
        /// Date the CMP VM build request was last updated
        /// </summary>
        public System.DateTime DateUpdated { get; set; }

        /// <summary>
        /// Flag of whether or not the CMP VM build request is complex
        /// </summary>
        public Nullable<bool> FlagComplex { get; set; }

        /// <summary>
        /// Flag of whether or not the CMP VM build request is standard
        /// </summary>
        public Nullable<bool> FlagNonStandard { get; set; }

        /// <summary>
        /// Flag of whether or not the CMP VM build request is template based
        /// </summary>
        public Nullable<bool> FlagTemplateBased { get; set; }

        /// <summary>
        /// Flag of whether or not the CMP VM build request is unplanned
        /// </summary>
        public Nullable<bool> FlagUnplanned { get; set; }

        /// <summary>
        /// ID of the organization requesting the CMP VM build request
        /// </summary>
        public Nullable<int> OrgID { get; set; }

        /// <summary>
        /// Financial asset owner of the organization requesting the CMP VM build request
        /// </summary>
        public string OrgFinancialAssetOwner { get; set; }

        /// <summary>
        /// Chargeback group of the organization requesting the CMP VM build request
        /// </summary>
        public string OrgChargebackGroup { get; set; }

        /// <summary>
        /// Division of the organization requesting the CMP VM build request
        /// </summary>
        public string OrgDivision { get; set; }

        /// <summary>
        /// Domain of the organization requesting the CMP VM build request
        /// </summary>
        public string OrgDomain { get; set; }

        /// <summary>
        /// Organization name of the organization requesting the CMP VM build request
        /// </summary>
        /* todo: rename this property*/
        public string OrgOrg { get; set; }

        /// <summary>
        /// Team in the organization requesting the CMP VM build request
        /// </summary>
        public string OrgTeam { get; set; }

        /// <summary>
        /// ID of the application for the CMP VM build request
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Name of the application the CMP VM build request
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// Flag whether the application has a parent service
        /// </summary>
        public Nullable<bool> AppHasParentService { get; set; }

        /// <summary>
        /// Internal Order Billing for the CMP VM build request
        /// </summary>
        public string BillingInternalOrder { get; set; }

        /// <summary>
        /// Billing cost center for the CMP VM build request
        /// </summary>
        public string BillingCostCenter { get; set; }

        /// <summary>
        /// MSIT monitored for the CMP VM build request
        /// </summary>

        public string MsitMonitored {get; set;}

        /// <summary>
        /// ITSM service category the CMP VM build request
        /// </summary>
        public string ITSMServiceCategory { get; set; }

        /// <summary>
        /// ITSM service window for the CMP VM build request
        /// </summary>
        public string ITSMServiceWindow { get; set; }

        /// <summary>
        /// ITSM RFC CI Creation for the CMP VM build request
        /// </summary>
        /* todo: rename this property */
        public string ITSMRFCCICreation { get; set; }

        /// <summary>
        /// ITSM RFC IIS SQL install the CMP VM build request
        /// </summary>
        /* todo: rename this property */
        public string ITSMRFCIISSQLInstall { get; set; }

        /// <summary>
        /// ITSM RFC storage XML configuration for the CMP VM build request
        /// </summary>
        /* todo: rename this property */
        public string ITSMRFCStorageConfig { get; set; }

        /// <summary>
        /// ITSM responsible owner for the VM to be created
        /// </summary>
        public string ITSMResponsibleOwner { get; set; }

        /// <summary>
        /// ITSM accountable owner for the VM to be created
        /// </summary>
        public string ITSMAccountableOwner { get; set; }

        /// <summary>
        /// ITSM CI owner for the VM to be created
        /// </summary>
        public string ITSMCIOwner { get; set; }

        /// <summary>
        /// ITSM L1 support team for the VM to be created
        /// </summary>
        public string ITSML1SupportTeam { get; set; }

        /// <summary>
        /// ITSM L2 support team for the VM to be created
        /// </summary>
        public string ITSML2SupportTeam { get; set; }

        /// <summary>
        /// ITSM L3 support team for the VM to be created
        /// </summary>
        public string ITSML3SupportTeam { get; set; }

        /// <summary>
        /// Flag of whether or not the VM to be created will be monitored by ITSM
        /// </summary>
        public Nullable<bool> ITSMMonitoredFlag { get; set; }
        
        /// <summary>
        /// Request administrators for the VM to be created
        /// </summary>
        public string RequestAdmins { get; set; }

        /// <summary>
        /// WAhich Azure admin API? ARM or RDFE
        /// </summary>
        public string AzureApiName { get; set; }

        /// <summary>
        /// ARM support
        /// </summary>
        public string AzureImagePublisher { get; set; }

        /// <summary>
        /// ARM support
        /// </summary>
        public string AzureImageOffer { get; set; }

        /// <summary>
        /// ARM support
        /// </summary>
        public string AzureWindowsOSVersion { get; set; }

    }
}
