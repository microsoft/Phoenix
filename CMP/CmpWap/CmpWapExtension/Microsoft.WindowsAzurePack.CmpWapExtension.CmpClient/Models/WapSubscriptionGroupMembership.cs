using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    /// <summary>
    /// Details of the WAP subscription group membership
    /// </summary>
    public partial class WapSubscriptionGroupMembership
    {
        /// <summary>
        /// Database-generated ID of the WAP subscription group membership
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID of the WAP subscription group membership
        /// </summary>
        public string WapSubscriptionID { get; set; }

        /// <summary>
        /// Group ID of the WAP subscription group membership
        /// </summary>
        public int GroupID { get; set; }
        
        /// <summary>
        /// Group name of the WAP subscription group membership
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// XML configuration of the WAP subscription group membership
        /// </summary>
        public string Config { get; set; }

        /// <summary>
        /// Tag data of the WAP subscription group membership
        /// </summary>
        public string TagData { get; set; }

        /// <summary>
        /// ID for the tag data of the WAP subscription group membership
        /// </summary>
        public Nullable<int> TagId { get; set; }

        /// <summary>
        /// Flag of whether or not the WAP subscription group membership is active
        /// </summary>
        public bool IsActive { get; set; }
    }
}