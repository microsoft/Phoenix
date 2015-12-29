//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Data model for operating system tenant view
    /// </summary>    
    public class CreateOsModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateOsModel" /> class.
        /// </summary>
        public CreateOsModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateOsModel" /> class.
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
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API operating system data contract.</returns>
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

        /// <summary>ID for the operating system</summary>
        public int VmOsId { get; set; }

        /// <summary>
        /// Name of the operating system
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the operating system
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Name of the associated Azure image
        /// </summary>
        public string AzureImageName { get; set; }
        
        /// <summary>
        /// Whether or not the operating system is active
        /// </summary>
        public bool IsActive { get; set; }
    }

    public class CreateOsComparer : IEqualityComparer<CreateOsModel>
    {
        public bool Equals(CreateOsModel x, CreateOsModel y)
        {
            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the objects' properties are equal.
            return x.Name == y.Name
                   && x.Description == y.Description
                   && x.AzureImageName == y.AzureImageName
                   && x.VmOsId == y.VmOsId;
        }

        public int GetHashCode(CreateOsModel os)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(os, null)) return 0;

            //Get hash code for each field if it is not null. 
            int hashName = os.Name == null ? 0 : os.Name.GetHashCode();
            int hashDescription = os.Description == null ? 0 : os.Description.GetHashCode();
            int hashAzureImageName = os.AzureImageName == null ? 0 : os.AzureImageName.GetHashCode();
            int hashVmOsId = os.VmOsId.GetHashCode();

            //Calculate the hash code for the os. 
            return hashName ^ hashDescription ^ hashAzureImageName ^ hashVmOsId;
        }
    }
}