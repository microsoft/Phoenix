using System;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    /// <summary>
    /// Represents details of a WAP subscription
    /// </summary>
    
    public partial class WapSubscriptionData
    {
        /// <summary>
        /// Database-generated ID of the WAP subscription
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///  ID of the WAP subscription
        /// </summary>
        public string WapSubscriptionID { get; set; }
        
        /// <summary>
        /// Default object creation group ID of the WAP subscription
        /// </summary>
        public int DefaultObjectCreationGroupID { get; set; }

        /// <summary>
        /// XML configuration of the WAP subscription
        /// </summary>
        public string Config { get; set; }
        
        /// <summary>
        /// Tag data of the WAP subscription
        /// </summary>
        public string TagData { get; set; }

        /// <summary>
        /// ID for the tag data of the WAP subscription
        /// </summary>
        public Nullable<int> TagId { get; set; }

        /// <summary>
        /// Flag of whether or not the WAP subscription is active
        /// </summary>
        public bool IsActive { get; set; }
    }
}
