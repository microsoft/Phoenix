using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
     [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
  public class SQLAnalysisServiceModes
    {
        [DataMember(Order = 1)]
         public int SQLAnalysisServicesModeId { get; set; }
        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string Description { get; set; }
    }
}
