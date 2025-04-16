
using galaxy_cli.DTO.Crews;

namespace galaxy_cli.Services;

public interface ICrewsService
{
    Task CreateCrewAsync(CreateCrewDto crewDto);
    Task<CrewSummaryDTO?> GetCrewItemAsync(int id);
    Task<List<CrewSummaryDTO>?> GetAllCrewsAsync();

    Task AddCrewMembers(int crewId, UpdateCrewMembersDto updateCrew);

    Task RemoveCrewMembers(int crewId, UpdateCrewMembersDto memberIds);
}