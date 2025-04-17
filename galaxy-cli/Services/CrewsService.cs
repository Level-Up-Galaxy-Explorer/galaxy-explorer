using System.Net.Http.Json;
using galaxy_api.DTOs.Crews;
using galaxy_cli.DTO.Crews;
using galaxy_cli.Services.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace galaxy_cli.Services;

public class CrewsService : BaseApiService, ICrewsService
{
    public CrewsService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, ILogger<CrewsService> logger)
    : base(httpClientFactory, apiSettings, logger) { }

    protected override string EndpointPath => "crews";

    public async Task AddCrewMembers(int crewId, UpdateCrewMembersDto memberIds)
    {
        await PostAsync(memberIds, $"{crewId}/members");
    }

    public async Task CreateCrewAsync(CreateCrewDto crewDto)
    {
        await PostAsync(crewDto);
    }

    public async Task<List<CrewSummaryDTO>?> GetAllCrewsAsync()
    {

        return await GetAndDeserializeAsync<List<CrewSummaryDTO>>();
    }

    public async Task<CrewSummaryDTO?> GetCrewItemAsync(int id)
    {
        return await GetAndDeserializeAsync<CrewSummaryDTO>(id.ToString()); ;
    }

    public async Task<CrewMissionSummaryDTO> GetCrewMissionHistory(int id)
    {
        return await GetAndDeserializeAsync<CrewMissionSummaryDTO>($"{id}/history");
    }

    public async Task RemoveCrewMembers(int crewId, UpdateCrewMembersDto memberIds)
    {
        await PatchAsync(memberIds, $"{crewId}/members");
    }
}