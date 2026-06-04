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
                Name = request.Name,
                Description = request.Description,
                Type = (AccountType)request.AccountType,
                Balance = request.Balance,
                Currency = request.Currency,
            };

            var createdAccount = await _repository.CreateAsync(account, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdAccount.Id }, createdAccount);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Account>> Update(string id, UpdateAccountRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _repository.GetByIdAsync(id, cancellationToken);
                account.Name = request.Name;
                account.Description = request.Description;
                account.Type = (AccountType)request.AccountType;
                account.Balance = request.Balance;
                account.Currency = request.Currency;

                var updated = await _repository.UpdateAsync(account, cancellationToken);
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
