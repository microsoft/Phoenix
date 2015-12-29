using System;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureAdminClientLib
{
    class StorageOps2
    {
        double validityPeriodInMinutes = 120;

        public enum TransferUnitEnum {Page,Block};

        public Response CloudCloudTransfer(
            BlobTransferInfo btiSource, BlobTransferInfo btiDestination, TransferUnitEnum transferUnit )
        {
            var Resp = new Response();

            try
            {
                var containerSource = MakeContainer(btiSource);
                var containerDestination = MakeContainer(btiDestination);

                //***************************

                var toDateTime = DateTime.UtcNow.AddMinutes(validityPeriodInMinutes);

                var policy = new SharedAccessBlobPolicy
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessStartTime = null,
                    SharedAccessExpiryTime = new DateTimeOffset(toDateTime)
                };

                //***************************

                switch( transferUnit )
                {
                    case TransferUnitEnum.Block:
                        var blobSourceBlock = containerSource.GetBlockBlobReference(btiSource.Blob);
                        var sasBlock = blobSourceBlock.GetSharedAccessSignature(policy);

                        var blobDestinationBlock = containerDestination.GetBlockBlobReference(btiDestination.Blob);
                        Resp.TaskId = blobDestinationBlock.StartCopyFromBlob(new Uri(blobSourceBlock.Uri.AbsoluteUri + sasBlock));
                        break;

                    case TransferUnitEnum.Page:
                        var blobSourcePage = containerSource.GetPageBlobReference(btiSource.Blob);
                        var sasPage = blobSourcePage.GetSharedAccessSignature(policy);

                        var blobDestinationPage = containerDestination.GetPageBlobReference(btiDestination.Blob);
                        Resp.TaskId = blobDestinationPage.StartCopyFromBlob(new Uri(blobSourcePage.Uri.AbsoluteUri + sasPage));
                        break;
                }

                Resp.ResultStatus = Response.ResultStatusEnum.OK;
            }
            catch (Exception ex)
            {
                Resp.ResultStatus = Response.ResultStatusEnum.EXCEPTION;
                Resp.ExceptionMessage = ex.Message;
            }

            return Resp;
        }

        private static CloudBlobContainer MakeContainer(BlobTransferInfo config)
        {
            var creds = new
               Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(config.Account, config.Key);

            var blobStorage = new CloudBlobClient(new Uri(config.AccountUrl), creds);
            var container = blobStorage.GetContainerReference(config.Container);

            return container;
        }

        private static void FetchTaskProgress(CloudBlobContainer container,
                                      string blobName)
        {
            CloudBlockBlob blob;
            blob = (CloudBlockBlob)container.GetBlobReferenceFromServer(blobName);
        }
    }
}
