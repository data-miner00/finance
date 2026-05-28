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
        private readonly IDbConnection connection;

        public AccountRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<Account> CreateAsync(Account entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);
            parameters.Add("Type", entity.Type.ToString());
            parameters.Add("Description", entity.Description);
            parameters.Add("Balance", entity.Balance);

            var createdAccount = await this.connection.QuerySingleOrDefaultAsync<AccountDto>(
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

            await this.connection.ExecuteAsync(
                SpNames.DeleteAccount,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT * FROM Accounts;");

            var accounts = await this.connection.QueryAsync<AccountDto>(command);

            return accounts.Select(x => x.ToModel());
        }

        public async Task<Account> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = "SELECT * FROM [dbo].[Accounts] WHERE [Id] = @Id;";
            var accountDto = await this.connection.QueryFirstAsync<AccountDto>(query, new { Id = Guid.Parse(id) });

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

            var updatedAccount = await this.connection.QuerySingleOrDefaultAsync<AccountDto>(
                SpNames.UpdateAccount,
                parameters,
                commandType: CommandType.StoredProcedure);

            return updatedAccount.ToModel();
        }
    }
}
