namespace AzureAdminClientLib
{
    public class AzureVirtualMachine
    {
        public AzureVirtualMachine(string roleName, string instanceName, string hostName, string instanceStatus, string deploymentSlot, string virtualNetworkName)
        {
            RoleName = roleName;
            InstanceName = instanceName;
            HostName = hostName;
            InstanceStatus = instanceStatus;
            DeploymentSlot = deploymentSlot;
            VirtualNetworkName = virtualNetworkName;
        }

        public string RoleName { get; protected set; }

        public string InstanceName { get; protected set; }

        public string HostName { get; protected set; }

        public string InstanceStatus { get; protected set; } // Should probably be an enumeration. TODO: See if we can get this from Azure.

        public string DeploymentSlot { get; protected set; } // Should probably be an enumeration. TODO: See if we can get this from Azure.

        public string VirtualNetworkName { get; protected set; }

    }
}
