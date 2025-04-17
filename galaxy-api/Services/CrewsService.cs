using ErrorOr;
using galaxy_api.DTOs;
using galaxy_api.DTOs.Crews;
using galaxy_api.Models;
using galaxy_api.Repositories;

namespace galaxy_api.Services;

class CrewsService : ICrewsService
{
    private readonly ICrewsRepositoty _repository;

    public CrewsService(ICrewsRepositoty repository)
    {
        _repository = repository;
    }

    public async Task<ErrorOr<Success>> AddCrewMembersAsync(int crewId, UpdateCrewMembersDto dto)
    {
        return await _repository.AddCrewMembersAsync(crewId, dto);
    }

    public async Task<ErrorOr<Crew>> CreateCrew(CreateCrewDto crewDto)
    {
        return await _repository.CreateCrew(crewDto);
    }

    public async Task<IEnumerable<CrewDTO>> GetAllCrewsAsync()
    {
        return await _repository.GetAllCrewsAsync();
    }

    public async Task<CrewDTO?> GetCrewAsync(int crew_id)
    {
        return await _repository.GetCrewAsync(crew_id);
    }

    public async Task<ErrorOr<CrewMissionSummaryDTO>> GetCrewMissionHistoryAsync(int crewId)
    {
        return await _repository.GetCrewMissionHistoryAsync(crewId);
    }

    public async Task<ErrorOr<Success>> RemoveCrewMembersAsync(int crewId, UpdateCrewMembersDto dto)
    {
        return await _repository.RemoveCrewMembersAsync(crewId, dto);
    }

    public async Task<ErrorOr<Success>> UpdateCrewDetailsAsync(int crewId, UpdateCrewDetailsDTO dto)
    {
        return await _repository.UpdateCrewDetailsAsync(crewId, dto);
    }
}