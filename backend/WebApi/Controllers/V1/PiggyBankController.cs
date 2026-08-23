using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PiggyBankController : ControllerBase
    {
        private readonly IRepository<PiggyBank> _repository;

        public PiggyBankController(IRepository<PiggyBank> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PiggyBank>>> GetAll(CancellationToken cancellationToken)
        {
            var piggyBanks = await _repository.GetAllAsync(cancellationToken);
            return Ok(piggyBanks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PiggyBank>> GetById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var piggyBank = await _repository.GetByIdAsync(id, cancellationToken);
                return Ok(piggyBank);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<PiggyBank>> Create(CreatePiggyBankRequest request, CancellationToken cancellationToken)
        {
            var piggyBank = new PiggyBank
            {
                Name = request.Name,
                Description = request.Description,
                Amount = request.Amount,
                Target = request.Target,
                Currency = request.Currency,
                Deadline = request.Deadline,
            };

            var createdPiggyBank = await _repository.CreateAsync(piggyBank, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdPiggyBank.Id }, createdPiggyBank);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PiggyBank>> Update(string id, UpdatePiggyBankRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var piggyBank = await _repository.GetByIdAsync(id, cancellationToken);
                piggyBank.Name = request.Name;
                piggyBank.Description = request.Description;
                piggyBank.Amount = request.Amount;
                piggyBank.Target = request.Target;
                piggyBank.Currency = request.Currency;
                piggyBank.Deadline = request.Deadline;

                var updated = await _repository.UpdateAsync(piggyBank, cancellationToken);
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
