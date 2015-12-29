using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Model for environment types
    /// </summary>
    public class CreateEnvironmentTypeModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateEnvironmentTypeModel" /> class.
        /// </summary>
        public CreateEnvironmentTypeModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateEnvironmentTypeModel" /> class.
        /// </summary>
        /// <param name="createEnvironmentTypeFromApi">The environment type from API.</param>
        public CreateEnvironmentTypeModel(EnvironmentType createEnvironmentTypeFromApi)
        {
            this.Name = createEnvironmentTypeFromApi.Name;
            this.Description = createEnvironmentTypeFromApi.Description;
            this.EnvironmentTypeId = createEnvironmentTypeFromApi.EnvironmentTypeId;
            this.IsActive = createEnvironmentTypeFromApi.IsActive;
            this.ResourceProviderAcctGroupName = createEnvironmentTypeFromApi.ResourceProviderAcctGroupName;
           
         
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API EnvironmentType data contract.</returns>
        public EnvironmentType ToApiObject()
        {
            return new EnvironmentType()
            {
              Name=this.Name,
              Description=this.Description,
              IsActive=this.IsActive,
              EnvironmentTypeId=this.EnvironmentTypeId,
              ResourceProviderAcctGroupName=this.ResourceProviderAcctGroupName
              
            };
        }

        /// <summary>
        /// ID for the environment type
        /// </summary>
        public int EnvironmentTypeId { get; set; }

        /// <summary>
        /// Name of the environment type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the environment type
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether the type is active or not
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Name of the RP account group
        /// </summary>
        public string ResourceProviderAcctGroupName { get; set; }
 
      
    }
}
