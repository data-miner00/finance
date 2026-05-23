using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

            await _repository.CreateAsync(piggyBank, cancellationToken);
            // Get Id from database
            return CreatedAtAction(nameof(GetById), new { id = piggyBank.Id }, piggyBank);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, UpdatePiggyBankRequest request, CancellationToken cancellationToken)
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

                await _repository.UpdateAsync(piggyBank, cancellationToken);
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
