using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Storage
{
    public sealed class BlobReceiptStorage : IReceiptStorage
    {
        private readonly BlobContainerClient containerClient;

        public BlobReceiptStorage(BlobServiceClient blobServiceClient)
        {
            this.containerClient = blobServiceClient.GetBlobContainerClient(BlobNames.Receipts);
        }

        public async Task<string> UploadAsync(string expenseId, string fileName, Stream content, string contentType, CancellationToken cancellationToken)
        {
            var blobName = $"{expenseId}/{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var blobClient = this.containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(
                content,
                new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType } },
                cancellationToken);

            return blobName;
        }

        public async Task<(Stream Content, string ContentType)> OpenReadAsync(string blobName, CancellationToken cancellationToken)
        {
            var blobClient = this.containerClient.GetBlobClient(blobName);
            var download = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);

            return (download.Value.Content, download.Value.Details.ContentType);
        }

        public async Task DeleteAsync(string blobName, CancellationToken cancellationToken)
        {
            var blobClient = this.containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }
    }
}
