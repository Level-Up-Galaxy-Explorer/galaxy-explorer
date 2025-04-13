using galaxy_api.Models;
using galaxy_api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace galaxy_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanetController : ControllerBase
    {
        private readonly IPlanetRepository _planetRepository;

        public PlanetController(IPlanetRepository planetRepository)
        {
            _planetRepository = planetRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Planet>>> GetAllPlanets()
        {
            var planets = await _planetRepository.GetAllPlanetsAsync();
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
                var createdPlanet = await _planetRepository.AddPlanetAsync(planet);
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
                var success = await _planetRepository.UpdatePlanetAsync(id, planet);
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