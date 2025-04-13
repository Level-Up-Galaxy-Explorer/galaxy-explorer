using galaxy_api.DTOs;
using galaxy_api.Repositories;

namespace galaxy_api.Services;

class CrewsService : ICrewsService
{
    private readonly ICrewsRepositoty _repository;

    public CrewsService(ICrewsRepositoty repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CrewDTO>> GetAllCrewsAsync()
    {
        return await _repository.GetAllCrewsAsync();
    }

    public async Task<CrewDTO?> GetCrewAsync(int crew_id)
    {
        return await _repository.GetCrewAsync(crew_id);
    }
}