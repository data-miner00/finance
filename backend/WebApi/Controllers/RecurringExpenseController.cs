using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecurringExpenseController : ControllerBase
    {
        private readonly IRepository<RecurringExpense> _repository;

        public RecurringExpenseController(IRepository<RecurringExpense> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecurringExpense>>> GetAll(CancellationToken cancellationToken)
        {
            var recurringExpenses = await _repository.GetAllAsync(cancellationToken);
            return Ok(recurringExpenses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecurringExpense>> GetById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var recurringExpense = await _repository.GetByIdAsync(id, cancellationToken);
                return Ok(recurringExpense);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<RecurringExpense>> Create(CreateRecurringExpenseRequest request, CancellationToken cancellationToken)
        {
            var recurringExpense = new RecurringExpense
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                RecurringAt = request.ExecutedAt,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(recurringExpense, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = recurringExpense.Id }, recurringExpense);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, UpdateRecurringExpenseRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var recurringExpense = await _repository.GetByIdAsync(id, cancellationToken);
                recurringExpense.Name = request.Name;
                recurringExpense.Description = request.Description;
                recurringExpense.IsActive = request.IsActive;
                recurringExpense.RecurringAt = request.ExecutedAt;

                await _repository.UpdateAsync(recurringExpense, cancellationToken);
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
