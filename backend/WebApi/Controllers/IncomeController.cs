using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncomeController : ControllerBase
    {
        private readonly IRepository<Income> _repository;

        public IncomeController(IRepository<Income> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Income>>> GetAll(CancellationToken cancellationToken)
        {
            var incomes = await _repository.GetAllAsync(cancellationToken);
            return Ok(incomes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Income>> GetById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var income = await _repository.GetByIdAsync(id, cancellationToken);
                return Ok(income);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Income>> Create(CreateIncomeRequest request, CancellationToken cancellationToken)
        {
            var income = new Income
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                Amount = request.Amount,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(income, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = income.Id }, income);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, UpdateIncomeRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var income = await _repository.GetByIdAsync(id, cancellationToken);
                income.Name = request.Name;
                income.Description = request.Description;
                income.Amount = request.Amount;

                await _repository.UpdateAsync(income, cancellationToken);
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
