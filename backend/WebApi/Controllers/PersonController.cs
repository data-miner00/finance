using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IRepository<Person> _repository;

        public PersonController(IRepository<Person> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetAll(CancellationToken cancellationToken)
        {
            var person = await _repository.GetAllAsync(cancellationToken);
            return Ok(person);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var person = await _repository.GetByIdAsync(id, cancellationToken);
                return Ok(person);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Person>> Create(CreatePersonRequest request, CancellationToken cancellationToken)
        {
            var piggyBank = new Person
            {
                Name = request.Name,
                Description = request.Description,
                Alias = request.Alias,
            };

            var createdPerson = await _repository.CreateAsync(piggyBank, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdPerson.Id }, createdPerson);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Person>> Update(string id, UpdatePersonRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var person = await _repository.GetByIdAsync(id, cancellationToken);
                person.Name = request.Name;
                person.Description = request.Description;
                person.Alias = request.Alias;

                var updated = await _repository.UpdateAsync(person, cancellationToken);
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
