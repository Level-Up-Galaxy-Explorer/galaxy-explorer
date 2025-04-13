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

    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<CrewDTO>>> GetCrew(int id)
    {
        var crew = await _crewsService.GetCrewAsync(id);

        if (crew == null) {
            return NotFound();
        }

        return Ok(crew);
    }

}