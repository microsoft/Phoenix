namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    /// <summary>
    /// Class representing VM operating system information
    /// </summary>
    /* todo: rename this class */
    [Table("VmOs")]
    public partial class VmOs
    {
        /// <summary>
        /// ID of the VM operating system information
        /// </summary>
        [Key]
        public int VmOsId { get; set; }

        /// <summary>
        /// Name of the VM operating system information
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Description of the VM operating system information
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Which family of operating systems this image belongs to
        /// </summary>
        [Required]
        [StringLength(50)]
        public string OsFamily { get; set; }

        /// <summary>
        /// Azure image name of the VM OS
        /// </summary>
        [Required]
        public string AzureImageName { get; set; }

        /// <summary>
        /// Whether the image is a custom one rather than one provided by the Azure product
        /// </summary>
        [Required]
        public bool IsCustomImage { get; set; }

        /// <summary>
        /// Flag of whether or not the VM OS is active or not
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Datetime of when the VM operating system information was created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Name of who created the VM operating system information
        /// </summary>
        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Most recent datetime when the VM operating system information
        /// </summary>
        public DateTime LastUpdatedOn { get; set; }

        /// <summary>
        /// Name of who last updated the Vm operation system information
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LastUpdatedBy { get; set; }

        [Required]
        [StringLength(100)]
        public string AzureImagePublisher { get; set; }

        [Required]
        [StringLength(100)]
        public string AzureImageOffer { get; set; }

        [Required]
        [StringLength(100)]
        public string AzureWindowsOSVersion { get; set; }

    }
}
