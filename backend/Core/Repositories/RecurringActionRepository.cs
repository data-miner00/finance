using Core.Dtos;
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

            var createdRecurringAction = await this.connection.QuerySingleOrDefaultAsync<RecurringActionDto>(
                SpNames.AddRecurring,
                parameters,
                commandType: CommandType.StoredProcedure);

            return createdRecurringAction.ToModel();
        }

        public Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RecurringAction>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT * FROM Recurrings;");

            var dtos = await this.connection.QueryAsync<RecurringActionDto>(command);
            return dtos.Select(d => d.ToModel());
        }

        public async Task<RecurringAction> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = "SELECT * FROM [dbo].[Recurrings] WHERE [Id] = @Id;";
            var dto = await this.connection.QueryFirstAsync<RecurringActionDto>(query, new { Id = Guid.Parse(id) });

            return dto.ToModel();
        }

        public Task UpdateAsync(RecurringAction entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
