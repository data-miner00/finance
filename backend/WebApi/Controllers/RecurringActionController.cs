using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/recurring")]
    public class RecurringActionController : ControllerBase
    {
        private readonly IRepository<RecurringAction> _repository;

        public RecurringActionController(IRepository<RecurringAction> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecurringAction>>> GetAll(CancellationToken cancellationToken)
        {
            var recurringActions = await _repository.GetAllAsync(cancellationToken);
            return Ok(recurringActions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecurringAction>> GetById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var recurringAction = await _repository.GetByIdAsync(id, cancellationToken);
                return Ok(recurringAction);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<RecurringAction>> Create(CreateRecurringActionRequest request, CancellationToken cancellationToken)
        {
            var recurringAction = new RecurringAction
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                Amount = request.Amount,
                RecurringAt = request.RecurringAt,
                Type = request.Type,
            };

            await _repository.CreateAsync(recurringAction, cancellationToken);
            // Get Id from database
            return CreatedAtAction(nameof(GetById), new { id = recurringAction.Id }, recurringAction);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, UpdateRecurringActionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var recurringAction = await _repository.GetByIdAsync(id, cancellationToken);
                recurringAction.Name = request.Name;
                recurringAction.Description = request.Description;
                recurringAction.Amount = request.Amount;
                recurringAction.IsActive = request.IsActive;
                recurringAction.RecurringAt = request.RecurringAt;

                await _repository.UpdateAsync(recurringAction, cancellationToken);
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
