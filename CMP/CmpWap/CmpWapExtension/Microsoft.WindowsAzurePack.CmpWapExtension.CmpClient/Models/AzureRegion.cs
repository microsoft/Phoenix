namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    /// <summary>
    /// Represents an Azure region
    /// </summary>
    [Table("AzureRegion")]
    public partial class AzureRegion
    {
        /// <summary>
        /// ID of the Azure region
        /// </summary>
        public int AzureRegionId { get; set; }

        /// <summary>
        /// Name of the Azure region
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Description of the Azure region
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Azure Storage container where the OS image is stored
        /// </summary>
        public string OsImageContainer { get; set; }

        /// <summary>
        /// Flag of whether or not the Azure storage table is active or not
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Date the Azure region was created
        /// </summary>
        public DateTime CreatedOn { get; set; }


        /// <summary>
        /// Name of who created the Azure region
        /// </summary>
        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Most recent date the Azure region was updated
        /// </summary>
        public DateTime LastUpdatedOn { get; set; }

        /// <summary>
        /// Name of who most recently updated the Azure region
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LastUpdatedBy { get; set; }
    }
}
