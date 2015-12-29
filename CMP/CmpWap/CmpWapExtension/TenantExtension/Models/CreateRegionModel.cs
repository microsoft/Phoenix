//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Data model for region tenant view
    /// </summary>    
    public class CreateRegionModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRegionModel" /> class.
        /// </summary>
        public CreateRegionModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRegionModel" /> class.
        /// </summary>
        /// <param name="createSizeFromApi">The domain name from API.</param>
        public CreateRegionModel(Region createSizeFromApi)
        {
            this.Name = createSizeFromApi.Name;
            this.Description = createSizeFromApi.Description;
            this.AzureRegionId = createSizeFromApi.AzureRegionId;
            this.IsActive = createSizeFromApi.IsActive;
            this.OsImageContainer = createSizeFromApi.OsImageContainer;
            this.CreatedOn = createSizeFromApi.CreatedOn;
            this.CreatedBy = createSizeFromApi.CreatedBy;
            this.LastUpdatedOn = createSizeFromApi.LastUpdatedOn;
            this.LastUpdatedBy = createSizeFromApi.LastUpdatedBy;
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API region data contract.</returns>
        public Region ToApiObject()
        {
            return new Region()
            {
                Name = this.Name,
                Description = this.Description,
                AzureRegionId = this.AzureRegionId,
                IsActive = this.IsActive,
                OsImageContainer=this.OsImageContainer,
                CreatedOn = this.CreatedOn,
                CreatedBy = this.CreatedBy,
                LastUpdatedOn = this.LastUpdatedOn,
                LastUpdatedBy = this.LastUpdatedBy
            };
        }

        /// <summary>
        /// Name of the region
        /// </summary>
        public string Name { get; set; }

        /// <summary>Description of the region</summary>
        public string Description { get; set; }

        /// <summary>Container for the operating system image</summary>
        public string OsImageContainer { get; set; }

        /// <summary>Azure ID for the region</summary>
        public int AzureRegionId { get; set; }

        /// <summary>Whether or not the region is active</summary>
        public bool IsActive { get; set; }

        ///// <summary> </summary>
        //public string TagData { get; set; }
        public System.DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime LastUpdatedOn { get; set; }

        public string LastUpdatedBy { get; set; }
    }

    public class CreateRegionComparer : IEqualityComparer<CreateRegionModel>
    {
        public bool Equals(CreateRegionModel x, CreateRegionModel y)
        {
            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the objects' properties are equal.
            return x.Name == y.Name
                   && x.Description == y.Description
                   && x.OsImageContainer == y.OsImageContainer
                   && x.AzureRegionId == y.AzureRegionId;
        }

        public int GetHashCode(CreateRegionModel region)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(region, null)) return 0;

            //Get hash code for each field if it is not null. 
            int hashName = region.Name == null ? 0 : region.Name.GetHashCode();
            int hashDescription = region.Description == null ? 0 : region.Description.GetHashCode();
            int hashOsImageContainer = region.OsImageContainer == null ? 0 : region.OsImageContainer.GetHashCode();
            int hashAzureRegionId = region.AzureRegionId.GetHashCode();

            //Calculate the hash code for the region. 
            return hashName ^ hashDescription ^ hashOsImageContainer ^ hashAzureRegionId;
        }
    }
}
