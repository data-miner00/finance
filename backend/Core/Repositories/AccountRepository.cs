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

        public Task CreateAsync(Account entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);
            parameters.Add("Type", entity.AccountType.ToString());
            parameters.Add("Description", entity.Description);

            return this.connection.ExecuteAsync(
                SpNames.AddAccount,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT * FROM Accounts;");

            return this.connection.QueryAsync<Account>(command);
        }

        public Task<Account> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Account entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
