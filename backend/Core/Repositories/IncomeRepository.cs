using Core.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Core.Repositories
{
    public sealed class IncomeRepository : IRepository<Income>
    {
        private readonly IDbConnection connection;

        public IncomeRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<Income> CreateAsync(Income entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Description", entity.Description);

            var createdIncome = await this.connection.QuerySingleOrDefaultAsync<Income>(
                SpNames.AddIncome,
                parameters,
                commandType: CommandType.StoredProcedure);

            return createdIncome;
        }

        public Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Income>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT * FROM Incomes;");

            return this.connection.QueryAsync<Income>(command);
        }

        public Task<Income> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Income entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
