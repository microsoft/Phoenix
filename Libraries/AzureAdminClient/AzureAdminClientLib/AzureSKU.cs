using System.Runtime.Serialization;

namespace AzureAdminClientLib
{
    [DataContract]
    public class AzureSku
    {
        [DataMember]
        public string location { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string id { get; set; }
    }
}
