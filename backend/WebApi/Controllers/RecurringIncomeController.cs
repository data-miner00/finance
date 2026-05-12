using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecurringIncomeController : ControllerBase
    {
        private readonly IRepository<RecurringIncome> _repository;

        public RecurringIncomeController(IRepository<RecurringIncome> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecurringIncome>>> GetAll(CancellationToken cancellationToken)
        {
            var recurringIncomes = await _repository.GetAllAsync(cancellationToken);
            return Ok(recurringIncomes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecurringIncome>> GetById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var recurringIncome = await _repository.GetByIdAsync(id, cancellationToken);
                return Ok(recurringIncome);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<RecurringIncome>> Create(CreateRecurringIncomeRequest request, CancellationToken cancellationToken)
        {
            var recurringIncome = new RecurringIncome
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                ExecutedAt = request.ExecutedAt,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(recurringIncome, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = recurringIncome.Id }, recurringIncome);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, UpdateRecurringIncomeRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var recurringIncome = await _repository.GetByIdAsync(id, cancellationToken);
                recurringIncome.Name = request.Name;
                recurringIncome.Description = request.Description;
                recurringIncome.IsActive = request.IsActive;
                recurringIncome.ExecutedAt = request.ExecutedAt;

                await _repository.UpdateAsync(recurringIncome, cancellationToken);
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
