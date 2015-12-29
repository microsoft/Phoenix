using CmpInterfaceModel.Models;
using System.Collections.Generic;

namespace AzureAdminClientLib
{
    public class VmRole
    {
        public VmRole(string roleName, string roleSize, IList<DataVirtualHardDisk> dataVirtualHardDisks, OsVirtualHardDisk osVirtualHardDisk, Deployment deployment,SubscriptionInfo subscription)
        {
            RoleName = roleName;
            DataVirtualHardDisks = dataVirtualHardDisks;
            OSVirtualHardDisk = osVirtualHardDisk;
            RoleSize = roleSize;
            Deployment = deployment;
            Subscription = subscription;
        }

        public string RoleName { get; protected set; }
        public IList<DataVirtualHardDisk> DataVirtualHardDisks { get; protected set; }
        public OsVirtualHardDisk OSVirtualHardDisk { get; set; }
        public string RoleSize { get; set; }
        public Deployment Deployment { get; protected set; }
        public SubscriptionInfo Subscription { get; protected set; }
    }
}
