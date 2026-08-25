using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using Core.Storage;
using WebApi.Models;
using Core.Streams;
using System.Text.Json;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private const string DefaultExportFormat = "json";
        private const string BudgetNearLimitType = "BudgetNearLimit";
        private const string BudgetOverBudgetType = "BudgetOverBudget";
        private const decimal NearLimitRatio = 0.8m;
        private const long MaxReceiptSizeBytes = 10 * 1024 * 1024;
        private static readonly HashSet<string> AllowedReceiptContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "application/pdf",
        };

        private readonly ExpenseRepository _repository;
        private readonly CategoryRepository categoryRepository;
        private readonly INotificationRepository notificationRepository;
        private readonly IReceiptStorage receiptStorage;
        private readonly ILogger<ExpenseController> logger;
        private readonly IDictionary<string, IDataStreamifier> dataStreamifiers;

        public ExpenseController(
            ExpenseRepository repository,
            CategoryRepository categoryRepository,
            INotificationRepository notificationRepository,
            IReceiptStorage receiptStorage,
            ILogger<ExpenseController> logger,
            IDictionary<string, IDataStreamifier> dataStreamifiers)
        {
            _repository = repository;
            this.categoryRepository = categoryRepository;
            this.notificationRepository = notificationRepository;
            this.receiptStorage = receiptStorage;
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
            var expense = await _repository.GetByIdAsync(id, cancellationToken);
            if (expense is null)
            {
                return NotFound();
            }

            return Ok(expense);
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
            var expense = await _repository.GetByIdAsync(id, cancellationToken);
            if (expense is null)
            {
                return NotFound();
            }

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

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            var expense = await _repository.GetByIdAsync(id, cancellationToken);
            if (expense is null)
            {
                return NotFound();
            }

            await _repository.DeleteByIdAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/receipt")]
        public async Task<ActionResult<Expense>> UploadReceipt(string id, IFormFile file, CancellationToken cancellationToken)
        {
            var expense = await _repository.GetByIdAsync(id, cancellationToken);
            if (expense is null)
            {
                return NotFound();
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (file.Length > MaxReceiptSizeBytes)
            {
                return BadRequest("Receipt file is too large. Maximum size is 10MB.");
            }

            if (!AllowedReceiptContentTypes.Contains(file.ContentType))
            {
                return BadRequest("Receipt must be a JPEG, PNG, or PDF file.");
            }

            if (!string.IsNullOrEmpty(expense.ReceiptImage))
            {
                await this.receiptStorage.DeleteAsync(expense.ReceiptImage, cancellationToken);
            }

            using (var stream = file.OpenReadStream())
            {
                expense.ReceiptImage = await this.receiptStorage.UploadAsync(id, file.FileName, stream, file.ContentType, cancellationToken);
            }

            var updated = await _repository.UpdateAsync(expense, cancellationToken);
            return Ok(updated);
        }

        [HttpGet("{id}/receipt")]
        public async Task<ActionResult> GetReceipt(string id, CancellationToken cancellationToken)
        {
            var expense = await _repository.GetByIdAsync(id, cancellationToken);
            if (expense is null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(expense.ReceiptImage))
            {
                return NotFound();
            }

            var (content, contentType) = await this.receiptStorage.OpenReadAsync(expense.ReceiptImage, cancellationToken);
            return File(content, contentType);
        }

        [HttpDelete("{id}/receipt")]
        public async Task<ActionResult<Expense>> DeleteReceipt(string id, CancellationToken cancellationToken)
        {
            var expense = await _repository.GetByIdAsync(id, cancellationToken);
            if (expense is null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(expense.ReceiptImage))
            {
                return Ok(expense);
            }

            await this.receiptStorage.DeleteAsync(expense.ReceiptImage, cancellationToken);
            expense.ReceiptImage = null;

            var updated = await _repository.UpdateAsync(expense, cancellationToken);
            return Ok(updated);
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

