namespace CmpInterfaceModel
{
    public class Constants
    {
        public enum StatusEnum
        {
            Uninitialized, Submitted, QcDataGathering, QcVmRequest, QcVmRequestPassed, 
            ReadyForConversion, Converting, Converted, ReadyForTransfer, Transferring, 
            Transferred, CreatingImage, ReadyToCreateService, CreatingService, 
            ReadyForUploadingServiceCert, UploadingServiceCert, ReadyForCreatingVM, 
            CreatingVM, CreatedVM, ContactingVM, MovingPagefile, WaitForReboot1,
            CreatingDrives, InstallingIpak, StartingSequences, RunningSequences, 
            PostProcessing1, PostProvisioningQC, CmdbUpdate, Complete, Exception,
            Rejected, Retrying, Agedout, Processing}
        public enum VmSizeEnum { VmSizeCustom, ExtraSmall, Small, Medium, Large, ExtraLarge, A5, A6, A7 }
        public enum TargetLocationTypeEnum { Region, AffinityGroup, Vnet }
        public enum TargetServiceProviderTypeEnum { Azure }
        public enum ContainerTypeEnum { Service, ResourceGroup }
        public enum TargetAccountTypeEnum { AzureSubscription }
        public enum RequestTypeEnum { NewVM, MigrateVm, SyncVm }
        public enum AftsDestinationTypeEnum { BLOB }
        public enum AftsSourceTypeEnum { FILE }
        public enum AftsTransferTypeEnum { FILETOBLOB }
        public enum RequestExceptionTypeCodeEnum { Undefined, Customer, Capacity, Admin }
        public enum VmOpcodeEnum { Undefined, DELETE, DELETEFROMSTORAGE, RESTART, STOP, START, DEALLOCATE, RESIZE, ADDISK, GET, DETACH, DETACHANDDELETE, ATTACHEXISTING, DELETEONEXCEPTION }
        public enum ClinetAuthMechanismEnum { Undefined, Basic, ClientCert }
        public enum AssetTypeCodeEnum { Undefined, Subscription, Vm, Vnet, Subnet, Hostservice, Datastore, Datacontainer }
        public enum OsFamily { Undefined, Windows, Linux }
        public enum AzureApiType { Undefined, RDFE, ARM }
        public enum BuildMigratedVmConfigStringResultEnum { Success, MissingDisk }
        public enum PostProvInintDisksResultEnum { Success, FailToConnect, NotFound }

        public const string AUTOBLOBSTORELOCATION = "[AUTOBLOBSTORELOCATION]";
        public const string AUTOLOCALADMINUSERNAME = "[AUTOLOCALADMINUSERNAME]";
        public const string AUTOLOCALADMINPASSWORD = "[AUTOLOCALADMINPASSWORD]";
        public const string AUTOAFFINITYGROUP = "[AUTOAFFINITYGROUP]";
        public const string AUTOLOCATION = "[AUTOLOCATION]";
        public const string AUTOVNET = "[AUTOVNET]";
        public const string AUTOSUBNETNAME = "[AUTOSUBNETNAME]";
        public const string AUTOSTORAGEACCOUNTNAME = "[AUTOSTORAGEACCOUNTNAME]";
        public const string AUTOSTORAGECONTAINERNAME = "[AUTOSTORAGECONTAINERNAME]";
        //public const string AUTOSTATICIPADDRESS = "[AUTOSTATICIPADDRESS]";

        public const string AZUREAPIVERSION = "2015-01-01";
        public const string ARMMANAGEMENTADDRESS = "management.azure.com";
    }
}
