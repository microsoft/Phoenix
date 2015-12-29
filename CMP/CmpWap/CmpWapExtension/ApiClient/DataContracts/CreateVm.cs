// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
    /// <summary>
    /// This is a data contract class between extensions and resource provider
    /// CreateVM contains data contract of data which shows up in "VMs" tab inside Cmp Tenant Extension
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
    public class CreateVm
    {
        /// <summary> </summary>
        [Required]
        [DataMember(Order = 1)]
        public string Name { get; set; }

        /// <summary> </summary>
        /// 
        [Required]
        [DataMember(Order = 2)]
        public string SubscriptionId { get; set; }

        /// <summary> </summary>
        [Required]
        [DataMember(Order = 3)]
        public string VmAppName { get; set; }

        [Required]
        [DataMember(Order = 4)]
        public string VmAppId { get; set; }
        
        [Required]
        [DataMember(Order = 5)]
        public string EnvResourceGroupName { get; set; }
        
        [DataMember(Order = 6)]
        public string VmDomain { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 7)]
        public string VmAdminName { get; set; }

        [DataMember(Order = 8)]
        public string VmAdminPassword { get; set; }

        [Required]
        [DataMember(Order = 9)]
        public string VmSourceImage { get; set; }

        [Required]
        [DataMember(Order = 10)]
        public string VmSize { get; set; }

        [Required]
        [DataMember(Order = 11)]
        public string VmRegion { get; set; }

        [Required]
        [DataMember(Order = 12)]
        public string VmRole { get; set; }

        [Required]
        [DataMember(Order = 13)]
        public string VmDiskSpec { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 14)]
        public string VmConfig { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 15)]
        public string VmTagData { get; set; }

        [Required]
        [DataMember(Order = 16)]
        public string ServiceCategory { get; set; }

        //[Required]
        [DataMember(Order = 17)]
        public string Nic1 { get; set; }

        [DataMember(Order = 18)]
        public string Msitmonitored { get; set; }

        [DataMember(Order = 19)]
        public SQLConfig sqlconfig { get; set; }

        [DataMember(Order = 20)]
        public IISConfig IIsconfig { get; set; }
        /// <summary> </summary>
        [DataMember(Order = 21)]
        public int Id { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 22)]
        public string StatusCode { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 23)]
        public string StatusMessage { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 24)]
        public int CmpRequestId { get; set; }

        /// <summary> </summary>
        [DataMember(Order = 25)]
        public string AddressFromVm { get; set; }


        [DataMember(Order = 26)]
        public string Type { get; set; }


        [DataMember(Order = 27)]
        public string CreatedBy { get; set; }

        [Required]
        [DataMember(Order = 28)]
        public string EnvironmentClass { get; set; }

       [DataMember(Order =29)]
        public string ExceptionMessage { get; set; }
        [DataMember(Order = 30)]
        public string AccountAdminLiveEmailId { get; set; }

       [DataMember(Order = 31)]
       public string OsCode { get; set; } //*** ["Windows","Linux"]

       [DataMember(Order = 32)]
       public string AzureApiName { get; set; } //*** ["RDFE","ARM"]
    }

    public class SQLConfig
    {
         [DataMember(Order = 1)]
        public bool InstallSql { get; set; }
         [DataMember(Order = 2)]
        public bool InstallAnalysisServices { get; set; }
         [DataMember(Order = 3)]
        public bool InstallReplicationServices { get; set; }
         [DataMember(Order = 4)]
        public bool InstallIntegrationServices { get; set; }
         [DataMember(Order = 5)]
        public string SqlInstancneName { get; set; }
         [DataMember(Order = 6)]
        public string Collation { get; set; }
         [DataMember(Order = 7)]
        public string Version { get; set; }
         [DataMember(Order = 8)]
        public string AdminGroups { get; set; }
         [DataMember(Order = 9)]
        public string AnalysisServicesMode { get; set; }
    }

    public class IISConfig
    {
        [DataMember(Order = 1)]
         public bool InstallIis { get; set; }
        [DataMember(Order = 2)]
         public string RoleServices { get; set; }
       
    }

    public class SecurityGroupResult
    {
        [DataMember(Order = 1)]
        public bool Status { get; set; }

        [DataMember(Order = 2)]
        public string Result { get; set; }

    }


}
