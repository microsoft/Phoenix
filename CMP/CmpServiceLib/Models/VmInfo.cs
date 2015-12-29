using CmpInterfaceModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmpServiceLib.Models
{
    public class VmInfo
    {

   
        public IList<DataVirtualHardDisk> DataVirtualHardDisks { get; set; }
       
        public string RDPCertificateThumbprint { get; set; }

        public string InternalIP { get; set; }

        public string Status {get;set;}
       
        public Uri MediaLocation { get; set; }
      
        public string OSVersion { get; set; }
      
        public OsVirtualHardDisk OSVirtualHardDisk { get; set; }
       
        public string RoleName { get; set; }
      
        public string RoleSize { get; set; }

        public string DNSName { get; set; }

        public string DeploymentID { get; set; }

        public SubscriptionInfo Subscription { get; set;}


       //The below data is used for Usage bar in Wap Vm Dashboard
        //public string CoreCount { get; set; }

        //public string CurrentSubscriptionCoreCount { get; set; }

        //public string CurrentSubscriptionMaxCoreCount {get;set;}

        //public string SubscriptionName { get; set; }

        //public string SubscriptionId { get; set; }
    }
}
