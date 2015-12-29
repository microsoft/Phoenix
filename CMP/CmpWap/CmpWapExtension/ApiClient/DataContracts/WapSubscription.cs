using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
     [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
    public class WapSubscription
    {
         [DataMember(Order = 1)]
         public string AdminEmailId { get; set; }
         [DataMember(Order = 2)]
         public IList<string> CoAdminNames { get; set; }

         [DataMember(Order = 3)]
         public string OfferFriendlyName { get; set; }
         [DataMember(Order = 4)]
         public string PlanId { get; set; }

         [DataMember(Order = 5)]
         public string SubscriptionID { get; set; }
         [DataMember(Order = 6)]
         public string SubscriptionName { get; set; }
    }
}
