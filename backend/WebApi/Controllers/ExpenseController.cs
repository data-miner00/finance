using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;
using Core.Streams;
using System.Text.Json;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private const string DefaultExportFormat = "json";
        private const string BudgetNearLimitType = "BudgetNearLimit";
        private const string BudgetOverBudgetType = "BudgetOverBudget";
        private const decimal NearLimitRatio = 0.8m;

        private readonly ExpenseRepository _repository;
        private readonly CategoryRepository categoryRepository;
        private readonly INotificationRepository notificationRepository;
        private readonly ILogger<ExpenseController> logger;
        private readonly IDictionary<string, IDataStreamifier> dataStreamifiers;

        public ExpenseController(
            ExpenseRepository repository,
            CategoryRepository categoryRepository,
            INotificationRepository notificationRepository,
            ILogger<ExpenseController> logger,
            IDictionary<string, IDataStreamifier> dataStreamifiers)
        {
            _repository = repository;
            this.categoryRepository = categoryRepository;
            this.notificationRepository = notificationRepository;
            this.logger = logger;
            this.dataStreamifiers = dataStreamifiers;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetAll(CancellationToken cancellationToken)
        {
            var expenses = await _repository.GetAllAsync(cancellationToken);
            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var expense = await _repository.GetByIdAsync(id, cancellationToken);
                return Ok(expense);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Expense>> Create(CreateExpenseRequest request, CancellationToken cancellationToken)
        {
            var expense = new Expense
            {
                CategoryName = request.CategoryName,
                AccountId = request.AccountId,
                Name = request.Name,
                Description = request.Description,
                Amount = request.Amount,
                Currency = request.Currency,
                Location = request.Location,
                ReceiptImage = request.ReceiptImage,
                AgentName = request.AgentName,
            };

            var createdExpense = await _repository.CreateAsync(expense, cancellationToken);
            await this.EvaluateBudgetAlertsAsync(createdExpense.CategoryName, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdExpense.Id }, createdExpense);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Expense>> Update(string id, UpdateExpenseRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var expense = await _repository.GetByIdAsync(id, cancellationToken);
                expense.CategoryName = request.CategoryName;
                expense.AccountId = request.AccountId;
                expense.Name = request.Name;
                expense.Description = request.Description;
                expense.Amount = request.Amount;
                expense.Currency = request.Currency;
                expense.Location = request.Location;
                expense.ReceiptImage = request.ReceiptImage;
                expense.ActionedAt = request.ActionedAt;
                expense.AgentName = request.AgentName;

                var updated = await _repository.UpdateAsync(expense, cancellationToken);
                await this.EvaluateBudgetAlertsAsync(updated.CategoryName, cancellationToken);
                return this.Ok(updated);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            try
            {
                await _repository.GetByIdAsync(id, cancellationToken);
                await _repository.DeleteByIdAsync(id, cancellationToken);
                return NoContent();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet("export")]
        public async Task<ActionResult> ExportAll([FromQuery] string? format, CancellationToken cancellationToken)
        {
            format ??= DefaultExportFormat;
            var streamifier = this.dataStreamifiers[format]!;
            var expenses = await _repository.GetAllAsync(cancellationToken);
            var stream = await streamifier.StreamifyAsync(expenses, cancellationToken);
            
            const string contentType = "application/octet-stream";
            string downloadName = $"downloaded_file.{format}";

            return File(stream, contentType, downloadName);
        }

        [HttpPost("import")]
        public async Task<ActionResult> ImportFrom(IFormFile file,  CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!file.ContentType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
                return BadRequest("File must be JSON.");

            using var stream = file.OpenReadStream();
            var items = await JsonSerializer.DeserializeAsync<List<ExpenseImportModel>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (items == null)
                return BadRequest("Could not parse JSON.");

            await this._repository.ImportAsync(items, cancellationToken);

            return this.NoContent();
        }

        private async Task EvaluateBudgetAlertsAsync(string? categoryName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return;
            }

            try
            {
                var status = await this.categoryRepository.GetMonthlySpendAsync(categoryName, cancellationToken);
                if (status?.BudgetAmount is null || status.BudgetAmount <= 0)
                {
                    return;
                }

                var ratio = status.SpentThisMonth / status.BudgetAmount.Value;
                string? type = ratio > 1.0m ? BudgetOverBudgetType : ratio >= NearLimitRatio ? BudgetNearLimitType : null;
                if (type is null)
                {
                    return;
                }

                if (await this.notificationRepository.ExistsForEntityThisMonthAsync("Category", status.CategoryId, type, cancellationToken))
                {
                    return;
                }

                var isOverBudget = type == BudgetOverBudgetType;
                await this.notificationRepository.CreateAsync(new Notification
                {
                    Type = type,
                    Title = isOverBudget ? "Over budget" : "Near budget limit",
                    Message = isOverBudget
                        ? $"You've spent {status.SpentThisMonth:C} in \"{status.CategoryName}\" this month, over your {status.BudgetAmount:C} budget."
                        : $"You've spent {status.SpentThisMonth:C} of your {status.BudgetAmount:C} budget in \"{status.CategoryName}\" this month.",
                    EntityType = "Category",
                    EntityId = status.CategoryId,
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to evaluate budget alerts for category '{CategoryName}'.", categoryName);
            }
        }
    }
}

