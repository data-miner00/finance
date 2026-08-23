using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsRepository _repository;

        public SettingsController(ISettingsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Setting>>> GetAll(CancellationToken cancellationToken)
        {
            var settings = await _repository.GetAllAsync(cancellationToken);
            return Ok(settings);
        }

        [HttpPut]
        public async Task<ActionResult<IEnumerable<Setting>>> Save(UpdateSettingsRequest request, CancellationToken cancellationToken)
        {
            var updated = await _repository.UpsertManyAsync(request.Values, cancellationToken);
            return Ok(updated);
        }
    }
}
