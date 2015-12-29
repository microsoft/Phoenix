using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
    /// <summary>
    /// This is a data contract class between extensions and resource provider
    /// CreateVM contains data contract of data which shows up in "VMs" tab inside Cmp Tenant Extension
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
    public class ServerRole
    {
        [DataMember(Order = 1)]
        public int ServerRoleId { get; set; }
        /// <summary>
        /// Name of the file share
        /// </summary>
        [DataMember(Order = 2)]
        public string Name { get; set; }


        /// <summary>
        /// Name of the file share
        /// </summary>
        [DataMember(Order = 3)]
        public string Description { get; set; }

        /// <summary>
        /// SubscriptionId of user who created this file share
        /// </summary>
        [DataMember(Order = 4)]
        public bool IsActive { get; set; }

      



    }
}
