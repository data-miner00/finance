using Core.Dtos;
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
        private readonly IDbConnectionFactory connectionFactory;

        public IncomeRepository(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<Income> CreateAsync(Income entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Currency", entity.Currency);
            parameters.Add("Description", entity.Description);
            parameters.Add(
                "AccountId",
                string.IsNullOrEmpty(entity.AccountId) ? (Guid?)null : Guid.Parse(entity.AccountId),
                DbType.Guid);

            using var connection = this.connectionFactory.CreateConnection();
            var createdIncome = await connection.QuerySingleOrDefaultAsync<IncomeDto>(
                SpNames.AddIncome,
                parameters,
                commandType: CommandType.StoredProcedure);

            return createdIncome.ToModel();
        }

        public async Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(id), DbType.Guid);

            using var connection = this.connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                SpNames.DeleteIncome,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Income>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                $"SELECT [Id], [Name], [Description], [Amount], [Currency], [ActionedAt], [CreatedAt], [UpdatedAt], [AccountId], [AccountName] " +
                $"FROM {VwNames.GetAllIncomes} ORDER BY {nameof(Income.ActionedAt)} DESC;");

            using var connection = this.connectionFactory.CreateConnection();
            var dtos = await connection.QueryAsync<IncomeDto>(command);
            return dtos.Select(x => x.ToModel());
        }

        public async Task<Income?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!Guid.TryParse(id, out var guid))
            {
                return null;
            }

            var query = $"SELECT [Id], [Name], [Description], [Amount], [Currency], [ActionedAt], [CreatedAt], [UpdatedAt], [AccountId], [AccountName] " +
                $"FROM [dbo].[{VwNames.GetAllIncomes}] WHERE [Id] = @Id;";

            using var connection = this.connectionFactory.CreateConnection();
            var dto = await connection.QueryFirstOrDefaultAsync<IncomeDto>(query, new { Id = guid });

            return dto?.ToModel();
        }

        public async Task<Income> UpdateAsync(Income entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(entity.Id), DbType.Guid);
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Currency", entity.Currency);
            parameters.Add("Description", entity.Description);
            parameters.Add(
                "AccountId",
                string.IsNullOrEmpty(entity.AccountId) ? (Guid?)null : Guid.Parse(entity.AccountId),
                DbType.Guid);

            using var connection = this.connectionFactory.CreateConnection();
            var updatedIncome = await connection.QuerySingleOrDefaultAsync<IncomeDto>(
                SpNames.UpdateIncome,
                parameters,
                commandType: CommandType.StoredProcedure);

            return updatedIncome.ToModel();
        }
    }
}
