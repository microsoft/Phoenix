using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CmpInterfaceModel;

namespace AzureAdminClientLib.DataContracts
{
    [DataContract]
    public class InstanceViewStatus
    {
        [DataMember(Name = "statuses")]
        public List<Status> Statuses { get; set; }
    }

    [DataContract]
    public class Status
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }
        [DataMember(Name = "level")]
        public string Level { get; set; }
        [DataMember(Name = "displayStatus")]
        public string DisplayStatus { get; set; }
        [DataMember(Name = "time")]
        public string Time { get; set; }

        public static InstanceViewStatus DeserializeJsonInstanceViewStatus(string jsonString)
        {
            try
            {
                return Utilities.DeSerializeJson<InstanceViewStatus>(jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception caught in DeserializeJsonVmSize: " + ex.ToString());
            }
        }
    }
}
