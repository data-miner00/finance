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
        private readonly IDbConnection connection;

        public TaxRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<Tax> CreateAsync(Tax entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Description", entity.Description);

            var createdTax = await this.connection.QuerySingleOrDefaultAsync<TaxDto>(
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

            await this.connection.ExecuteAsync(
                SpNames.DeleteTax,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Tax>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT * FROM Taxes;");

            var dtos = await this.connection.QueryAsync<TaxDto>(command);
            return dtos.Select(x => x.ToModel());
        }

        public async Task<Tax> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = "SELECT * FROM [dbo].[Taxes] WHERE [Id] = @Id;";
            var dto = await this.connection.QueryFirstAsync<TaxDto>(query, new { Id = Guid.Parse(id) });

            return dto.ToModel();
        }

        public async Task<Tax> UpdateAsync(Tax entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(entity.Id), DbType.Guid);
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Description", entity.Description);

            var updatedTax = await this.connection.QuerySingleOrDefaultAsync<TaxDto>(
                SpNames.UpdateTax,
                parameters,
                commandType: CommandType.StoredProcedure);

            return updatedTax.ToModel();
        }
    }
}
