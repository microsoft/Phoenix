using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AzureAdminClientLib
{
    [DataContract]
    public class AzurePublisher
    {
        [DataMember]
        public string location { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string id { get; set; }

        public class AzurePublisherComparer : IEqualityComparer<AzurePublisher>
        {
            public bool Equals(AzurePublisher x, AzurePublisher y)
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

            public int GetHashCode(AzurePublisher publisher)
            {
                //Check whether the object is null 
                if (Object.ReferenceEquals(publisher, null)) return 0;

                int hashLocation = publisher.name == null ? 0 : publisher.name.GetHashCode();
                int hashName = publisher.name == null ? 0 : publisher.name.GetHashCode();
                int hashId = publisher.name == null ? 0 : publisher.name.GetHashCode();

                return hashLocation ^ hashName ^ hashId;
            }
        }
    }
}
