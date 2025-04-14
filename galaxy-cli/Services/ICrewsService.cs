
using galaxy_cli.DTO.Crews;

namespace galaxy_cli.Services;

public interface ICrewsService
{
    Task<CrewSummaryDTO?> GetCrewItemAsync(int id);
    Task<List<CrewSummaryDTO>?> GetAllCrewsAsync();
}