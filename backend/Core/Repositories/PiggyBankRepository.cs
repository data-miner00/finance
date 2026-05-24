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
        private readonly IDbConnection connection;

        public PiggyBankRepository(IDbConnection connection)
        {
            this.connection = connection;
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

            var createdPiggyBank = await this.connection.QuerySingleOrDefaultAsync<PiggyBankDto>(
                SpNames.AddPiggyBank,
                parameters,
                commandType: CommandType.StoredProcedure);

            return createdPiggyBank.ToModel();
        }

        public Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PiggyBank>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT * FROM PiggyBanks;");

            var dtos = await this.connection.QueryAsync<PiggyBankDto>(command);
            return dtos.Select(x => x.ToModel());
        }

        public async Task<PiggyBank> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = "SELECT * FROM [dbo].[PiggyBanks] WHERE [Id] = @Id;";
            var dto = await this.connection.QueryFirstAsync<PiggyBankDto>(query, new { Id = Guid.Parse(id) });

            return dto.ToModel();
        }

        public Task UpdateAsync(PiggyBank entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
