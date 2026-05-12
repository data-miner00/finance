using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IRepository<Account> _repository;

        public AccountController(IRepository<Account> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAll(CancellationToken cancellationToken)
        {
            var accounts = await _repository.GetAllAsync(cancellationToken);
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _repository.GetByIdAsync(id, cancellationToken);
                return Ok(account);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Account>> Create(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            var account = new Account
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                AccountType = (AccountType)request.AccountType,
                Amount = request.Amount,
                Currency = request.Currency,
                ActionedAt = request.ActionedAt,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(account, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, UpdateAccountRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _repository.GetByIdAsync(id, cancellationToken);
                account.Name = request.Name;
                account.Description = request.Description;
                account.AccountType = (AccountType)request.AccountType;
                account.Amount = request.Amount;
                account.Currency = request.Currency;
                account.ActionedAt = request.ActionedAt;
                account.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(account, cancellationToken);
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
