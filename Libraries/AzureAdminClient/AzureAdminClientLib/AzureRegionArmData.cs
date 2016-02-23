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
}
