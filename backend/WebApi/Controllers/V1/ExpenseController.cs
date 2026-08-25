using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using Core.Services;
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
        private const long MaxReceiptSizeBytes = 10 * 1024 * 1024;
        private static readonly HashSet<string> AllowedReceiptContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "application/pdf",
        };

        private readonly ExpenseRepository _repository;
        private readonly IBudgetAlertService budgetAlertService;
        private readonly IReceiptStorage receiptStorage;
        private readonly IDictionary<string, IDataStreamifier> dataStreamifiers;

        public ExpenseController(
            ExpenseRepository repository,
            IBudgetAlertService budgetAlertService,
            IReceiptStorage receiptStorage,
            IDictionary<string, IDataStreamifier> dataStreamifiers)
        {
            _repository = repository;
            this.budgetAlertService = budgetAlertService;
            this.receiptStorage = receiptStorage;
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
            await this.budgetAlertService.EvaluateAsync(createdExpense.CategoryName, cancellationToken);
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
            await this.budgetAlertService.EvaluateAsync(updated.CategoryName, cancellationToken);
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
            if (!this.dataStreamifiers.TryGetValue(format, out var streamifier))
            {
                return BadRequest($"Unsupported export format \"{format}\". Supported formats: {string.Join(", ", this.dataStreamifiers.Keys)}.");
            }

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
    }
}

