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

        public Task CreateAsync(PiggyBank entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Target", entity.Target);
            parameters.Add("Description", entity.Description);
            parameters.Add("Deadline", entity.Deadline);

            return this.connection.ExecuteAsync(
                SpNames.AddPiggyBank,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PiggyBank>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT * FROM PiggyBanks;");

            return this.connection.QueryAsync<PiggyBank>(command);
        }

        public Task<PiggyBank> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(PiggyBank entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
