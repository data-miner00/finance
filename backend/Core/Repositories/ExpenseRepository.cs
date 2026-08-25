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
        private readonly IDbConnectionFactory connectionFactory;

        public ExpenseRepository(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<Expense> CreateAsync(Expense entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Currency", entity.Currency);
            parameters.Add("CategoryName", entity.CategoryName);
            parameters.Add("Location", entity.Location);
            parameters.Add("Description", entity.Description);
            parameters.Add("AgentName", entity.AgentName);
            parameters.Add(
                "AccountId",
                string.IsNullOrEmpty(entity.AccountId) ? (Guid?)null : Guid.Parse(entity.AccountId),
                DbType.Guid);
            parameters.Add("ReceiptImage", entity.ReceiptImage);

            using var connection = this.connectionFactory.CreateConnection();
            var createdExpense = await connection.QuerySingleOrDefaultAsync<ExpenseDto>(
                SpNames.AddExpenseWithCategoryName,
                parameters,
                commandType: CommandType.StoredProcedure);

            return createdExpense.ToModel();
        }

        public async Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(id), DbType.Guid);

            using var connection = this.connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                SpNames.DeleteExpense,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Expense>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            /*
             * Actually, the view itself already have the order by clause, it works in SQL Management Studio,
             * but ordering is ignored when calling from C# due to query optimization.
             * Hence, the fix is to use Linq in here OR put the ORDER BY clause to the final query below.
             */
            var command = new CommandDefinition(
                $"SELECT [Id], [Name], [Description], [Amount], [Currency], [Location], [ReceiptImage], [ActionedAt], [CreatedAt], [UpdatedAt], [CategoryName], [AgentName], [AccountId], [AccountName] " +
                $"FROM {VwNames.GetAllExpenses} ORDER BY {nameof(Expense.ActionedAt)} DESC;");

            using var connection = this.connectionFactory.CreateConnection();
            var dtos = await connection.QueryAsync<ExpenseDto>(command);
            return dtos.Select(x => x.ToModel());
        }

        public async Task<Expense?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!Guid.TryParse(id, out var guid))
            {
                return null;
            }

            var query = $"SELECT [Id], [Name], [Description], [Amount], [Currency], [Location], [ReceiptImage], [ActionedAt], [CreatedAt], [UpdatedAt], [CategoryName], [AgentName], [AccountId], [AccountName] " +
                $"FROM [dbo].[{VwNames.GetAllExpenses}] WHERE [Id] = @Id;";

            using var connection = this.connectionFactory.CreateConnection();
            var dto = await connection.QueryFirstOrDefaultAsync<ExpenseDto>(query, new { Id = guid });

            return dto?.ToModel();
        }

        public async Task<Expense> UpdateAsync(Expense entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(entity.Id), DbType.Guid);
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Currency", entity.Currency);
            parameters.Add("CategoryName", entity.CategoryName);
            parameters.Add("Location", entity.Location);
            parameters.Add("Description", entity.Description);
            parameters.Add("ActionedAt", entity.ActionedAt);
            parameters.Add("AgentName", entity.AgentName);
            parameters.Add(
                "AccountId",
                string.IsNullOrEmpty(entity.AccountId) ? (Guid?)null : Guid.Parse(entity.AccountId),
                DbType.Guid);
            parameters.Add("ReceiptImage", entity.ReceiptImage);

            using var connection = this.connectionFactory.CreateConnection();
            var updatedExpense = await connection.QuerySingleOrDefaultAsync<ExpenseDto>(
                SpNames.UpdateExpenseWithCategoryName,
                parameters,
                commandType: CommandType.StoredProcedure);

            return updatedExpense.ToModel();
        }

        public async Task ImportAsync(IEnumerable<ExpenseImportModel> expenseImports, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var connection = this.connectionFactory.CreateConnection();

            // TODO: Instead of loop, make bulk imports
            foreach (var model in expenseImports)
            {
                var parameters = new DynamicParameters();
                parameters.Add("Id", model.Id);
                parameters.Add("Name", model.Name);
                parameters.Add("Amount", model.Amount);
                parameters.Add("Currency", model.Currency ?? "MYR");
                parameters.Add("Description", model.Description);
                parameters.Add("ActionedAt", model.ActionedAt);
                parameters.Add("CreatedAt", model.CreatedAt);
                parameters.Add("UpdatedAt", model.UpdatedAt);
                parameters.Add("CategoryName", model.CategoryName);
                parameters.Add("Location", model.Location);
                parameters.Add("AgentName", model.AgentName);

                await connection.ExecuteAsync(
                    SpNames.AddExpenseFullParams,
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
