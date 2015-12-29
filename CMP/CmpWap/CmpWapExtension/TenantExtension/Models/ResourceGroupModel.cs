using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Represents a resource group
    /// </summary>
    public class ResourceGroupModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceGroupModel" /> class.
        /// </summary>
        public ResourceGroupModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceGroupModel" /> class.
        /// </summary>
        /// <param name="resourceGroup">The group from API.</param>
        public ResourceGroupModel(ResourceGroup resourceGroup)
        {
            this.ResourceProviderAcctGroupId = resourceGroup.ResourceProviderAcctGroupId;
            this.Name = resourceGroup.Name;
            this.DomainId = resourceGroup.DomainId;
            this.EnvironmentTypeId = resourceGroup.EnvironmentTypeId;
            this.NetworkNICId = resourceGroup.NetworkNICId;
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API group data contract.</returns>
        public ResourceGroup ToApiObject()
        {
            return new ResourceGroup()
            {
                ResourceProviderAcctGroupId = this.ResourceProviderAcctGroupId,
                Name = this.Name,
                DomainId = this.DomainId,
                EnvironmentTypeId = this.EnvironmentTypeId,
                NetworkNICId = this.NetworkNICId
            };
        }

        /// <summary>
        /// The ID of the RP account group
        /// </summary>
        public int ResourceProviderAcctGroupId { get; set; }

        /// <summary>
        /// The name of the group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ID of the group's domain
        /// </summary>
        public int DomainId { get; set; }
        
        /// <summary>
        /// The ID of the group's network interface controller
        /// </summary>
        public int NetworkNICId { get; set; }
        
        /// <summary>
        /// The ID of the group's environment type
        /// </summary>
        public int EnvironmentTypeId { get; set; }
    }
}
