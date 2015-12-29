namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    /// <summary>
    /// Class representing a CMP requet
    /// </summary>
    public partial class CmpRequest
    {
        /// <summary>
        /// Database-generated ID for the CMP request
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// WAP subscription ID for the CMP request
        /// </summary>
        [StringLength(100)]
        public string WapSubscriptionID { get; set; }

        /// <summary>
        /// ID for the CMP request
        /// </summary>
        public int? CmpRequestID { get; set; }

        /// <summary>
        /// Parent application name for the CMP request
        /// </summary>
        [StringLength(100)]
        public string ParentAppName { get; set; }

        /// <summary>
        /// Name of the target Vm for the CMP request
        /// </summary>
        [StringLength(256)]
        public string TargetVmName { get; set; }

        /// <summary>
        /// Domain of the CMP request
        /// </summary>
        [StringLength(100)]
        public string Domain { get; set; }

        /// <summary>
        /// Vm size for the target VM
        /// </summary>
        [StringLength(50)]
        public string VmSize { get; set; }

        /// <summary>
        /// Target location of the target VM
        /// </summary>
        [StringLength(50)]
        public string TargetLocation { get; set; }

        /// <summary>
        /// Status code of the CMP request
        /// </summary>
        [StringLength(50)]
        public string StatusCode { get; set; }

        /// <summary>
        /// Source image name of the target VM
        /// </summary>
        [StringLength(256)]
        public string SourceImageName { get; set; }

        /// <summary>
        /// Source server of the CMP request
        /// </summary>
        [StringLength(256)]
        public string SourceServerName { get; set; }

        /// <summary>
        /// User specification for the CMP request
        /// </summary>
        [StringLength(1024)]
        public string UserSpec { get; set; }

        /// <summary>
        /// Storage specification for the CMP request
        /// </summary>
        [StringLength(1024)]
        public string StorageSpec { get; set; }

        /// <summary>
        /// Feature specification for the CMP request
        /// </summary>
        [StringLength(1024)]
        public string FeatureSpec { get; set; }


        /// <summary>
        /// XML configuration for the CMP request
        /// </summary>
        public string Config { get; set; }

        /// <summary>
        /// Request type of the CMP request
        /// </summary>
        [StringLength(50)]
        public string RequestType { get; set; }

        /// <summary>
        /// Name of who requested the CMP request
        /// </summary>
        [StringLength(50)]
        public string WhoRequested { get; set; }

        /// <summary>
        /// Date the CMP request was requested
        /// </summary>
        public DateTime? WhenRequested { get; set; }

        /// <summary>
        /// Status message for the CMP request
        /// </summary>
        [StringLength(4096)]
        public string StatusMessage { get; set; }

        /// <summary>
        /// Exception message for the CMP request; only populated if an exception occurs
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Warnings for the CMP request
        /// </summary>
        public string Warnings { get; set; }

        /// <summary>
        /// Datetime of most recent status update
        /// </summary>
        public DateTime? LastStatusUpdate { get; set; }

        /// <summary>
        /// Flag for whether or not the CMP request is active
        /// </summary>
        public bool? Active { get; set; }

        /// <summary>
        /// Tag data for the CMP request
        /// </summary>
        public string TagData { get; set; }

        /// <summary>
        /// ID for the CMP request TagData
        /// </summary>
        public int? TagID { get; set; }

        /// <summary>
        /// Address from the VM
        /// </summary>
        [StringLength(100)]
        public string AddressFromVm { get; set; }

        /// <summary>
        /// Group access ID for the VM
        /// </summary>
        public int? AccessGroupId { get; set; }
    }
}
