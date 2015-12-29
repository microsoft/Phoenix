using System;

namespace AzureAdminClientLib
{
    public class AzureStorageAccountContainer
    {
        public string Name { get; protected set; }

        public string VnetName { get; set; }

        public string Url { get; protected set; }

        public int ObjectCount { get; set; }

        public AzureStorageAccount StorageAccount { get; set; }

        public bool Resolved = false;

        public Exception Exception = null;

        public AzureStorageAccountContainer(string name, string url, int objectCount, AzureStorageAccount storageAccount)
        {
            Name = name;
            Url = url;
            ObjectCount = objectCount;
            StorageAccount = storageAccount;
        }
    }
}
