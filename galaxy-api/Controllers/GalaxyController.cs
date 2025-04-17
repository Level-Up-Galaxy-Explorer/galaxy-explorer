using galaxy_api.Models;
using galaxy_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace galaxy_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GalaxyController : ControllerBase
    {
        private readonly IGalaxyService _galaxyService;
        private readonly ILogger<GalaxyController> _logger;

        public GalaxyController(IGalaxyService galaxyService, ILogger<GalaxyController> logger)
        {
            _galaxyService = galaxyService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Galaxy>>> GetGalaxies()
        {
            var galaxies = await _galaxyService.GetAllGalaxyAsync();

            return Ok(galaxies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Galaxy>> GetGalaxyById(int id)
        {
            var galaxy = await _galaxyService.GetGalaxyByIdAsync(id);
            if (galaxy == null)
            {
                _logger.LogWarning("Galaxy with ID {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Found galaxy: {@Galaxy}", galaxy);
            return Ok(galaxy);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<string>>> GetGalaxyTypes()
        {
            var types = await _galaxyService.GetGalaxyTypesAsync();
            return Ok(types);
        }

        [HttpPost]
        public async Task<ActionResult> CreateGalaxy(Galaxy galaxy)
        {
            if (galaxy == null)
            {
                return BadRequest("Galaxy data is required.");
            }

            await _galaxyService.AddGalaxyAsync(galaxy);

            return CreatedAtAction(nameof(GetGalaxyById), new { id = galaxy.Galaxy_Id }, galaxy);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateGalaxy(int id, Galaxy galaxy)
        {
            var galaxyModel = await _galaxyService.GetGalaxyByIdAsync(id);
            if (galaxyModel == null)
            {
                return NotFound();
            }

            await _galaxyService.UpdateGalaxyAsync(id, galaxy);

            return Ok(galaxy);
        }
    }
}
