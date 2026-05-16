using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IRepository<Expense> _repository;

        public ExpenseController(IRepository<Expense> repository)
        {
            _repository = repository;
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
                Id = Guid.NewGuid().ToString(),
                Category = request.Category,
                Name = request.Name,
                Description = request.Description,
                Amount = request.Amount,
                Currency = request.Currency,
                Location = request.Location,
                ReceiptImage = request.ReceiptImage,
                CreatedAt = DateTime.UtcNow
            };

            if (!string.IsNullOrWhiteSpace(request.ActionedAt)
                && DateTime.TryParse(request.ActionedAt, out var actionedAt))
            {
                expense.ActionedAt = actionedAt;
            }

            await _repository.CreateAsync(expense, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, UpdateExpenseRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var expense = await _repository.GetByIdAsync(id, cancellationToken);
                expense.Category = request.Category;
                expense.Name = request.Name;
                expense.Description = request.Description;
                expense.Amount = request.Amount;
                expense.Currency = request.Currency;
                expense.Location = request.Location;
                expense.ReceiptImage = request.ReceiptImage;
                expense.ActionedAt = request.ActionedAt;

                await _repository.UpdateAsync(expense, cancellationToken);
                return NoContent();
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
    }
}

