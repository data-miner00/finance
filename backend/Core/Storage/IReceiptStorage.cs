using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Storage
{
    public interface IReceiptStorage
    {
        Task<string> UploadAsync(string expenseId, string fileName, Stream content, string contentType, CancellationToken cancellationToken);

        Task<(Stream Content, string ContentType)> OpenReadAsync(string blobName, CancellationToken cancellationToken);

        Task DeleteAsync(string blobName, CancellationToken cancellationToken);
    }
}
