using galaxy_api.DTOs;
using galaxy_api.Models;
using galaxy_api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace galaxy_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanetController : ControllerBase
    {
        private readonly IPlanetService _planetService;

        public PlanetController(IPlanetService planetService)
        {
            _planetService = planetService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanetDTO>>> GetAllPlanets()
        {
            var planets = await _planetService.GetAllPlanetsAsync();
            if (planets == null || !planets.Any())
            {
                return NotFound(new { Message = "No planets found." });
            }
            return Ok(planets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlanetDTO>> GetPlanet(int id)
        {
            var planet = await _planetService.GetPlanetAsync(id);
            if (planet == null)
            {
                return NotFound(new { Message = $"Planet with ID {id} not found." });
            }
            return Ok(planet);
        }

        [HttpGet("search/{name}")]
        public async Task<ActionResult<IEnumerable<PlanetDTO>>> SearchPlanets(string name)
        {
            var planets = await _planetService.SearchPlanetsAsync(name);
            if (planets == null || !planets.Any())
            {
                return NotFound(new { Message = $"No planets found with the name '{name}'." });
            }
            return Ok(planets);
        }

        [HttpPost]
        public async Task<ActionResult<PlanetDTO>> CreatePlanet([FromBody] PlanetDTO planetDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid planet data provided.", Errors = ModelState });
            }

            var createdPlanet = await _planetService.AddPlanetAsync(planetDto);
            return CreatedAtAction(nameof(GetAllPlanets), createdPlanet, new
            {
                Message = "Planet created successfully.",
                Planet = createdPlanet
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlanet(int id, [FromBody] PlanetDTO planetDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid planet data provided.", Errors = ModelState });
            }

            var success = await _planetService.UpdatePlanetAsync(id, planetDto);
            if (!success)
            {
                return NotFound(new { Message = $"Planet with ID {id} not found." });
            }

            return Ok(new { Message = "Planet updated successfully." });
        }
    }
}