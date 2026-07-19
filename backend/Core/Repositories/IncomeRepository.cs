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
            parameters.Add(
                "AccountId",
                string.IsNullOrEmpty(entity.AccountId) ? (Guid?)null : Guid.Parse(entity.AccountId),
                DbType.Guid);

            var createdIncome = await this.connection.QuerySingleOrDefaultAsync<IncomeDto>(
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

            await this.connection.ExecuteAsync(
                SpNames.DeleteIncome,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Income>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                $"SELECT * FROM {VwNames.GetAllIncomes} ORDER BY {nameof(Income.ActionedAt)} DESC;");

            var dtos = await this.connection.QueryAsync<IncomeDto>(command);
            return dtos.Select(x => x.ToModel());
        }

        public async Task<Income> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = $"SELECT * FROM [dbo].[{VwNames.GetAllIncomes}] WHERE [Id] = @Id;";
            var dto = await this.connection.QueryFirstAsync<IncomeDto>(query, new { Id = Guid.Parse(id) });

            return dto.ToModel();
        }

        public async Task<Income> UpdateAsync(Income entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(entity.Id), DbType.Guid);
            parameters.Add("Name", entity.Name);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Description", entity.Description);
            parameters.Add(
                "AccountId",
                string.IsNullOrEmpty(entity.AccountId) ? (Guid?)null : Guid.Parse(entity.AccountId),
                DbType.Guid);

            var updatedIncome = await this.connection.QuerySingleOrDefaultAsync<IncomeDto>(
                SpNames.UpdateIncome,
                parameters,
                commandType: CommandType.StoredProcedure);

            return updatedIncome.ToModel();
        }
    }
}
