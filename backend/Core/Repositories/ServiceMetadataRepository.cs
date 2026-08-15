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
        private readonly IDbConnectionFactory connectionFactory;

        public ServiceMetadataRepository(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<ServiceMetadata?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = "SELECT [Id], [ServiceName], [Description], [CreatedAt], [UpdatedAt] FROM [dbo].[ServiceMetadata] WHERE [ServiceName] = @Name;";

            using var connection = this.connectionFactory.CreateConnection();
            var dto = await connection.QueryFirstOrDefaultAsync<ServiceMetadataDto>(query, new { Name = name });

            return dto?.ToModel();
        }

        public async Task<ServiceMetadata> UpsertAsync(ServiceMetadata serviceMetadata, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", serviceMetadata.ServiceName);
            parameters.Add("Description", serviceMetadata.Description);

            using var connection = this.connectionFactory.CreateConnection();
            var metadata = await connection.QuerySingleAsync<ServiceMetadataDto>(
                SpNames.UpsertServiceMetadata,
                parameters,
                commandType: CommandType.StoredProcedure);

            return metadata.ToModel();
        }
    }
}
