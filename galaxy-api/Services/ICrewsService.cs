using ErrorOr;
using galaxy_api.DTOs;
using galaxy_api.DTOs.Crews;
using galaxy_api.Models;

namespace galaxy_api.Services;

public interface ICrewsService
{

    Task<IEnumerable<CrewDTO>> GetAllCrewsAsync();

    Task<CrewDTO?> GetCrewAsync(int crew_id);

    Task<ErrorOr<Crew>> CreateCrew(CreateCrewDto crewDto);

    Task<ErrorOr<Success>> UpdateCrewDetailsAsync(int crewId, UpdateCrewDetailsDTO dto);
    Task<ErrorOr<Success>> AddCrewMembersAsync(int crewId, UpdateCrewMembersDto dto);
    Task<ErrorOr<Success>> RemoveCrewMembersAsync(int crewId, UpdateCrewMembersDto dto);
    Task<ErrorOr<CrewMissionSummaryDTO>> GetCrewMissionHistoryAsync(int crewId);
}