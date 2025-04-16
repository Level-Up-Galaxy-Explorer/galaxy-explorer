using System.Text.Json;
using galaxy_api.DTOs;
using galaxy_cli.Services.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace galaxy_cli.Services;

public sealed class PlanetService : BaseApiService, IPlanetService
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonOptions;

    public PlanetService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, ILogger<PlanetService> logger)
    : base(httpClientFactory, apiSettings, logger)
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _httpClient = this.CreateClient();
    }

    protected override string EndpointPath => "/planet";

    public async Task<IEnumerable<PlanetDTO>> GetPlanetsAsync()
    {
        return await GetAndDeserializeAsync<IEnumerable<PlanetDTO>>();
    }
}

public interface IPlanetService
{
    Task<IEnumerable<PlanetDTO>> GetPlanetsAsync();
}