using Core.Dtos;
using Core.Models;
using Dapper;
using System.Data;

namespace Core.Repositories
{
    public sealed class CategoryRepository : IRepository<Category>
    {
        private readonly IDbConnection connection;

        public CategoryRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<Category> CreateAsync(Category entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);

            var created = await this.connection.QuerySingleOrDefaultAsync<CategoryDto>(
                SpNames.AddCategory,
                parameters,
                commandType: CommandType.StoredProcedure);

            return created.ToModel();
        }

        public async Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(id), DbType.Guid);

            await this.connection.ExecuteAsync(SpNames.DeleteCategory, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition("SELECT * FROM Categories;");
            var dtos = await this.connection.QueryAsync<CategoryDto>(command);
            return dtos.Select(x => x.ToModel());
        }

        public async Task<Category> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = "SELECT * FROM [dbo].[Categories] WHERE [Id] = @Id;";
            var dto = await this.connection.QueryFirstAsync<CategoryDto>(query, new { Id = Guid.Parse(id) });

            return dto.ToModel();
        }

        public async Task<Category> UpdateAsync(Category entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(entity.Id), DbType.Guid);
            parameters.Add("Name", entity.Name);

            var updatedCategory = await this.connection.QuerySingleOrDefaultAsync<CategoryDto>(
                SpNames.UpdateCategory,
                parameters,
                commandType: CommandType.StoredProcedure);

            return updatedCategory.ToModel();
        }
    }
}
