using Core.Dtos;
using Core.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Core.Repositories
{
    public sealed class AccountRepository : IRepository<Account>
    {
        private readonly IDbConnectionFactory connectionFactory;

        public AccountRepository(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<Account> CreateAsync(Account entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);
            parameters.Add("Type", entity.Type.ToString());
            parameters.Add("Description", entity.Description);
            parameters.Add("Balance", entity.Balance);

            using var connection = this.connectionFactory.CreateConnection();
            var createdAccount = await connection.QuerySingleOrDefaultAsync<AccountDto>(
                SpNames.AddAccount,
                parameters,
                commandType: CommandType.StoredProcedure);

            return createdAccount.ToModel();
        }

        public async Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(id), DbType.Guid);

            using var connection = this.connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                SpNames.DeleteAccount,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT [Id], [Name], [Description], [Type], [Balance], [CreatedAt], [UpdatedAt] FROM Accounts;");

            using var connection = this.connectionFactory.CreateConnection();
            var accounts = await connection.QueryAsync<AccountDto>(command);

            return accounts.Select(x => x.ToModel());
        }

        public async Task<Account> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = "SELECT [Id], [Name], [Description], [Type], [Balance], [CreatedAt], [UpdatedAt] FROM [dbo].[Accounts] WHERE [Id] = @Id;";

            using var connection = this.connectionFactory.CreateConnection();
            var accountDto = await connection.QueryFirstAsync<AccountDto>(query, new { Id = Guid.Parse(id) });

            return accountDto.ToModel();
        }

        public async Task<Account> UpdateAsync(Account entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(entity.Id), DbType.Guid);
            parameters.Add("Name", entity.Name);
            parameters.Add("Type", entity.Type.ToString());
            parameters.Add("Balance", entity.Balance);
            parameters.Add("Description", entity.Description);

            using var connection = this.connectionFactory.CreateConnection();
            var updatedAccount = await connection.QuerySingleOrDefaultAsync<AccountDto>(
                SpNames.UpdateAccount,
                parameters,
                commandType: CommandType.StoredProcedure);

            return updatedAccount.ToModel();
        }
    }
}
