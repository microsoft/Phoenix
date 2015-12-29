using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models
{
    public class CreateSizeModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVmModel" /> class.
        /// </summary>
        public CreateSizeModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVmModel" /> class.
        /// </summary>
        /// <param name="createSizeFromApi">The domain name from API.</param>
        public CreateSizeModel(VmSize createSizeFromApi)
        {
            this.VmSizeId = createSizeFromApi.VmSizeId;
            this.Name = createSizeFromApi.Name;
            Cores = createSizeFromApi.Cores;
            this.Description = createSizeFromApi.Description;

            this.IsActive = createSizeFromApi.IsActive;
            Memory = createSizeFromApi.Memory;
            MaxDataDiskCount = createSizeFromApi.MaxDataDiskCount;
        }

        /// <summary>
        /// Covert to the API object.
        /// </summary>
        /// <returns>The API DomainName data contract.</returns>
        public VmSize ToApiObject()
        {
            return new VmSize()
            {
                VmSizeId = this.VmSizeId,
                Name = this.Name,
                Description = this.Description,

                Cores = this.Cores,
                IsActive = this.IsActive,
                Memory = this.Memory,
                MaxDataDiskCount = this.MaxDataDiskCount,
            };
        }

        /// <summary> /// </summary>
        public int VmSizeId { get; set; }


        public string Name { get; set; }


        public string Description { get; set; }

        public int Cores { get; set; }

        public int Memory { get; set; }

        public int MaxDataDiskCount { get; set; }

        public bool IsActive { get; set; }
    }
}
