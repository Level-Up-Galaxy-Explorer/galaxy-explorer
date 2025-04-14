using galaxy_api.DTOs;
using galaxy_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace galaxy_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PlanetController : ControllerBase
    {
        private readonly IPlanetService _planetService;

        public PlanetController(IPlanetService planetService)
        {
            _planetService = planetService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanetDTO>>> GetPlanets()
        {
            var planets = await _planetService.GetAllPlanetsAsync();
            return Ok(planets);
        }

    }
}


