using Core.Models;
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
            parameters.Add("CategoryId", entity.CategoryId);
            parameters.Add("Location", entity.Location);
            parameters.Add("Description", entity.Description);

            var createdExpense = await this.connection.QuerySingleOrDefaultAsync<Expense>(
                SpNames.AddExpense,
                parameters,
                commandType: CommandType.StoredProcedure);

            return createdExpense;
        }

        public Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Expense>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT * FROM Expenses;");

            return this.connection.QueryAsync<Expense>(command);
        }

        public Task<Expense> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = "SELECT * FROM [dbo].[Expenses] WHERE [Id] = @Id;";

            return this.connection.QueryFirstAsync<Expense>(query, new { Id = id });
        }

        public Task UpdateAsync(Expense entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
