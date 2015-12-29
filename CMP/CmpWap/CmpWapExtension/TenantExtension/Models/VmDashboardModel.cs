using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Represents data for displaying on the RP dashboard
    /// </summary>
    public class VmDashboardModel
    {
         public const string RegisteredStatus = "Registered";

      

        /// <summary>
        /// Initializes a new instance of the <see cref="VmDashboardModel" /> class.
        /// </summary>
        public VmDashboardModel()
        {
           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VmDashboardModel" /> class.
        /// </summary>
        /// <param name="createvmApi">The virtual machine from API.</param>
        public VmDashboardModel(VmDashboardInfo createvmApi)
        {
            this.DataVirtualHardDisks = createvmApi.DataVirtualHardDisks;
            this.OSVirtualHardDisk = createvmApi.OSVirtualHardDisk;
            this.RDPCertificateThumbprint = createvmApi.RDPCertificateThumbprint;
            this.RoleName = createvmApi.RoleName;
            this.RoleSize = createvmApi.RoleSize;
            this.Status = createvmApi.Status;
            this.Subscription = createvmApi.Subscription;
            this.Cores = createvmApi.Cores;
            this.DeploymentID = createvmApi.DeploymentID;
            this.DNSName = createvmApi.DNSName;
            this.InternalIP = createvmApi.InternalIP;
          
        }
        
        /// <summary>
        /// Data disks for the virtual machine
        /// </summary>
        public IList<DataVirtualHardDisk> DataVirtualHardDisks { get; set; }

        /// <summary>
        /// The Remote Desktop Protocol certificate thumbprint for the virtual machine
        /// </summary>
        public string RDPCertificateThumbprint { get; set; }

        /// <summary>
        /// The internal IP address of the virtual machine
        /// </summary>
        public string InternalIP { get; set; }

        /// <summary>
        /// The status of the virtual machine
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The media location of the virtual machine
        /// </summary>
        public Uri MediaLocation { get; set; }
        
        /// <summary>
        /// The virtual machine's operating system version
        /// </summary>
        public string OSVersion { get; set; }
        
        /// <summary>
        /// The operating system disk of the virtual machine
        /// </summary>
        public OsVirtualHardDisk OSVirtualHardDisk { get; set; }
        
        /// <summary>
        /// The name of the virtual machine's role
        /// </summary>
        public string RoleName { get; set; }
        
        /// <summary>
        /// The size of the virtual machine's role
        /// </summary>
        public string RoleSize { get; set; }
        
        /// <summary>
        /// The virtual machine's domain name system
        /// </summary>
        public string DNSName { get; set; }
        
        /// <summary>
        /// The virtual machine's deployment ID
        /// </summary>
        public string DeploymentID { get; set; }
        
        /// <summary>
        /// The subscription associated with the virtual machine
        /// </summary>
        public SubscriptionInfo Subscription { get; set; }

        /// <summary>
        /// The number of cores in the virtual machine
        /// </summary>
        public string Cores { get; set; }
    }
}
