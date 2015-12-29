using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Represents a SQL Analysis Service mode
    /// </summary>
    class SQLAnalysisServiceModesModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes an empty model
        /// </summary>
        public SQLAnalysisServiceModesModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLAnalysisServiceModesModel" /> class.
        /// </summary>
        /// <param name="createIIsRoleservicesFromApi">The mode from API.</param>
          public SQLAnalysisServiceModesModel(SQLAnalysisServiceModes createIIsRoleservicesFromApi)
        {
            this.Id = createIIsRoleservicesFromApi.SQLAnalysisServicesModeId;
            this.Name = createIIsRoleservicesFromApi.Name;
            this.Description = createIIsRoleservicesFromApi.Description;
          
        }

        /// <summary>
        /// Converts to an API object
        /// </summary>
        /// <returns>A configured mode</returns>
        public SQLAnalysisServiceModes ToApiObject()
        {
            return new SQLAnalysisServiceModes()
            {
                SQLAnalysisServicesModeId = this.Id,
            Name = this.Name,
            Description = this.Description
           
            };
        }
        
        /// <summary>
        /// The ID of the mode
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the mode
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the mode
        /// </summary>
        public string Description { get; set; }
    }
}
