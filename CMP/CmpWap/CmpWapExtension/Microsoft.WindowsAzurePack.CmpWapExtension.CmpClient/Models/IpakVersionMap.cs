namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    /// <summary>
    /// Class representing the IPAK version
    /// </summary>
    [Table("IpakVersionMap")]
    public partial class IpakVersionMap
    {
        /// <summary>
        /// Database-generated ID of the IPAK version
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// Version code for the IPAK version
        /// </summary>
        [StringLength(50)]
        public string VersionCode { get; set; }

        /// <summary>
        /// Name of the IPAK version
        /// </summary>
        [StringLength(200)]
        public string VersionName { get; set; }

        /// <summary>
        /// Azure region of the IPAK version
        /// </summary>
        [StringLength(50)]
        public string AzureRegion { get; set; }

        /// <summary>
        /// Ad domain of the IPAK version
        /// </summary>
        [StringLength(200)]
        public string AdDomain { get; set; }

        /// <summary>
        /// XML configuration for the IPAK version
        /// </summary>
        public string Config { get; set; }

        /// <summary>
        /// Tag data for the IPAK version
        /// </summary>
        public string TagData { get; set; }

        /// <summary>
        /// Directory location for the IPAK version
        /// </summary>
        [StringLength(250)]
        public string IpakDirLocation { get; set; }

        /// <summary>
        /// Full file location for the IPAK version
        /// </summary>
        [StringLength(250)]
        public string IpakFullFileLocation { get; set; }

        /// <summary>
        /// Flag of whether or not the IPAK version is active
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Administrator name for the IPAK version
        /// </summary>
        [StringLength(100)]
        public string AdminName { get; set; }

        /// <summary>
        /// QFE version for the IPAK version
        /// </summary>
        /* todo: rename property*/
        [StringLength(100)]
        public string QfeVersion { get; set; }
    }
}
