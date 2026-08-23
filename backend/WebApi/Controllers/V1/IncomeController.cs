using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
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
                Name = request.Name,
                Description = request.Description,
                Amount = request.Amount,
                Currency = request.Currency,
                AccountId = request.AccountId,
            };

            var createdIncome = await _repository.CreateAsync(income, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdIncome.Id }, createdIncome);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Income>> Update(string id, UpdateIncomeRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var income = await _repository.GetByIdAsync(id, cancellationToken);
                income.Name = request.Name;
                income.Description = request.Description;
                income.Amount = request.Amount;
                income.Currency = request.Currency;
                income.AccountId = request.AccountId;

                var updated = await _repository.UpdateAsync(income, cancellationToken);
                return this.Ok(updated);
            }
            catch
            {
                return this.NotFound();
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
