using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
   public class VmDashboardInfo
    {
         [DataMember(Order = 1)]
        public IList<DataVirtualHardDisk> DataVirtualHardDisks { get; set; }

         [DataMember(Order = 2)]
        public string RDPCertificateThumbprint { get; set; }

         [DataMember(Order = 3)]
        public string InternalIP { get; set; }

         [DataMember(Order = 4)]
        public string Status { get; set; }

         [DataMember(Order = 5)]
        public Uri MediaLocation { get; set; }
         [DataMember(Order = 6)]
        public string OSVersion { get; set; }
         [DataMember(Order = 7)]
        public OsVirtualHardDisk OSVirtualHardDisk { get; set; }
         [DataMember(Order = 8)]
        public string RoleName { get; set; }
         [DataMember(Order = 9)]
        public string RoleSize { get; set; }
         [DataMember(Order = 10)]
        public string DNSName { get; set; }
         [DataMember(Order = 11)]
        public string DeploymentID { get; set; }
         [DataMember(Order = 12)]
        public SubscriptionInfo Subscription { get; set; }
         [DataMember(Order = 13)]
        public string Cores { get; set; }
         [DataMember(Order = 14)]
         public string QueueStatus { get; set; }
    }

    public class DataVirtualHardDisk
    {
        [DataMember(Order = 1)]
        public string HostCaching { get; set; }
         [DataMember(Order = 2)]
        public string DiskLabel { get; set; }
         [DataMember(Order = 3)]
        public string DiskName { get; set; }
         [DataMember(Order = 4)]
        public string Lun { get; set; }
         [DataMember(Order = 5)]
        public string LogicalDiskSizeInGB { get; set; }
         [DataMember(Order = 6)]
        public string MediaLink { get; set; }
         [DataMember(Order = 7)]
        public string SourceMediaLink { get; set; }
         [DataMember(Order = 7)]
         public string Type { get; set; } 

    }

    public class OsVirtualHardDisk
    {
         [DataMember(Order = 1)]
        public string HostCaching { get; set; }
         [DataMember(Order = 2)]
        public string DiskLabel { get; set; }
         [DataMember(Order = 3)]
        public string DiskName { get; set; }
         [DataMember(Order = 4)]
        public string MediaLink { get; set; }
         [DataMember(Order = 5)]
        public string SourceImageName { get; set; }
         [DataMember(Order = 6)]
        public string OS { get; set; }
         [DataMember(Order = 7)]
        public string RemoteSourceImageLink { get; set; }

         [DataMember(Order = 7)]
         public string Type { get; set; } 
    }


    public class SubscriptionInfo
    {
         [DataMember(Order = 1)]
        public string CurrentCoreCount { get; set; }
         [DataMember(Order = 2)]
        public string MaximumCoreCount { get; set; }

         [DataMember(Order = 3)]
        public string SubscriptionID { get; set; }
         [DataMember(Order = 4)]
        public string SubscriptionName { get; set; }
    }
}
