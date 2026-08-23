using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApi.Models;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CategoryController : ControllerBase
    {
        private const int SqlForeignKeyViolationErrorNumber = 547;
        private const int SqlCheckConstraintViolationErrorNumber = 547;
        private const int SqlUniqueConstraintViolationErrorNumber = 2627;

        private readonly CategoryRepository repository;

        public CategoryController(CategoryRepository repository)
        {
            this.repository = repository;
        }

        private CancellationToken CancellationToken => this.HttpContext.RequestAborted;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var categories = await this.repository.GetAllAsync(this.CancellationToken);

            return this.Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetById(string id)
        {
            try
            {
                var category = await this.repository.GetByIdAsync(id, this.CancellationToken);
                return this.Ok(category);
            }
            catch
            {
                return this.NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Create(CreateCategoryRequest request)
        {
            var name = request.Name?.Trim() ?? string.Empty;
            if (name.Length == 0)
            {
                return this.BadRequest("Category name is required.");
            }

            var existingCategories = await this.repository.GetAllAsync(this.CancellationToken);
            if (existingCategories.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                return this.Conflict($"A category named \"{name}\" already exists.");
            }

            var category = new Category
            {
                Name = name,
                Color = request.Color,
                Icon = request.Icon,
                BudgetAmount = request.BudgetAmount,
            };

            try
            {
                var created = await this.repository.CreateAsync(category, this.CancellationToken);
                return this.CreatedAtAction(nameof(this.GetById), new { id = created.Id }, created);
            }
            catch (SqlException ex) when (ex.Number == SqlUniqueConstraintViolationErrorNumber)
            {
                return this.Conflict($"A category named \"{name}\" already exists.");
            }
            catch (SqlException ex) when (ex.Number == SqlCheckConstraintViolationErrorNumber)
            {
                return this.BadRequest("Category name is required.");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> Update(string id, UpdateCategoryRequest request)
        {
            Category category;
            try
            {
                category = await this.repository.GetByIdAsync(id, this.CancellationToken);
            }
            catch
            {
                return this.NotFound();
            }

            var name = request.Name?.Trim() ?? string.Empty;
            if (name.Length == 0)
            {
                return this.BadRequest("Category name is required.");
            }

            var existingCategories = await this.repository.GetAllAsync(this.CancellationToken);
            if (existingCategories.Any(c => c.Id != id && string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                return this.Conflict($"A category named \"{name}\" already exists.");
            }

            category.Name = name;
            category.Color = request.Color;
            category.Icon = request.Icon;
            category.BudgetAmount = request.BudgetAmount;

            try
            {
                var updated = await this.repository.UpdateAsync(category, this.CancellationToken);
                return this.Ok(updated);
            }
            catch (SqlException ex) when (ex.Number == SqlUniqueConstraintViolationErrorNumber)
            {
                return this.Conflict($"A category named \"{name}\" already exists.");
            }
            catch (SqlException ex) when (ex.Number == SqlCheckConstraintViolationErrorNumber)
            {
                return this.BadRequest("Category name is required.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                await this.repository.GetByIdAsync(id, this.CancellationToken);
            }
            catch
            {
                return this.NotFound();
            }

            try
            {
                await this.repository.DeleteByIdAsync(id, this.CancellationToken);
                return this.NoContent();
            }
            catch (SqlException ex) when (ex.Number == SqlForeignKeyViolationErrorNumber)
            {
                return this.Conflict("Category is still in use by one or more expenses. Merge it into another category first.");
            }
        }

        [HttpPost("merge")]
        public async Task<ActionResult> Merge(MergeCategoriesRequest request)
        {
            try
            {
                await this.repository.GetByIdAsync(request.SourceCategoryId, this.CancellationToken);
                await this.repository.GetByIdAsync(request.TargetCategoryId, this.CancellationToken);
            }
            catch
            {
                return this.NotFound();
            }

            await this.repository.MergeAsync(request.SourceCategoryId, request.TargetCategoryId, this.CancellationToken);
            return this.NoContent();
        }
    }
}
