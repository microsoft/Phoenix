using System;

namespace CmpInterfaceModel.Models
{
    public class VM
    {
        public int ID { get; set; }
        public string RequestName { get; set; }
        public string RequestDescription { get; set; }
        public string ParentAppName { get; set; }

        public string VmMachineName { get; set; }

        public string RequestConfig { get; set; }
        public string RequestTagData { get; set; }

        //public Image TheImage { get; set; }
        public string SourceServer { get; set; }
        public string SourceFile { get; set; }
        public string SourceBlob { get; set; }
        public string SourceImage { get; set; }
        public string TargetAccountName { get; set; }
        public string VmSizeCustom { get; set; }

        public string CloudService { get; set; }
        public string Subscription { get; set; }
        public string TargetLocation { get; set; }
        public string StorageAccount { get; set; }
        public string AvailabilitySet { get; set; }

        public string VmAdminName { get; set; }
        public string VmAdminPassword { get; set; }

        public string WhoRequested { get; set; }
        public DateTime WhenRequested { get; set; }

        /*public enum TargetLocationTypeEnum { Region, AffinityGroup, Vnet }
        public TargetLocationTypeEnum TargetLocationType { get; set; }

        public enum TargetAccountTypeEnum { Azure, MSIT }
        public TargetAccountTypeEnum TargetAccountType { get; set; }

        public enum VmSizeEnum { VmSizeCustom, ExtraSmall, Small, Medium, Large, ExtraLarge, A6, A7 }
        public VmSizeEnum VmSize { get; set; }*/
    }
}
