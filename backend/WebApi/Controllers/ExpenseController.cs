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

        private readonly ExpenseRepository _repository;
        private readonly IDictionary<string, IDataStreamifier> dataStreamifiers;

        public ExpenseController(
            ExpenseRepository repository,
            IDictionary<string, IDataStreamifier> dataStreamifiers)
        {
            _repository = repository;
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
                Name = request.Name,
                Description = request.Description,
                Amount = request.Amount,
                Currency = request.Currency,
                Location = request.Location,
                ReceiptImage = request.ReceiptImage,
                AgentName = request.AgentName,
            };

            var createdExpense = await _repository.CreateAsync(expense, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdExpense.Id }, createdExpense);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Expense>> Update(string id, UpdateExpenseRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var expense = await _repository.GetByIdAsync(id, cancellationToken);
                expense.CategoryName = request.CategoryName;
                expense.Name = request.Name;
                expense.Description = request.Description;
                expense.Amount = request.Amount;
                expense.Currency = request.Currency;
                expense.Location = request.Location;
                expense.ReceiptImage = request.ReceiptImage;
                expense.ActionedAt = request.ActionedAt;
                expense.AgentName = request.AgentName;

                var updated = await _repository.UpdateAsync(expense, cancellationToken);
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
    }
}

