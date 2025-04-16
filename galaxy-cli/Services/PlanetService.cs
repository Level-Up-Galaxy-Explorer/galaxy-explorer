using System.Text.Json;
using System.Text;
using galaxy_cli.DTO.Planets;
using galaxy_cli.Services;
using galaxy_cli.Services.Base;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace galaxy_cli.Services;

public sealed class PlanetService : BaseApiService, IPlanetService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public PlanetService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings,  ILogger<MissionService> logger) 
        : base(httpClientFactory, apiSettings, logger)
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _httpClient = this.CreateClient();
    }

    protected override string EndpointPath => "/planet";

    public async Task<IEnumerable<PlanetDTO>> GetAllPlanetsAsync()
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

    public async Task<PlanetDTO> AddPlanetAsync(PlanetDTO planetDto)
    {
        var content = new StringContent(JsonSerializer.Serialize(planetDto, _jsonOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(GetFullUrl(), content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PlanetDTO>(responseContent, _jsonOptions) ?? throw new Exception("Failed to deserialize response.");
        }
        else
        {
            throw new Exception($"Error adding planet: {response.StatusCode}");
        }
    }

    public async Task<bool> UpdatePlanetAsync(int planetId, PlanetDTO planetDto)
    {
        var content = new StringContent(JsonSerializer.Serialize(planetDto, _jsonOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{GetFullUrl()}/{planetId}", content);

        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<string>> GetPlanetTypesAsync()
    {
        var response = await _httpClient.GetAsync($"{GetFullUrl()}/types");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonSerializer.Deserialize<IEnumerable<string>>(content, _jsonOptions) ?? new List<string>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing planet types: {ex.Message}");
                return new List<string>();
            }
        }
        else
        {
            Console.WriteLine($"Error fetching planet types: {response.StatusCode}");
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(errorContent);
            return new List<string>();
        }
    }

    public async Task<IEnumerable<string>> GetGalaxiesAsync()
    {
        var response = await _httpClient.GetAsync($"{GetFullUrl()}/galaxies");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonSerializer.Deserialize<IEnumerable<string>>(content, _jsonOptions) ?? new List<string>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing galaxies: {ex.Message}");
                return new List<string>();
            }
        }
        else
        {
            Console.WriteLine($"Error fetching galaxies: {response.StatusCode}");
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(errorContent);
            return new List<string>();
        }
    }
}