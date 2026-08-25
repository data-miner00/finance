using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
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
            var account = await _repository.GetByIdAsync(id, cancellationToken);
            if (account is null)
            {
                return NotFound();
            }

            return this.Ok(account);
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
                AnnualSpendTarget = request.AnnualSpendTarget,
            };

            var createdAccount = await _repository.CreateAsync(account, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdAccount.Id }, createdAccount);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Account>> Update(string id, UpdateAccountRequest request, CancellationToken cancellationToken)
        {
            var account = await _repository.GetByIdAsync(id, cancellationToken);
            if (account is null)
            {
                return this.NotFound();
            }

            account.Name = request.Name;
            account.Description = request.Description;
            account.Type = (AccountType)request.AccountType;
            account.Balance = request.Balance;
            account.Currency = request.Currency;
            account.AnnualSpendTarget = request.AnnualSpendTarget;

            var updated = await _repository.UpdateAsync(account, cancellationToken);
            return this.Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            var account = await _repository.GetByIdAsync(id, cancellationToken);
            if (account is null)
            {
                return this.NotFound();
            }

            await _repository.DeleteByIdAsync(id, cancellationToken);
            return this.NoContent();
        }
    }
}
