using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace AzureAdminClientLib
{
    [DataContract]
    public class AzureOffer
    {
        [DataMember]
        public string location { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string id { get; set; }

        public class AzureOfferComparer : IEqualityComparer<AzureOffer>
        {
            public bool Equals(AzureOffer x, AzureOffer y)
            {
                //Check whether the compared objects reference the same data. 
                if (Object.ReferenceEquals(x, y)) return true;

                //Check whether any of the compared objects is null. 
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                return x.location == y.location
                    && x.name == y.name
                    && x.id == y.id;
            }

            public int GetHashCode(AzureOffer offer)
            {
                //Check whether the object is null 
                if (Object.ReferenceEquals(offer, null)) return 0;

                int hashLocation = offer.name == null ? 0 : offer.name.GetHashCode();
                int hashName = offer.name == null ? 0 : offer.name.GetHashCode();
                int hashId = offer.name == null ? 0 : offer.name.GetHashCode();

                return hashLocation ^ hashName ^ hashId;
            }
        }
    }
}
