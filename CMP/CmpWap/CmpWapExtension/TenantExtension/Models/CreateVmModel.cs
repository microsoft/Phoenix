//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Data model for virtual machine tenant view
    /// </summary>    
    public class CreateVmModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVmModel" /> class.
        /// </summary>
        public CreateVmModel()
        {
            this.Type = "VMs";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVmModel" /> class.
        /// </summary>
        /// <param name="createVmFromApi">The domain name from API.</param>
        public CreateVmModel(CreateVm createVmFromApi)
        {
            this.Id = createVmFromApi.Id;
            this.CmpRequestId = createVmFromApi.CmpRequestId;
            this.Name = createVmFromApi.Name;
            this.SubscriptionId = createVmFromApi.SubscriptionId;
            this.VmAppName = createVmFromApi.VmAppName;
            this.VmAppID = createVmFromApi.VmAppId;
            this.VmDomain = createVmFromApi.VmDomain;
            this.VmAdminName = createVmFromApi.VmAdminName;
            this.VmAdminPassword = createVmFromApi.VmAdminPassword;
            this.VmSourceImage = createVmFromApi.VmSourceImage;
            this.VmSize = createVmFromApi.VmSize;
            this.VmRegion = createVmFromApi.VmRegion;
            this.VmRole = createVmFromApi.VmRole;
            this.VmDiskSpec = createVmFromApi.VmDiskSpec;
            this.VmConfig = createVmFromApi.VmConfig;
            this.VmTagData = createVmFromApi.VmTagData;
            this.StatusCode = createVmFromApi.StatusCode;
            this.StatusMessage = createVmFromApi.StatusMessage;
            this.AddressFromVm = createVmFromApi.AddressFromVm;
            this.ServiceCategory = createVmFromApi.ServiceCategory;
            this.Nic1 = createVmFromApi.Nic1;
            this.Msitmonitored = createVmFromApi.Msitmonitored;
            this.Sqlconfig = createVmFromApi.sqlconfig==null ?null:new SQLConfig(createVmFromApi.sqlconfig);
            this.IIsconfig = createVmFromApi.IIsconfig == null ? null : new IISConfig(createVmFromApi.IIsconfig);
            this.EnvResourceGroupName = createVmFromApi.EnvResourceGroupName;
            this.EnvironmentClass = createVmFromApi.EnvironmentClass;
            this.Type = "VMs";
            this.AccountAdminLiveEmailId = createVmFromApi.AccountAdminLiveEmailId;
            this.OsCode = createVmFromApi.OsCode;
            this.AzureApiName = createVmFromApi.AzureApiName;
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API VM data contract.</returns>
        public CreateVm ToApiObject()
        {
            return new CreateVm()
            {
                Id = this.Id,
                CmpRequestId = this.CmpRequestId,
                Name = this.Name,
                SubscriptionId = this.SubscriptionId,
                VmAppName = this.VmAppName,
                VmAppId=this.VmAppID,
                VmDomain = this.VmDomain,
                VmAdminName = this.VmAdminName,
                VmAdminPassword = this.VmAdminPassword,
                VmSourceImage = this.VmSourceImage,
                VmSize = this.VmSize,
                VmRegion = this.VmRegion,
                VmRole = this.VmRole,
                VmDiskSpec = this.VmDiskSpec,
                VmConfig = this.VmConfig,
                VmTagData = this.VmTagData,
                StatusCode = this.StatusCode,
                StatusMessage = this.StatusMessage,
                AddressFromVm = this.AddressFromVm,
                ServiceCategory = this.ServiceCategory,
                Nic1 = this.Nic1,
                Msitmonitored = this.Msitmonitored,
                EnvResourceGroupName=this.EnvResourceGroupName,
                sqlconfig = this.Sqlconfig==null? null:new ApiClient.DataContracts.SQLConfig {
                            InstallSql = this.Sqlconfig.InstallSql,
                            InstallAnalysisServices = this.Sqlconfig.InstallAnalysisServices,
                            InstallReplicationServices = this.Sqlconfig.InstallReplicationServices,
                            InstallIntegrationServices = this.Sqlconfig.InstallIntegrationServices,
                            SqlInstancneName = this.Sqlconfig.SqlInstancneName,
                            Collation = this.Sqlconfig.Collation,
                            Version = this.Sqlconfig.Version,
                            AdminGroups = this.Sqlconfig.AdminGroups,
                            AnalysisServicesMode = this.Sqlconfig.AnalysisServicesMode
                             },

                IIsconfig=this.IIsconfig ==null? null: new ApiClient.DataContracts.IISConfig{
                            InstallIis=this.IIsconfig.InstallIis,
                            RoleServices=this.IIsconfig.RoleServices

                },
                EnvironmentClass = this.EnvironmentClass,
                Type = this.Type,
                AccountAdminLiveEmailId = this.AccountAdminLiveEmailId,
                OsCode = this.OsCode,
                AzureApiName = this.AzureApiName
            };
        }

        /// <summary>
        /// Gets or sets the ID.
        // </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationId.
        // </summary>
        public string VmAppID { get; set; }

        /// <summary>
        /// Gets or sets the CMP request ID.
        // </summary>
        public int CmpRequestId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        // </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the subscription ID.
        // </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Resource group name for the VM
        /// </summary>
        public string EnvResourceGroupName { get; set; }

        /// <summary>
        /// Gets or sets the value of the display name of the application 
        /// </summary>
        public string VmAppName { get; set; }

        /// <summary>
        /// Gets or sets the value of the domain
        /// </summary>
        public string VmDomain { get; set; }

        /// <summary>
        /// Gets or sets the value of the admin name
        /// </summary>
        public string VmAdminName { get; set; }

        /// <summary>
        /// Gets or sets the value of the admin password
        /// </summary>
        public string VmAdminPassword { get; set; }

        /// <summary>
        /// Gets or sets the value of the source image
        /// </summary>
        public string VmSourceImage { get; set; }

        /// <summary>
        /// Gets or sets the value of the size
        /// </summary>
        public string VmSize { get; set; }

        /// <summary>
        /// Gets or sets the value of the region
        /// </summary>
        public string VmRegion { get; set; }

        /// <summary>
        /// Gets or sets the value of the role
        /// </summary>
        public string VmRole { get; set; }

        /// <summary>
        /// Gets or sets the value of the disk specification
        /// </summary>
        public string VmDiskSpec { get; set; }

        /// <summary>
        /// Gets or sets the value of the configuration
        /// </summary>
        public string VmConfig { get; set; }

        /// <summary>
        /// Gets or sets the value of the tag data
        /// </summary>
        public string VmTagData { get; set; }

        /// <summary>
        /// The current status code of the request
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// The current status message for the request
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// The IP address for the VM
        /// </summary>
        public string AddressFromVm { get; set; }

        /// <summary>
        /// The service category of the VM
        /// </summary>
        public string ServiceCategory { get; set; }

        /// <summary>
        /// The first network interface controller
        /// </summary>
        public string Nic1 { get; set; }

        /// <summary>
        /// Whether or not the VM is monitored by MSIT
        /// todo: rename
        /// </summary>
        public string Msitmonitored { get; set; }

        /// <summary>
        /// The SQL Server configuration for the VM
        /// </summary>
        public SQLConfig Sqlconfig { get; set; }

        /// <summary>
        /// The IIS configuration for the VM
        /// </summary>
        public IISConfig IIsconfig { get; set; }

        /// <summary>
        /// The environment class of the request
        /// </summary>
        public string EnvironmentClass { get; set; }

        /// <summary>
        /// 'RDFE' or 'ARM'
        /// </summary>
        public string AzureApiName { get; set; }

        /// <summary>
        /// 'Windows' or 'Linux'
        /// </summary>
        public string OsCode { get; set; }

      //  /// <summary> 
      // /// This property to be set to navigate to sub-tabs. 
      //  /// </summary> 
      public string Type { get; set; }


      public string AccountAdminLiveEmailId { get; set; }
    }


    /// <summary>
    /// Represents a SQL Server configuration
    /// todo: Move to separate class
    /// </summary>
    public class SQLConfig
    {
        /// <summary>
        /// Instantiates an empty configuration
        /// </summary>
        public SQLConfig()
        {

        }

        /// <summary>
        /// Instantiates a new configuration using the API object
        /// </summary>
        /// <param name="sqlconfig">The model to use</param>
        public SQLConfig(ApiClient.DataContracts.SQLConfig sqlconfig)
        {
            this.InstallSql = sqlconfig.InstallSql;

            this.InstallAnalysisServices = sqlconfig.InstallAnalysisServices;

            this.InstallReplicationServices = sqlconfig.InstallReplicationServices;

            this.InstallIntegrationServices = sqlconfig.InstallIntegrationServices;

            this.SqlInstancneName = sqlconfig.SqlInstancneName;

            this.Collation = sqlconfig.Collation;

            this.Version = sqlconfig.Version;

            this.AdminGroups = sqlconfig.AdminGroups;

            this.AnalysisServicesMode = sqlconfig.AnalysisServicesMode;

        }

        /// <summary>
        /// Whether or not to install SQL Server
        /// </summary>
        public bool InstallSql { get; set; }

        /// <summary>
        /// Whether or not to install the SQL Analysis Services
        /// </summary>
        public bool InstallAnalysisServices { get; set; }

        /// <summary>
        /// Whether or not to install the SQL Replication Services
        /// </summary>
        public bool InstallReplicationServices { get; set; }

        /// <summary>
        /// Whether or not to install the SQL Integration Services
        /// </summary>
        public bool InstallIntegrationServices { get; set; }

        /// <summary>
        /// The name of the SQL Server instance
        /// </summary>
        public string SqlInstancneName { get; set; }

        /// <summary>
        /// The collation of the SQL Server instance
        /// </summary>
        public string Collation { get; set; }

        /// <summary>
        /// The version of SQL Server installed
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The adminstrative groups of the SQL Server instance
        /// </summary>
        public string AdminGroups { get; set; }

        /// <summary>
        /// Which mode of SQL Analysis Services was used, if any
        /// </summary>
        public string AnalysisServicesMode { get; set; }
    }

    /// <summary>
    /// Represents a Internet Information Services configuration
    /// todo: Move to separate file
    /// </summary>
    public class IISConfig
    {
        /// <summary>
        /// Instantiates an empty configuration of IIS
        /// </summary>
        public IISConfig()
        {

        }

        /// <summary>
        /// Instantiates an IIS configuration using an API object
        /// </summary>
        /// <param name="iisconfig">The API model to use</param>
        public IISConfig(ApiClient.DataContracts.IISConfig iisconfig)
        {

            this.InstallIis = iisconfig.InstallIis;
            this.RoleServices = iisconfig.RoleServices;
        }

        /// <summary>
        /// Whether or not to install IIS
        /// </summary>
        public bool InstallIis { get; set; }
        
        /// <summary>
        /// Which role services were used
        /// </summary>
        public string RoleServices { get; set; }
       
    }

}