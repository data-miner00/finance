using Core.Dtos;
using Core.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Core.Repositories
{
    public sealed class PiggyBankRepository : IRepository<PiggyBank>
    {
        private readonly IDbConnectionFactory connectionFactory;

        public PiggyBankRepository(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<PiggyBank> CreateAsync(PiggyBank entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Target", entity.Target);
            parameters.Add("Description", entity.Description);
            parameters.Add("Deadline", entity.Deadline);

            using var connection = this.connectionFactory.CreateConnection();
            var createdPiggyBank = await connection.QuerySingleOrDefaultAsync<PiggyBankDto>(
                SpNames.AddPiggyBank,
                parameters,
                commandType: CommandType.StoredProcedure);

            return createdPiggyBank.ToModel();
        }

        public async Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(id), DbType.Guid);

            using var connection = this.connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                SpNames.DeletePiggyBank,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<PiggyBank>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT [Id], [Name], [Description], [Amount], [Target], [Deadline], [CreatedAt], [UpdatedAt] FROM PiggyBanks;");

            using var connection = this.connectionFactory.CreateConnection();
            var dtos = await connection.QueryAsync<PiggyBankDto>(command);
            return dtos.Select(x => x.ToModel());
        }

        public async Task<PiggyBank?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!Guid.TryParse(id, out var guid))
            {
                return null;
            }

            var query = "SELECT [Id], [Name], [Description], [Amount], [Target], [Deadline], [CreatedAt], [UpdatedAt] FROM [dbo].[PiggyBanks] WHERE [Id] = @Id;";

            using var connection = this.connectionFactory.CreateConnection();
            var dto = await connection.QueryFirstOrDefaultAsync<PiggyBankDto>(query, new { Id = guid });

            return dto?.ToModel();
        }

        public async Task<PiggyBank> UpdateAsync(PiggyBank entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(entity.Id), DbType.Guid);
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Target", entity.Target);
            parameters.Add("Deadline", entity.Deadline);
            parameters.Add("Description", entity.Description);

            using var connection = this.connectionFactory.CreateConnection();
            var updatedPiggyBank = await connection.QuerySingleOrDefaultAsync<PiggyBankDto>(
                SpNames.UpdatePiggyBank,
                parameters,
                commandType: CommandType.StoredProcedure);

            return updatedPiggyBank.ToModel();
        }
    }
}
