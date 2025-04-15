using System.Text.Json;
using galaxy_api.DTOs;
using galaxy_cli.Services.Base;
using Microsoft.Extensions.Options;

namespace galaxy_cli.Services;

public sealed class PlanetService : BaseApiService, IPlanetService
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonOptions;

    public PlanetService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings) : base(httpClientFactory, apiSettings)
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
        var response = await _httpClient.GetAsync(GetFullUrl());

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonSerializer.Deserialize<IEnumerable<PlanetDTO>>(content, _jsonOptions) ?? new List<PlanetDTO>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing planet data: {ex.Message}");
                return new List<PlanetDTO>();
            }
        }
        else
        {
            Console.WriteLine($"Error fetching planets: {response.StatusCode}");
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(errorContent);
            return new List<PlanetDTO>();
        }
    }
}

public interface IPlanetService
{
    Task<IEnumerable<PlanetDTO>> GetPlanetsAsync();
}