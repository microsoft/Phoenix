using System.Collections.Generic;

namespace AzureAdminClientLib
{
    public class AzureStorageAccount
    {

        public string Name {get; protected set;}
        
        public string AffinityGroup {get; protected set;}

        public string Location {get; protected set;}

        public string PrimaryAccessKey { get; protected set; }

        public string SecondaryAccessKey { get; protected set; }

        public List<AzureStorageAccountContainer> Containers { get; set; }

        public List<AzureVirtualNetwork> VirtualNetworksAvailable { get; set; }

        public AzureStorageAccount(string name, string affinityGroup, string location, string primaryAccessKey, string secondaryAccessKey, List<AzureStorageAccountContainer> containers)
        {
            Name = name;
            AffinityGroup = affinityGroup;
            Location = location;
            PrimaryAccessKey = primaryAccessKey;
            SecondaryAccessKey = secondaryAccessKey;
            Containers = containers;
        }
    }
}
