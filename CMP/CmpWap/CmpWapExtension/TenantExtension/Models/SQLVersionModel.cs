using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Represents a SQL Server version
    /// </summary>
    public class SQLVersionModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLVersionModel" /> class.
        /// </summary>
        public SQLVersionModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLVersionModel" /> class.
        /// </summary>
        /// <param name="createSVM">The version from API.</param>
        public SQLVersionModel(SQLVersion createSVM)
        {
            this.SQLVersionId = createSVM.SQLVersionId;
            this.Name = createSVM.Name;
            this.Description = createSVM.Description;
            this.IsActive = createSVM.IsActive;
            
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API version data contract.</returns>
        public SQLVersion ToApiObject()
        {
            return new SQLVersion()
            {

            SQLVersionId = this.SQLVersionId,
            Name = this.Name,
            Description = this.Description,
            IsActive = this.IsActive
            
            };
        }
        
        /// <summary>
        /// The version ID
        /// </summary>
        public int SQLVersionId { get; set; }

        /// <summary>
        /// The name of the version
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the version
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Whether or not the version is active
        /// </summary>
        public bool IsActive { get; set; }
    }
}
