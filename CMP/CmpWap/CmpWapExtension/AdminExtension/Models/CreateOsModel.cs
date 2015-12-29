using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models
{
    public class CreateOsModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVmModel" /> class.
        /// </summary>
        public CreateOsModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVmModel" /> class.
        /// </summary>
        /// <param name="createOsFromApi">The domain name from API.</param>
        public CreateOsModel(OS createOsFromApi)
        {
            this.AzureImageName = createOsFromApi.AzureImageName;
            this.VmOsId = createOsFromApi.VmOsId;
            this.Name = createOsFromApi.Name;
            this.Description = createOsFromApi.Description;
            this.IsActive = createOsFromApi.IsActive;
        }

        /// <summary>
        /// Covert to the API object.
        /// </summary>
        /// <returns>The API DomainName data contract.</returns>
        public OS ToApiObject()
        {
            return new OS()
            {
                AzureImageName = this.AzureImageName,
                VmOsId = this.VmOsId,
                Name = this.Name,
                Description = this.Description,
                IsActive = this.IsActive
            };
        }

        /// <summary> </summary>
        public int VmOsId { get; set; }


        public string Name { get; set; }

        public string Description { get; set; }


        public string AzureImageName { get; set; }

        public bool IsActive { get; set; }

    }
}
