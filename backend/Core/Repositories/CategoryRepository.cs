using Core.Dtos;
using Core.Models;
using Dapper;
using System.Data;

namespace Core.Repositories
{
    public sealed class CategoryRepository : IRepository<Category>
    {
        private readonly IDbConnectionFactory connectionFactory;

        public CategoryRepository(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<Category> CreateAsync(Category entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Name", entity.Name);
            parameters.Add("Color", entity.Color);
            parameters.Add("Icon", entity.Icon);
            parameters.Add("BudgetAmount", entity.BudgetAmount);

            using var connection = this.connectionFactory.CreateConnection();
            var created = await connection.QuerySingleOrDefaultAsync<CategoryDto>(
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

            using var connection = this.connectionFactory.CreateConnection();
            await connection.ExecuteAsync(SpNames.DeleteCategory, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT [Id], [Name], [Color], [Icon], [BudgetAmount], [CreatedAt], [UpdatedAt] FROM Categories;");

            using var connection = this.connectionFactory.CreateConnection();
            var dtos = await connection.QueryAsync<CategoryDto>(command);
            return dtos.Select(x => x.ToModel());
        }

        public async Task<Category> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = "SELECT [Id], [Name], [Color], [Icon], [BudgetAmount], [CreatedAt], [UpdatedAt] FROM [dbo].[Categories] WHERE [Id] = @Id;";

            using var connection = this.connectionFactory.CreateConnection();
            var dto = await connection.QueryFirstAsync<CategoryDto>(query, new { Id = Guid.Parse(id) });

            return dto.ToModel();
        }

        public async Task<Category> UpdateAsync(Category entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(entity.Id), DbType.Guid);
            parameters.Add("Name", entity.Name);
            parameters.Add("Color", entity.Color);
            parameters.Add("Icon", entity.Icon);
            parameters.Add("BudgetAmount", entity.BudgetAmount);

            using var connection = this.connectionFactory.CreateConnection();
            var updatedCategory = await connection.QuerySingleOrDefaultAsync<CategoryDto>(
                SpNames.UpdateCategory,
                parameters,
                commandType: CommandType.StoredProcedure);

            return updatedCategory.ToModel();
        }

        public async Task MergeAsync(string sourceCategoryId, string targetCategoryId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("SourceCategoryId", Guid.Parse(sourceCategoryId), DbType.Guid);
            parameters.Add("TargetCategoryId", Guid.Parse(targetCategoryId), DbType.Guid);

            using var connection = this.connectionFactory.CreateConnection();
            await connection.ExecuteAsync(SpNames.MergeCategories, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<CategoryMonthlySpend?> GetMonthlySpendAsync(string categoryName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = $"SELECT [CategoryId], [CategoryName], [BudgetAmount], [SpentThisMonth] FROM [dbo].[{VwNames.GetCategoryMonthlySpend}] WHERE [CategoryName] = @CategoryName;";

            using var connection = this.connectionFactory.CreateConnection();
            var dto = await connection.QueryFirstOrDefaultAsync<CategoryMonthlySpendDto>(query, new { CategoryName = categoryName });
            return dto?.ToModel();
        }
    }
}
