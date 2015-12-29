using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models
{
    public class CreateRegionModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVmModel" /> class.
        /// </summary>
        public CreateRegionModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVmModel" /> class.
        /// </summary>
        /// <param name="createSizeFromApi">The domain name from API.</param>
        public CreateRegionModel(Region createSizeFromApi)
        {
            this.Name = createSizeFromApi.Name;
           // this.DisplayName = createSizeFromApi.Description;
            //this.Config = createSizeFromApi.Config;
            this.Description = createSizeFromApi.Description;
            this.AzureRegionId = createSizeFromApi.AzureRegionId;
            this.IsActive = createSizeFromApi.IsActive;
            this.OsImageContainer = createSizeFromApi.OsImageContainer;
           // this.TagData = createSizeFromApi.TagData;
            this.CreatedOn = createSizeFromApi.CreatedOn;
            this.CreatedBy = createSizeFromApi.CreatedBy;
            this.LastUpdatedOn = createSizeFromApi.LastUpdatedOn;
            this.LastUpdatedBy = createSizeFromApi.LastUpdatedBy;
        }

        /// <summary>
        /// Covert to the API object.
        /// </summary>
        /// <returns>The API DomainName data contract.</returns>
        public Region ToApiObject()
        {
            return new Region()
            {
             Name = this.Name,
             Description = this.Description,
             AzureRegionId = this.AzureRegionId,
             IsActive = this.IsActive,
             OsImageContainer = this.OsImageContainer,
             CreatedOn = this.CreatedOn,
             CreatedBy = this.CreatedBy,
             LastUpdatedOn = this.LastUpdatedOn,
             LastUpdatedBy = this.LastUpdatedBy
            };
        }

        /// <summary> /// </summary>
        public string Name { get; set; }

        /// <summary> </summary>
        //public string DisplayName { get; set; }

        ///// <summary> </summary>
        //public string Config { get; set; }

        /// <summary> </summary>
        public string Description { get; set; }

        /// <summary> </summary>
        public int AzureRegionId { get; set; }

        /// <summary> </summary>
        public bool IsActive { get; set; }

        public string OsImageContainer { get; set; }

        ///// <summary> </summary>
        //public string TagData { get; set; }

        public System.DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime LastUpdatedOn { get; set; }

        public string LastUpdatedBy { get; set; }
    }
}
