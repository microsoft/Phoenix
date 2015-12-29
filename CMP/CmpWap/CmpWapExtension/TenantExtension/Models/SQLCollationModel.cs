using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Represents a SQL collation
    /// </summary>
    public class SQLCollationModel
    {
         public const string RegisteredStatus = "Registered";

        /// <summary>
         /// Initializes a new instance of the <see cref="SQLCollationModel" /> class.
        /// </summary>
        public SQLCollationModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLCollationModel" /> class.
        /// </summary>
        /// <param name="createSCM">The collation from API.</param>
        public SQLCollationModel(SQLCollation createSCM)
        {
            this.SQLCollationId = createSCM.SQLCollationId;
            this.Name = createSCM.Name;
            this.Description = createSCM.Description;
            this.IsActive = createSCM.IsActive;
            
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API collation data contract.</returns>
        public SQLCollation ToApiObject()
        {
            return new SQLCollation()
            {

            SQLCollationId = this.SQLCollationId,
            Name = this.Name,
            Description = this.Description,
            IsActive = this.IsActive
            
            };
        }
        
        /// <summary>
        /// The ID of the collation
        /// </summary>
        public int SQLCollationId { get; set; }

        /// <summary>
        /// The name of the collation
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the collation
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Whether or not the collation is active
        /// </summary>
        public bool IsActive { get; set; }
    }
}
