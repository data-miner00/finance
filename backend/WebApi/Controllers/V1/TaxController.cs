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
            var tax = await _repository.GetByIdAsync(id, cancellationToken);
            if (tax is null)
            {
                return NotFound();
            }

            return Ok(tax);
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
            var tax = await _repository.GetByIdAsync(id, cancellationToken);
            if (tax is null)
            {
                return NotFound();
            }

            tax.Name = request.Name;
            tax.Description = request.Description;
            tax.Amount = request.Amount;

            var updated = await _repository.UpdateAsync(tax, cancellationToken);
            return this.Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            var tax = await _repository.GetByIdAsync(id, cancellationToken);
            if (tax is null)
            {
                return NotFound();
            }

            await _repository.DeleteByIdAsync(id, cancellationToken);
            return NoContent();
        }
    }
}

