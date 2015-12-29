using System;
namespace CmpInterfaceModel.Models
{
    public enum PlacementMethodEnum { Undefined, Manual, AutoDefault }
    public class PlacementSpec
    {
        public string Method { set; get; }
        public string Config { set; get; }
        public string TagData { set; get; }
        public int TargetServiceProviderAccountID { set; get; }
        public string TargetServiceProviderAccountGroup { set; get; }
        //public string TargetServiceProviderStorageUrl { set; get; }
        public string ServerOu { set; get; }
        public string WorkstationOu { set; get; }

        public string StorageContainerUrl { set; get; }
        public string AffinityGroup { set; get; }
        public string Location { set; get; }
        public string VNet { set; get; }
        public string Subnet { set; get; }
        public int DiskCount { set; get; }
        public bool RebuildRequest { set; get; }
    }

    public class PlacementConfig
    {
        public bool UseStaticIpAddr { get; set; }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public string Serialize()
        {
            return Utilities.Serialize(typeof(PlacementConfig), this);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static PlacementConfig Deserialize(string input)
        {
            try
            {
                return Utilities.DeSerialize(typeof(PlacementConfig), input, true) as PlacementConfig;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VmConfig.Deserialize() : Unable to deserialize given PlacementConfig structure, may be malformed : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }
    }

}
