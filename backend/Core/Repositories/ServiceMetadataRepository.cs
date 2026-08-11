using Core.Dtos;
using Core.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Core.Repositories
{
    public class ServiceMetadataRepository : IServiceMetadataRepository
    {
        private readonly IDbConnection connection;

        public ServiceMetadataRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<ServiceMetadata?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = $"SELECT * FROM [dbo].[ServiceMetadata] WHERE [ServiceName] = @Name;";
            var dto = await this.connection.QueryFirstOrDefaultAsync<ServiceMetadataDto>(query, new { Name = name });

            return dto?.ToModel();
        }

        public async Task<ServiceMetadata> UpsertAsync(ServiceMetadata serviceMetadata, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", serviceMetadata.ServiceName);
            parameters.Add("Description", serviceMetadata.Description);

            var metadata = await this.connection.QuerySingleAsync<ServiceMetadataDto>(
                SpNames.UpsertServiceMetadata,
                parameters,
                commandType: CommandType.StoredProcedure);

            return metadata.ToModel();
        }
    }
}
