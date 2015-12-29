using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Model for the server role
    /// </summary>
    public class CreateServerRoleModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateServerRoleModel" /> class.
        /// </summary>
        public CreateServerRoleModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateServerRoleModel" /> class.
        /// </summary>
        /// <param name="createServerRoleFromApi">The server role from API.</param>
        public CreateServerRoleModel(ServerRole createServerRoleFromApi)
        {
            this.ServerRoleId = createServerRoleFromApi.ServerRoleId;
            this.Name = createServerRoleFromApi.Name;
            this.Description = createServerRoleFromApi.Description;
            this.IsActive = createServerRoleFromApi.IsActive;
            
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API server role data contract.</returns>
        public ServerRole ToApiObject()
        {
            return new ServerRole()
            {
            ServerRoleId = this.ServerRoleId,
            Name = this.Name,
            Description = this.Description,
            IsActive = this.IsActive,
            
            };
        }

        /// <summary>
        /// ID of the server role
        /// </summary>
        public int ServerRoleId { get; set; }
        /// <summary>
        /// Name of the server role
        /// </summary>
       
        public string Name { get; set; }


        /// <summary>
        /// Description of the server role
        /// </summary>
   
        public string Description { get; set; }

        /// <summary>
        /// Whether or not the server role is active
        /// </summary> 
        public bool IsActive { get; set; }

       
    }
}
