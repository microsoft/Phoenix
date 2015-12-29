using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Represents a network interface controller
    /// </summary>
    public class NetworkNICModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkNICModel" /> class.
        /// </summary>
        public NetworkNICModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkNICModel" /> class.
        /// </summary>
        /// <param name="createNetworkNIC">The controller from API.</param>
        public NetworkNICModel(NetworkNIC createNetworkNIC)
        {
            this.NetworkNICId = createNetworkNIC.NetworkNICId;
            this.Name = createNetworkNIC.Name;
            this.Description = createNetworkNIC.Description;
            this.IsActive = createNetworkNIC.IsActive;
            this.ADDomain = createNetworkNIC.ADDomain;
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API controller data contract.</returns>
        public NetworkNIC ToApiObject()
        {
            return new NetworkNIC()
            {
            NetworkNICId = this.NetworkNICId,
            Name = this.Name,
            Description = this.Description,
            IsActive = this.IsActive,
            ADDomain = this.ADDomain
            };
        }
        
        /// <summary>
        /// The ID of the NIC
        /// </summary>
        public int NetworkNICId { get; set; }

        /// <summary>
        /// The name of the NIC
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the NIC
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Whether or not the NIC is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The Active Directory domain associated with the NIC
        /// </summary>
        public string ADDomain { get; set; }
    }
}
