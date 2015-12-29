using System;
using System.Collections.Generic;
using CmpInterfaceModel;
using System.Runtime.Serialization;

namespace AzureAdminClientLib
{
    [DataContract]
    public class AzureVmSizeArmData
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public int numberOfCores { get; set; }
        [DataMember]
        public int osDiskSizeInMB { get; set; }
        [DataMember]
        public int resourceDiskSizeInMB { get; set; }
        [DataMember]
        public int memoryInMB { get; set; }
        [DataMember]
        public int maxDataDiskCount { get; set; }

        public static IEnumerable<AzureVmSizeArmData> DeserializeJsonVmSize(string jsonString)
        {
            try
            {
                return Utilities.DeSerializeJson<List<AzureVmSizeArmData>>(jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception caught in DeserializeJsonVmSize: " + ex.ToString());
            }
        }
        public class AzureVmSizeComparer : IEqualityComparer<AzureVmSizeArmData>
        {
            public bool Equals(AzureVmSizeArmData x, AzureVmSizeArmData y)
            {
                //Check whether the compared objects reference the same data. 
                if (Object.ReferenceEquals(x, y)) return true;

                //Check whether any of the compared objects is null. 
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                //Check whether the locations' properties are equal.
                return x.name == y.name
                    && x.numberOfCores == y.numberOfCores
                    && x.osDiskSizeInMB == y.osDiskSizeInMB
                    && x.resourceDiskSizeInMB == y.resourceDiskSizeInMB
                    && x.memoryInMB == y.memoryInMB
                    && x.maxDataDiskCount == y.maxDataDiskCount;
            }

            public int GetHashCode(AzureVmSizeArmData vmSize)
            {
                //Check whether the object is null 
                if (Object.ReferenceEquals(vmSize, null)) return 0;

                //Get hash code for each field if it is not null. 
                int hashName = vmSize.name == null ? 0 : vmSize.name.GetHashCode();
                int hashCores = vmSize.numberOfCores == null ? 0 : vmSize.numberOfCores.GetHashCode();
                int hashOSDiskSize = vmSize.osDiskSizeInMB == null ? 0 : vmSize.osDiskSizeInMB.GetHashCode();
                int hashResourceDiskSize = vmSize.resourceDiskSizeInMB == null ? 0 : vmSize.resourceDiskSizeInMB.GetHashCode();
                int hashMemory = vmSize.memoryInMB == null ? 0 : vmSize.memoryInMB.GetHashCode();
                int hashMaxDataDiskCount = vmSize.maxDataDiskCount == null ? 0 : vmSize.maxDataDiskCount.GetHashCode();

                //Calculate the hash code for the vmSize. 
                return hashName ^ hashCores ^ hashOSDiskSize ^ hashResourceDiskSize ^ hashMemory ^ hashMaxDataDiskCount;
            }
        }
    }
}
