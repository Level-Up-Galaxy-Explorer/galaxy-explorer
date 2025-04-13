using galaxy_api.Models;
using galaxy_api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        public async Task<ActionResult<IEnumerable<Planet>>> GetAllPlanets()
        {
            var planets = await _planetService.GetAllPlanetsAsync();
            return Ok(planets);
        }

        [HttpPost]
        public async Task<ActionResult<Planet>> CreatePlanet(Planet planet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdPlanet = await _planetService.AddPlanetAsync(planet);
                return CreatedAtAction(nameof(GetAllPlanets), createdPlanet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the planet");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlanet(int id, Planet planet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var success = await _planetService.UpdatePlanetAsync(id, planet);
                if (!success)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the planet");
            }
        }
    }
}