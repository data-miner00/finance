using Core.Dtos;
using Core.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Core.Repositories
{
    public class PersonRepository : IRepository<Person>
    {
        private readonly IDbConnection connection;

        public PersonRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<Person> CreateAsync(Person entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);
            parameters.Add("Alias", entity.Alias);
            parameters.Add("Description", entity.Description);

            var createdPerson = await this.connection.QuerySingleOrDefaultAsync<PersonDto>(
                SpNames.AddPerson,
                parameters,
                commandType: CommandType.StoredProcedure);

            return createdPerson.ToModel();
        }

        public async Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(id), DbType.Guid);

            await this.connection.ExecuteAsync(
                SpNames.DeletePerson,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Person>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT * FROM People;");

            var dtos = await this.connection.QueryAsync<PersonDto>(command);
            return dtos.Select(x => x.ToModel());
        }

        public async Task<Person> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = "SELECT * FROM [dbo].[Person] WHERE [Id] = @Id;";
            var dto = await this.connection.QueryFirstAsync<PersonDto>(query, new { Id = Guid.Parse(id) });

            return dto.ToModel();
        }

        public async Task<Person> UpdateAsync(Person entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(entity.Id), DbType.Guid);
            parameters.Add("Name", entity.Name);
            parameters.Add("Alias", entity.Alias);
            parameters.Add("Description", entity.Description);

            var updatedPerson = await this.connection.QuerySingleOrDefaultAsync<PersonDto>(
                SpNames.UpdatePiggyBank,
                parameters,
                commandType: CommandType.StoredProcedure);

            return updatedPerson.ToModel();
        }
    }
}
