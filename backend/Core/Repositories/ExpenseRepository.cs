using Core.Models;
using Core.Dtos;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Core.Repositories
{
    public sealed class ExpenseRepository : IRepository<Expense>
    {
        private readonly IDbConnection connection;

        public ExpenseRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<Expense> CreateAsync(Expense entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("CategoryId", entity.CategoryId is null ? null : Guid.Parse(entity.CategoryId));
            parameters.Add("Location", entity.Location);
            parameters.Add("Description", entity.Description);

            var createdExpense = await this.connection.QuerySingleOrDefaultAsync<ExpenseDto>(
                SpNames.AddExpense,
                parameters,
                commandType: CommandType.StoredProcedure);

            return createdExpense.ToModel();
        }

        public Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Expense>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT * FROM Expenses;");

            var dtos = await this.connection.QueryAsync<ExpenseDto>(command);
            return dtos.Select(x => x.ToModel());
        }

        public async Task<Expense> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = "SELECT * FROM [dbo].[Expenses] WHERE [Id] = @Id;";
            var dto = await this.connection.QueryFirstAsync<ExpenseDto>(query, new { Id = id });

            return dto.ToModel();
        }

        public Task UpdateAsync(Expense entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
