using galaxy_api.DTOs;

namespace galaxy_api.Repositories;

public interface ICrewsRepositoty {
    Task<IEnumerable<CrewDTO>> GetAllCrewsAsync();
}