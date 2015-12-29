using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;

namespace AzureAdminClientLib
{
    class BlobHelper
    {
        public CloudStorageAccount Account;
        public CloudBlobClient BlobClient;

        // Constructor - pass in a storage connection string.
        public BlobHelper(string connectionString)
        {
            Account = CloudStorageAccount.Parse(connectionString);

            BlobClient = Account.CreateCloudBlobClient();
            BlobClient.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.Zero);
        }

        public BlobHelper(string storageName, string privateKey)
        {
            var connectionString = String.Format(
                "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", 
                storageName, privateKey);
            Account = CloudStorageAccount.Parse(connectionString);
            BlobClient = Account.CreateCloudBlobClient();

            BlobClient.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.Zero);
        }

        public bool SnapshotBlob(string containerName, string blobName)
        {
            try
            {
                var container = BlobClient.GetContainerReference(containerName);
                var blob = container.GetBlobReference(blobName);
                blob.CreateSnapshot();
                return true;
            }
            catch (StorageClientException ex)
            {
                if ((int)ex.StatusCode == 404)
                {
                    return false;
                }

                throw;
            }
        }
    }
}
