//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Data model for domain name tenant view
    /// </summary>    
    public class CreateDomainModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDomainModel" /> class.
        /// </summary>
        public CreateDomainModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDomainModel" /> class.
        /// </summary>
        /// <param name="createVmFromApi">The domain name from API.</param>
        public CreateDomainModel(Domain createVmFromApi)
        {
            this.Name = createVmFromApi.Name;
            this.DisplayName = createVmFromApi.DisplayName;
            this.Id = createVmFromApi.Id;
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API DomainName data contract.</returns>
        public Domain ToApiObject()
        {
            return new Domain()
            {
                Name = this.Name,
                DisplayName = this.DisplayName,
                Id=this.Id
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

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public int Id { get; set; }
    }
}