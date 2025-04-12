using galaxy_api.Models;
using galaxy_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace galaxy_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GalaxyController : ControllerBase
    {
        private readonly IGalaxyService _galaxyService;

        public GalaxyController(IGalaxyService galaxyService)
        {
            _galaxyService = galaxyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Galaxy>>> GetGalaxies()
        {
            var galaxies = await _galaxyService.GetAllGalaxyAsync();

            var galaxy = galaxies.Select(g => new Galaxy
            {
                Galaxy_Id = g.Galaxy_Id,
                Name = g.Name,
                Galaxy_Type_Id = g.Galaxy_Type_Id, 
                Distance_From_Earth = g.Distance_From_Earth,
                Description = g.Description
            });

            return Ok(galaxy);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Galaxy>> GetGalaxyById(int id)
        {
            var galaxy = await _galaxyService.GetGalaxyByIdAsync(id);
            if (galaxy == null)
            {
                return NotFound();
            }

            var galaxyModel = new Galaxy
            {
                Galaxy_Id = galaxy.Galaxy_Id,
                Name = galaxy.Name,
                Galaxy_Type_Id = galaxy.Galaxy_Type_Id, 
                Distance_From_Earth = galaxy.Distance_From_Earth,
                Description = galaxy.Description
            };

            return Ok(galaxyModel);
        }

        [HttpPost]
        public async Task<ActionResult> CreateGalaxy(Galaxy galaxy)
        {
            if (galaxy == null)
            {
                return BadRequest("Galaxy data is required.");
            }

            var galaxyModel = new Galaxy
            {
                Name = galaxy.Name,
                Galaxy_Type_Id = galaxy.Galaxy_Type_Id,  
                Distance_From_Earth = galaxy.Distance_From_Earth,
                Description = galaxy.Description
            };

            await _galaxyService.AddGalaxyAsync(galaxy);

            return CreatedAtAction(nameof(GetGalaxyById), new { id = galaxy.Galaxy_Id }, galaxy);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateGalaxy(int id, Galaxy galaxy)
        {
            var galaxyModel = await _galaxyService.GetGalaxyByIdAsync(id);
            if (galaxy == null)
            {
                return NotFound();
            }

            galaxy.Name = galaxy.Name;
            galaxy.Galaxy_Type_Id = galaxy.Galaxy_Type_Id; 
            galaxy.Distance_From_Earth = galaxy.Distance_From_Earth;
            galaxy.Description = galaxy.Description;

            await _galaxyService.UpdateGalaxyAsync(id, galaxy);

            return Ok(galaxy);  
        }
    }
}
