using galaxy_api.DTOs;
using galaxy_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace galaxy_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CrewsController : ControllerBase
{
    private readonly ICrewsService _crewsService;

    public CrewsController(ICrewsService crewsService)
    {
        _crewsService = crewsService;
    }

    [HttpGet]
        public async Task<ActionResult<IEnumerable<CrewDTO>>> GetCrews()
        {
            var crews = await _crewsService.GetAllCrewsAsync();
            return Ok(crews);
        }

}