namespace CmpCommon
{
    public class Constants
    {
        public const string CmpAzureServiceWebRole_EventlogSourceName = "CmpWebService";
        public const string CmpAzureServiceWorkerRole_EventlogSourceName = "CmpWorkerService";
        public const string CmpWapConnector_EventlogSourceName = "CmpWapConnector";
        public enum AccountableEnum { Ops, Customer, Capacity }
        public enum Routing { Admin, Customer, Capacity }
    }

    public class VhdInfo
    {
        public string ID;
        public string HostCaching;
        public string DiskLabel;
        public string DiskName;
        public int Lun;
        public int LogicalDiskSizeInGB = 0;
        public string MediaLink;
        public string SourceMediaLink;
    }
}
