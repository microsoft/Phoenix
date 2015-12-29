using CmpInterfaceModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AzureAdminClientLib
{
    [DataContract]
    public class AzureLocationArmData
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }
        [DataMember(Name = "longitude")]
        public string Longitude { get; set; }
        [DataMember(Name = "latitude")]
        public string Latitude { get; set; }
    }

    public class LocationComparer : IEqualityComparer<AzureLocationArmData>
    {
        //Locations are equal if their properties are equal. 
        public bool Equals(AzureLocationArmData x, AzureLocationArmData y)
        {
            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the locations' properties are equal. 
            return x.Id == y.Id
                && x.DisplayName == y.DisplayName
                && x.Name == y.Name
                && x.Longitude == y.Longitude
                && x.Latitude == y.Latitude;
        }

        public int GetHashCode(AzureLocationArmData location)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(location, null)) return 0;

            //Get hash code for each field if it is not null. 
            int hashName = location.Name == null ? 0 : location.Name.GetHashCode();
            int hashDisplayName = location.DisplayName == null ? 0 : location.DisplayName.GetHashCode();
            int hashVmRoleSizesCount = location.Latitude.GetHashCode();
            int hashLongitudeCount = location.Longitude.GetHashCode();

            //Calculate the hash code for the location. 
            return hashName ^ hashDisplayName ^ hashVmRoleSizesCount ^ hashLongitudeCount;
        }
    }

    public class AzureRegionOps
    {
        const string URLTEMPLATE_GETAZUREREGIONS = "https://management.azure.com/subscriptions/{0}/locations?api-version=2015-11-01";
        const string URLTEMPLATE_GETAZUREVMSIZES = "https://management.azure.com/subscriptions/{0}/providers/Microsoft.Compute/locations/{1}/vmSizes?api-version=2015-06-15";
        const string URLTEMPLATE_GETPUBLISHERS = "https://management.azure.com/subscriptions/{0}/providers/Microsoft.Compute/locations/{1}/publishers?api-version=2015-06-15";
        const string URLTEMPLATE_GETOFFERS = "https://management.azure.com/subscriptions/{0}/providers/Microsoft.Compute/locations/{1}/publishers/{2}/artifacttypes/vmimage/offers?api-version=2015-06-15";
        const string URLTEMPLATE_GETSKUS = "https://management.azure.com//subscriptions/{0}/providers/Microsoft.Compute/locations/{1}/publishers/{2}/artifacttypes/vmimage/offers/{3}/skus?api-version=2015-06-15";

        private readonly IConnection _Connection = null;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// 
        //*********************************************************************

        public AzureRegionOps(IConnection connection)
        {
            _Connection = connection;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Makes a call to Azure API to get a set of Location objects based on
        /// a subscription ID. 
        /// </summary>
        /// <returns>A list of Location objects</returns>
        /// 
        //*********************************************************************

        public IEnumerable<AzureLocationArmData> GetAzureLocationsList()
        {
            var url = string.Format(URLTEMPLATE_GETAZUREREGIONS, _Connection.SubcriptionID);
            var hi = new HttpInterface(_Connection);
            HttpResponse response = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url);
            int startIndex = response.Body.IndexOf('[');
            int endIndex = response.Body.IndexOf(']');
            string processedJsonString = response.Body.Substring(startIndex, endIndex - startIndex + 1);
            return Utilities.DeSerializeJson<IEnumerable<AzureLocationArmData>>(processedJsonString);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Makes a call to Azure API to get a set of VmSize objects based on
        /// a region 
        /// </summary>
        /// <returns>A list of VmSize objects</returns>
        /// 
        //*********************************************************************
        public IEnumerable<AzureVmSizeArmData> GetVmSizeList(string location)
        {
            var url = string.Format(URLTEMPLATE_GETAZUREVMSIZES, _Connection.SubcriptionID, location);
            var hi = new HttpInterface(_Connection);
            HttpResponse response = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url);
            int startIndex = response.Body.IndexOf('[');
            int endIndex = response.Body.IndexOf(']');
            string processedJsonString = response.Body.Substring(startIndex, endIndex - startIndex + 1);
            return AzureVmSizeArmData.DeserializeJsonVmSize(processedJsonString);
        }

        public IEnumerable<AzurePublisher> GetPublisherList(string location)
        {
            var url = string.Format(URLTEMPLATE_GETPUBLISHERS, _Connection.SubcriptionID, location);
            var hi = new HttpInterface(_Connection);
            HttpResponse response = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url);
            int startIndex = response.Body.IndexOf('[');
            int endIndex = response.Body.IndexOf(']');
            string jsonString = response.Body.Substring(startIndex, endIndex - startIndex + 1);
            return Utilities.DeSerializeJson<IEnumerable<AzurePublisher>>(jsonString);
        }

        public IEnumerable<AzureOffer> GetOfferList(string location, string publisher)
        {
            var url = string.Format(URLTEMPLATE_GETOFFERS, _Connection.SubcriptionID, location, publisher);
            var hi = new HttpInterface(_Connection);
            HttpResponse response = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url);
            int startIndex = response.Body.IndexOf('[');
            int endIndex = response.Body.IndexOf(']');
            string jsonString = response.Body.Substring(startIndex, endIndex - startIndex + 1);
            return Utilities.DeSerializeJson<IEnumerable<AzureOffer>>(jsonString);
        }

        public IEnumerable<AzureSku> GetSKUList(string location, string publisher, string offer)
        {
            var url = string.Format(URLTEMPLATE_GETSKUS, _Connection.SubcriptionID, location, publisher, offer);
            var hi = new HttpInterface(_Connection);
            HttpResponse response = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url);
            int startIndex = response.Body.IndexOf('[');
            int endIndex = response.Body.IndexOf(']');
            string jsonString = response.Body.Substring(startIndex, endIndex - startIndex + 1);
            return Utilities.DeSerializeJson<IEnumerable<AzureSku>>(jsonString);
        }
    }
}
