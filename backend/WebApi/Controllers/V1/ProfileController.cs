using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository _repository;

        public ProfileController(IProfileRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<Profile>> Get(CancellationToken cancellationToken)
        {
            var profile = await _repository.GetAsync(cancellationToken);
            if (profile is null)
            {
                return NoContent();
            }

            return Ok(profile);
        }

        [HttpPut]
        public async Task<ActionResult<Profile>> Save(SaveProfileRequest request, CancellationToken cancellationToken)
        {
            var profile = new Profile
            {
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Bio = request.Bio,
                CompanyName = request.CompanyName,
                WebsiteUrl = request.WebsiteUrl,
                AvatarImage = request.AvatarImage,
            };

            var saved = await _repository.SaveAsync(profile, cancellationToken);
            return Ok(saved);
        }
    }
}
