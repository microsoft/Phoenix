using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    public partial class Group
    {
        /// <summary>
        /// ID of the group
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the group
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Description for the group
        /// </summary>
        public string GroupDescription { get; set; }

        /// <summary>
        /// XML configuration for the group
        /// </summary>
        public string Config { get; set; }

        /// <summary>
        /// Tag data for the group
        /// </summary>
        public string TagData { get; set; }

        /// <summary>
        /// ID for the tag data of the group
        /// </summary>
        public Nullable<int> TagId { get; set; }

        /// <summary>
        /// Flag whether or not the group is active
        /// </summary>
        public bool IsActive { get; set; }
    }
}
