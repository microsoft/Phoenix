using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models
{
    public class CreateVmModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVmModel" /> class.
        /// </summary>
        public CreateVmModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVmModel" /> class.
        /// </summary>
        /// <param name="createVmFromApi">The domain name from API.</param>
        public CreateVmModel(CreateVm createVmFromApi)
        {
            this.Id = createVmFromApi.Id;
            this.CmpRequestId = createVmFromApi.CmpRequestId;
            this.Name = createVmFromApi.Name;
            this.SubscriptionId = createVmFromApi.SubscriptionId;
            this.VmAppName = createVmFromApi.VmAppName;
            this.VmDomain = createVmFromApi.VmDomain;
            this.VmAdminName = createVmFromApi.VmAdminName;
          //  this.VmAdminPassword = createVmFromApi.VmAdminPassword;
            this.VmSourceImage = createVmFromApi.VmSourceImage;
            this.VmSize = createVmFromApi.VmSize;
            this.VmRegion = createVmFromApi.VmRegion;
            this.VmRole = createVmFromApi.VmRole;
            this.VmDiskSpec = createVmFromApi.VmDiskSpec;
            this.VmConfig = createVmFromApi.VmConfig;
            this.VmTagData = createVmFromApi.VmTagData;
            this.StatusCode = createVmFromApi.StatusCode;
            this.StatusMessage = createVmFromApi.StatusMessage;
            this.AddressFromVm = createVmFromApi.AddressFromVm;
        }

        /// <summary>
        /// Covert to the API object.
        /// </summary>
        /// <returns>The API DomainName data contract.</returns>
        public CreateVm ToApiObject()
        {
            return new CreateVm()
            {
                Id = this.Id,
                CmpRequestId = this.CmpRequestId,
                Name = this.Name,
                SubscriptionId = this.SubscriptionId,
                VmAppName = this.VmAppName,
                VmDomain = this.VmDomain,
                VmAdminName = this.VmAdminName,
             //   VmAdminPassword = this.VmAdminPassword,
                VmSourceImage = this.VmSourceImage,
                VmSize = this.VmSize,
                VmRegion = this.VmRegion,
                VmRole = this.VmRole,
                VmDiskSpec = this.VmDiskSpec,
                VmConfig = this.VmConfig,
                VmTagData = this.VmTagData,
                StatusCode = this.StatusCode,
                StatusMessage = this.StatusMessage,
                AddressFromVm = this.AddressFromVm
            };
        }

        /// <summary>
        /// Gets or sets the name.
        // </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        // </summary>
        public int CmpRequestId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        // </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name.
        // </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the value of the display name of the file server 
        /// </summary>
        public string VmAppName { get; set; }

        /// <summary>
        /// Gets or sets the value of the subscription id
        /// </summary>
        public string VmDomain { get; set; }

        /// <summary>
        /// Gets or sets the value of the admin name
        /// </summary>
        public string VmAdminName { get; set; }

        /// <summary>
        /// Gets or sets the value of the admin password
        /// </summary>
        public string VmAdminPassword { get; set; }

        /// <summary>
        /// Gets or sets the value of the file share size
        /// </summary>
        public string VmSourceImage { get; set; }

        /// <summary>
        /// Gets or sets the value of the file share size
        /// </summary>
        public string VmSize { get; set; }

        /// <summary>
        /// Gets or sets the value of the file share size
        /// </summary>
        public string VmRegion { get; set; }

        /// <summary>
        /// Gets or sets the value of the file share size
        /// </summary>
        public string VmRole { get; set; }

        /// <summary>
        /// Gets or sets the value of the file share size
        /// </summary>
        public string VmDiskSpec { get; set; }

        /// <summary>
        /// Gets or sets the value of the file share size
        /// </summary>
        public string VmConfig { get; set; }

        /// <summary>
        /// Gets or sets the value of the file share size
        /// </summary>
        public string VmTagData { get; set; }

        /// <summary> </summary>
        public string StatusCode { get; set; }

        /// <summary>  /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>  /// </summary>
        public string AddressFromVm { get; set; }

    }
}
