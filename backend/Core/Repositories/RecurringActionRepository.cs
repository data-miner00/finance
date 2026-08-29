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
        private readonly IDbConnectionFactory connectionFactory;

        public RecurringActionRepository(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<RecurringAction> CreateAsync(RecurringAction entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Currency", entity.Currency);
            parameters.Add("Type", entity.Type.ToString());
            parameters.Add("Description", entity.Description);
            parameters.Add("RecurringAt", entity.RecurringAt);
            parameters.Add("StartAt", entity.StartAt);
            parameters.Add("RecurrenceType", entity.RecurrenceType.ToString());
            parameters.Add("IntervalValue", entity.IntervalValue);
            parameters.Add("DayOfMonth", entity.DayOfMonth);

            using var connection = this.connectionFactory.CreateConnection();
            var createdRecurringAction = await connection.QuerySingleOrDefaultAsync<RecurringActionDto>(
                SpNames.AddRecurring,
                parameters,
                commandType: CommandType.StoredProcedure);

            return createdRecurringAction.ToModel();
        }

        public async Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(id), DbType.Guid);

            using var connection = this.connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                SpNames.DeleteRecurringAction,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<RecurringAction>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT [Id], [Name], [Description], [IsActive], [Type], [Amount], [Currency], [RecurringAt], [StartAt], [RecurrenceType], [IntervalValue], [DayOfMonth], [LastExecutedAt], [CreatedAt], [UpdatedAt] " +
                "FROM Recurrings;");

            using var connection = this.connectionFactory.CreateConnection();
            var dtos = await connection.QueryAsync<RecurringActionDto>(command);
            return dtos.Select(d => d.ToModel());
        }

        public async Task<RecurringAction?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!Guid.TryParse(id, out var guid))
            {
                return null;
            }

            var query = "SELECT [Id], [Name], [Description], [IsActive], [Type], [Amount], [Currency], [RecurringAt], [StartAt], [RecurrenceType], [IntervalValue], [DayOfMonth], [LastExecutedAt], [CreatedAt], [UpdatedAt] " +
                "FROM [dbo].[Recurrings] WHERE [Id] = @Id;";

            using var connection = this.connectionFactory.CreateConnection();
            var dto = await connection.QueryFirstOrDefaultAsync<RecurringActionDto>(query, new { Id = guid });

            return dto?.ToModel();
        }

        public async Task<RecurringAction> UpdateAsync(RecurringAction entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(entity.Id), DbType.Guid);
            parameters.Add("Name", entity.Name);
            parameters.Add("Description", entity.Description);
            parameters.Add("IsActive", entity.IsActive);
            parameters.Add("RecurringAt", entity.RecurringAt);
            parameters.Add("Type", entity.Type.ToString());
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Currency", entity.Currency);
            parameters.Add("StartAt", entity.StartAt);
            parameters.Add("RecurrenceType", entity.RecurrenceType.ToString());
            parameters.Add("IntervalValue", entity.IntervalValue);
            parameters.Add("DayOfMonth", entity.DayOfMonth);

            using var connection = this.connectionFactory.CreateConnection();
            var updatedRecurringAction = await connection.QuerySingleOrDefaultAsync<RecurringActionDto>(
                SpNames.UpdateRecurringAction,
                parameters,
                commandType: CommandType.StoredProcedure);

            return updatedRecurringAction.ToModel();
        }
    }
}
