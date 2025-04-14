using ErrorOr;
using galaxy_api.DTOs;
using galaxy_api.DTOs.Crews;
using galaxy_api.Models;
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

        if (crew == null)
        {
            return NotFound();
        }

        return Ok(crew);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCrew([FromBody] CreateCrewDto crewDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ErrorOr<Crew> createCrewResult = await _crewsService.CreateCrew(crewDto);

        return createCrewResult.Match(
                crew => CreatedAtAction(nameof(GetCrew), new { id = crew.CrewId }, crew),
                errors => Problem(errors.FirstOrDefault().Code, title: errors.FirstOrDefault().Description)
            );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCrewDetails(int id, [FromBody] UpdateCrewDetailsDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ErrorOr<Success> updateResult = await _crewsService.UpdateCrewDetailsAsync(id, dto);

        return updateResult.Match<IActionResult>(
            success => NoContent(),
            errors => Problem(errors.FirstOrDefault().Code, title: errors.FirstOrDefault().Description)
        );

    }

    [HttpPost("{id:int}/members")]
    public async Task<IActionResult> AddCrewMembers(int id, [FromBody] UpdateCrewMembersDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        ErrorOr<Success> addResult = await _crewsService.AddCrewMembersAsync(id, dto);
        return addResult.Match<IActionResult>(
            success => NoContent(),
            errors => Problem(errors.FirstOrDefault().Code, title: errors.FirstOrDefault().Description)
        );
    }


    [HttpDelete("{id:int}/members")]
    public async Task<IActionResult> RemoveCrewMembers(int id, [FromBody] UpdateCrewMembersDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        ErrorOr<Success> removeResult = await _crewsService.RemoveCrewMembersAsync(id, dto);
        return removeResult.Match<IActionResult>(
            success => NoContent(),
            errors => Problem(errors.FirstOrDefault().Code, title: errors.FirstOrDefault().Description)
        );
    }

}