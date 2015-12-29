using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    /// <summary>
    /// Represents the resource provider account group
    /// </summary>
    [Table("ResourceProviderAcctGroup")]
    public partial class ResourceProviderAcctGroup
    {
        /// <summary>
        /// ID for the resource provider account group
        /// </summary>
        public int ResourceProviderAcctGroupId { get; set; }

        /// <summary>
        /// Name for the resource provider account group
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// ID for the domain of the resource provider account group
        /// </summary>
        public int DomainId { get; set; }

        /// <summary>
        /// Network NIC of the resource provider account group
        /// </summary>
        public int NetworkNICId { get; set; }

        /// <summary>
        /// ID for the environment type of the resource provider account group
        /// </summary>
        public int EnvironmentTypeId { get; set; }
        
        /// <summary>
        /// Flag of whether or not the resource provider account group is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Datetime when the resource provider account group was created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Name of who created the resource provider account group
        /// </summary>
        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Most recent date time when the resource provider account group was updated
        /// </summary>
        public DateTime LastUpdatedOn { get; set; }

        /// <summary>
        /// Name of who last updated the resource provider account group
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LastUpdatedBy { get; set; }

        /// <summary>
        /// Ad domain map of the resource provider account group
        /// </summary>
        public virtual AdDomainMap AdDomainMap { get; set; }

        /// <summary>
        /// Environment type of the resource provider account group
        /// </summary>
        public virtual EnvironmentType EnvironmentType { get; set; }

        /// <summary>
        /// Network NIC of the resource provider account group
        /// </summary>
        public virtual NetworkNIC NetworkNIC { get; set; }
    }
}
