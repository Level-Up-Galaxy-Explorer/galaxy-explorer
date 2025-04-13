using galaxy_api.DTOs;

namespace galaxy_api.Repositories;

public interface ICrewsRepositoty {
    Task<IEnumerable<CrewDTO>> GetAllCrewsAsync();

    Task<CrewDTO?> GetCrewAsync(int crew_id);
}