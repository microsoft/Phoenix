using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Represents a drive mapping for server roles
    /// </summary>
    public class ServerRoleDriveMappingModel
    {

       public const string RegisteredStatus = "Registered";

        /// <summary>
       /// Initializes a new instance of the <see cref="ServerRoleDriveMappingModel" /> class.
        /// </summary>
        public ServerRoleDriveMappingModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerRoleDriveMappingModel" /> class.
        /// </summary>
        /// <param name="createSRDM">The domain name from API.</param>
        public ServerRoleDriveMappingModel(ServerRoleDriveMapping createSRDM)
        {
            this.Id = createSRDM.Id;
            this.ServerRoleId = createSRDM.ServerRoleId;
            this.Drive = createSRDM.Drive;
            this.MemoryInGB = createSRDM.MemoryInGB;
            
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API mapping data contract.</returns>
        public ServerRoleDriveMapping ToApiObject()
        {
            return new ServerRoleDriveMapping()
            {
               Id = this.Id,
            ServerRoleId = this.ServerRoleId,
            Drive = this.Drive,
            MemoryInGB = this.MemoryInGB
            
            };
        }

        /// <summary>
        /// The ID of the mapping
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The ID of the server role
        /// </summary>
        public int ServerRoleId { get; set; }

        /// <summary>
        /// Name of the drive
        /// </summary>
        public string Drive { get; set; }

        /// <summary>
        /// Size of the memory in the mapping in gigabytes
        /// </summary>
        public int MemoryInGB { get; set; }
    }
}
