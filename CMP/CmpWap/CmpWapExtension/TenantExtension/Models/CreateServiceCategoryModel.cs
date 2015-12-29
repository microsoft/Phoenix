using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Data model representing a service category
    /// </summary>
  public  class CreateServiceCategoryModel
    {
     public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateServiceCategoryModel" /> class.
        /// </summary>
        public CreateServiceCategoryModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateServiceCategoryModel" /> class.
        /// </summary>
        /// <param name="createServiceCategoryFromApi">The service category from API.</param>
        public CreateServiceCategoryModel(ServiceCategory createServiceCategoryFromApi)
        {
            this.ServiceCategoryId = createServiceCategoryFromApi.ServiceCategoryId;
            this.Name = createServiceCategoryFromApi.Name;
            this.Description = createServiceCategoryFromApi.Description;
            this.IsActive = createServiceCategoryFromApi.IsActive;
           
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API service category data contract.</returns>
        public ServiceCategory ToApiObject()
        {
            return new ServiceCategory()
            {
                ServiceCategoryId = this.ServiceCategoryId,
                Name = this.Name,
                Description = this.Description,
                IsActive = this.IsActive,

            };
        }
        
        /// <summary>
        /// ID of the service category
        /// </summary>
        public int ServiceCategoryId { get; set; }


        /// <summary>
        /// Name of the service category
        /// </summary>
     
        public string Name { get; set; }

        /// <summary>
        /// Description of the service category
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether or not the service category is active
        /// </summary>
        public bool IsActive { get; set; }

       
    }
}
