using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{

    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
   public class ServiceCategory
    {
        /// Name of the file share
        /// </summary>
        [DataMember(Order = 1)]
        public int ServiceCategoryId { get; set; }


        /// <summary>
        /// Name of the file share
        /// </summary>
        [DataMember(Order = 2)]
        public string Name { get; set; }

        /// <summary>
        /// SubscriptionId of user who created this file share
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
