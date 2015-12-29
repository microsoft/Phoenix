using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{

      [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
   public class ResourceGroup
    {
        [DataMember(Order = 1)]
        public int ResourceProviderAcctGroupId { get; set; }

       [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public int DomainId { get; set; }
        [DataMember(Order = 4)]
        public int NetworkNICId { get; set; }
        [DataMember(Order = 5)]
        public int EnvironmentTypeId { get; set; }
        
    }
}
