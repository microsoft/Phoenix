using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    /// <summary>
    /// Class representing the environment type 
    /// </summary>
    [Table("EnvironmentType")]
    public partial class EnvironmentType
    {
        public EnvironmentType()
        {
            ResourceProviderAcctGroups = new HashSet<ResourceProviderAcctGroup>();
        }

        /// <summary>
        /// ID for the environment type
        /// </summary>
        public int EnvironmentTypeId { get; set; }

        /// <summary>
        /// Name of the environment type
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Description of the environment type
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Flag on whther or not the environment type is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Creation date time of the environment type
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Name of who created the environment type
        /// </summary>
        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Date time of most recent update to the environment type
        /// </summary>
        public DateTime LastUpdatedOn { get; set; }

        /// <summary>
        /// Name of who last updated the environment type
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LastUpdatedBy { get; set; }

        /// <summary>
        /// Resource provider account groups for the environment type
        /// </summary>
        public virtual ICollection<ResourceProviderAcctGroup> ResourceProviderAcctGroups { get; set; }
    }
}
