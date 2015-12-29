namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    /// <summary>
    /// Active directory domain map class
    /// </summary>
    [Table("AdDomainMap")]
    public partial class AdDomainMap
    {
        public AdDomainMap()
        {
            ResourceProviderAcctGroups = new HashSet<ResourceProviderAcctGroup>();
        }

        /// <summary>
        /// Database-generated ID of the Ad domain map
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// Short name for the Ad domain
        /// </summary>
        public string DomainShortName { get; set; }

        /// <summary>
        /// Full name of the Ad domain
        /// </summary>
        public string DomainFullName { get; set; }

        /// <summary>
        /// Username for the Ad domain
        /// </summary>
        public string JoinCredsUserName { get; set; }

        /// <summary>
        /// Password for the Ad domain
        /// </summary>
        public string JoinCredsPasword { get; set; }

        /// <summary>
        /// Flag whether or not the Ad domain is active
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// XML Configuration for the Ad domain
        /// </summary>
        public string Config { get; set; }

        /// <summary>
        /// Server OU for the Ad domain
        /// </summary>
        /* to do: rename property*/
        [StringLength(200)]
        public string ServerOU { get; set; }

        /// <summary>
        /// Workstation OU for the Ad domain
        /// </summary>
        /* to do: rename property*/
        [StringLength(200)]
        public string WorkstationOU { get; set; }

        /// <summary>
        /// Default VM administrator member
        /// </summary>
        public string DefaultVmAdminMember { get; set; }

        /// <summary>
        /// Account group for the Ad domain's resource provider
        /// </summary>
        public virtual ICollection<ResourceProviderAcctGroup> ResourceProviderAcctGroups { get; set; }
    }
}
