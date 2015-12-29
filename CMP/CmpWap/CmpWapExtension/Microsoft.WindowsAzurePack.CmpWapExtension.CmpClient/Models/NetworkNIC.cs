using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    /// <summary>
    /// Class representing the network interface controller?
    /// </summary>
    /// /* todo: rename this class */
    [Table("NetworkNIC")]
    public partial class NetworkNIC
    {
        public NetworkNIC()
        {
            ResourceProviderAcctGroups = new HashSet<ResourceProviderAcctGroup>();
        }

        /// <summary>
        /// ID of the network NIC
        /// </summary>
        public int NetworkNICId { get; set; }

        /// <summary>
        /// Name of the network NIC
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Description of the network NIC
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Flag of whether or not the network NIC is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Datetime the network NIC was created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Name of who created the network NIC
        /// </summary>
        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Datetime of when the network NIC was last created
        /// </summary>
        public DateTime LastUpdatedOn { get; set; }

        /// <summary>
        /// Name of who last updated the network NIC
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LastUpdatedBy { get; set; }

        /// <summary>
        /// Ad domain of the network NIC
        /// </summary>
        [StringLength(256)]
        public string ADDomain { get; set; }

        /// <summary>
        /// ID for the Ad domain of the network NIC
        /// </summary>
        public int ADDomainId { get; set; }

        /// <summary>
        /// Resource provider groups of the network NIC
        /// </summary>
        public virtual ICollection<ResourceProviderAcctGroup> ResourceProviderAcctGroups { get; set; }
    }
}
