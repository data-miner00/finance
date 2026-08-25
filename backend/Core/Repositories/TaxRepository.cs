using Core.Dtos;
using Core.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Core.Repositories
{
    public sealed class TaxRepository : IRepository<Tax>
    {
        private readonly IDbConnectionFactory connectionFactory;

        public TaxRepository(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<Tax> CreateAsync(Tax entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Description", entity.Description);

            using var connection = this.connectionFactory.CreateConnection();
            var createdTax = await connection.QuerySingleOrDefaultAsync<TaxDto>(
                SpNames.AddTax,
                parameters,
                commandType: CommandType.StoredProcedure);

            return createdTax.ToModel();
        }

        public async Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(id), DbType.Guid);

            using var connection = this.connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                SpNames.DeleteTax,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Tax>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT [Id], [Name], [Description], [Amount], [CreatedAt], [UpdatedAt] FROM Taxes;");

            using var connection = this.connectionFactory.CreateConnection();
            var dtos = await connection.QueryAsync<TaxDto>(command);
            return dtos.Select(x => x.ToModel());
        }

        public async Task<Tax?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!Guid.TryParse(id, out var guid))
            {
                return null;
            }

            var query = "SELECT [Id], [Name], [Description], [Amount], [CreatedAt], [UpdatedAt] FROM [dbo].[Taxes] WHERE [Id] = @Id;";

            using var connection = this.connectionFactory.CreateConnection();
            var dto = await connection.QueryFirstOrDefaultAsync<TaxDto>(query, new { Id = guid });

            return dto?.ToModel();
        }

        public async Task<Tax> UpdateAsync(Tax entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(entity.Id), DbType.Guid);
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Description", entity.Description);

            using var connection = this.connectionFactory.CreateConnection();
            var updatedTax = await connection.QuerySingleOrDefaultAsync<TaxDto>(
                SpNames.UpdateTax,
                parameters,
                commandType: CommandType.StoredProcedure);

            return updatedTax.ToModel();
        }
    }
}
