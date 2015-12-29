using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Represents an IIS Role Service
    /// </summary>
   public class IISRoleServicesModel
    {
         public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="IISRoleServicesModel" /> class.
        /// </summary>
        public IISRoleServicesModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IISRoleServicesModel" /> class.
        /// </summary>
        /// <param name="createIIsRoleservicesFromApi">The role service from API.</param>
        public IISRoleServicesModel(IISRoleService createIIsRoleservicesFromApi)
        {
            this.Id = createIIsRoleservicesFromApi.IISRoleServiceId;
            this.Name = createIIsRoleservicesFromApi.Name;
            this.Description = createIIsRoleservicesFromApi.Description;
          
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API role service data contract.</returns>
        public IISRoleService ToApiObject()
        {
            return new IISRoleService()
            {
           IISRoleServiceId = this.Id,
            Name = this.Name,
            Description = this.Description
           
            };
        }
        
        /// <summary>
        /// ID of the role service
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Name of the role service
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Description of the role service
        /// </summary>
        public string Description { get; set; }

    }
}
