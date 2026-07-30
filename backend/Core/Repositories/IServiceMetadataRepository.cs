using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Repositories
{
    public interface IServiceMetadataRepository
    {
        Task<ServiceMetadata> UpsertAsync(ServiceMetadata serviceMetadata, CancellationToken cancellationToken);

        Task<ServiceMetadata?> GetByNameAsync(string name, CancellationToken cancellationToken);
    }
}
