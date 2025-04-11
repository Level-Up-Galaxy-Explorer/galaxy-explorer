using galaxy_api.DTOs;

namespace galaxy_api.Services;

public interface ICrewsService
{

    Task<IEnumerable<CrewDTO>> GetAllCrewsAsync();
}