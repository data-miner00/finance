using Core.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Core.Repositories
{
    public sealed class RecurringActionRepository : IRepository<RecurringAction>
    {
        private readonly IDbConnection connection;

        public RecurringActionRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<RecurringAction> CreateAsync(RecurringAction entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Type", entity.Type.ToString());
            parameters.Add("Description", entity.Description);
            parameters.Add("RecurringAt", entity.RecurringAt);

            var createdRecurringAction = await this.connection.QuerySingleOrDefaultAsync<RecurringAction>(
                SpNames.AddRecurring,
                parameters,
                commandType: CommandType.StoredProcedure);

            return createdRecurringAction;
        }

        public Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RecurringAction>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT * FROM Recurrings;");

            return this.connection.QueryAsync<RecurringAction>(command);
        }

        public Task<RecurringAction> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(RecurringAction entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
