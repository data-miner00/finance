using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TaxController : ControllerBase
    {
        private readonly IRepository<Tax> _repository;

        public TaxController(IRepository<Tax> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tax>>> GetAll(CancellationToken cancellationToken)
        {
            var expenses = await _repository.GetAllAsync(cancellationToken);
            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tax>> GetById(string id, CancellationToken cancellationToken)
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
        public async Task<ActionResult<Tax>> Create(CreateTaxRequest request, CancellationToken cancellationToken)
        {
            var expense = new Tax
            {
                Name = request.Name,
                Description = request.Description,
                Amount = request.Amount,
            };

            var createdTax = await _repository.CreateAsync(expense, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdTax.Id }, createdTax);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Tax>> Update(string id, UpdateTaxRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var expense = await _repository.GetByIdAsync(id, cancellationToken);
                expense.Name = request.Name;
                expense.Description = request.Description;
                expense.Amount = request.Amount;

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
    }
}

