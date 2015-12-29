using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models
{
    public class CreateDomainModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVmModel" /> class.
        /// </summary>
        public CreateDomainModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVmModel" /> class.
        /// </summary>
        /// <param name="createVmFromApi">The domain name from API.</param>
        public CreateDomainModel(Domain createVmFromApi)
        {
            this.Name = createVmFromApi.Name;
            this.DisplayName = createVmFromApi.DisplayName;
        }

        /// <summary>
        /// Covert to the API object.
        /// </summary>
        /// <returns>The API DomainName data contract.</returns>
        public Domain ToApiObject()
        {
            return new Domain()
            {
                Name = this.Name,
                DisplayName = this.DisplayName
            };
        }

        /// <summary>
        /// Gets or sets the name.
        // </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        // </summary>
        public string DisplayName { get; set; }

    }
}
