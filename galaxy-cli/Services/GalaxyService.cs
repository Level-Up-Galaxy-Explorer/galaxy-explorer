using System.Text.Json;
using galaxy_cli.DTO;
using galaxy_cli.Services.Base;
using Microsoft.Extensions.Options;
using System.Text;
using Spectre.Console;
using Microsoft.Extensions.Logging;

namespace galaxy_cli.Services;

public sealed class GalaxyService : BaseApiService, IGalaxyService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public GalaxyService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, ILogger<GalaxyService> logger)
        : base(httpClientFactory, apiSettings, logger)
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _httpClient = this.CreateClient();
    }

    protected override string EndpointPath => "/galaxy";

    public async Task<IEnumerable<GalaxyDTO>> GetAllGalaxyAsync()
    {
        var response = await _httpClient.GetAsync(GetFullUrl());

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonSerializer.Deserialize<IEnumerable<GalaxyDTO>>(content, _jsonOptions) ?? new List<GalaxyDTO>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing galaxy data: {ex.Message}");
                return new List<GalaxyDTO>();
            }
        }

        Console.WriteLine($"Error fetching galaxies: {response.StatusCode}");
        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine(errorContent);
        return new List<GalaxyDTO>();
    }

    public async Task<GalaxyDTO?> GetGalaxyByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{GetFullUrl()}/{id}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<GalaxyDTO>>(content, _jsonOptions);
                return apiResponse?.Data;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing galaxy data: {ex.Message}");
                return null;
            }
        }

        Console.WriteLine($"Error fetching galaxy: {response.StatusCode}");
        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine(errorContent);
        return null;
    }

    public async Task<bool> AddGalaxyAsync(GalaxyDTO galaxy)
    {
        var jsonContent = JsonSerializer.Serialize(galaxy, _jsonOptions);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(GetFullUrl(), content);

        if (response.IsSuccessStatusCode)
        {
            AnsiConsole.MarkupLine("[green]Galaxy created successfully![/]");
            return true;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            AnsiConsole.WriteLine($"Failed to create a galaxy: {response.StatusCode} {error}");
            return false;
        }
    }

    public async Task<bool> UpdateGalaxyAsync(int id, GalaxyDTO galaxy)
    {
        var jsonContent = JsonSerializer.Serialize(galaxy, _jsonOptions);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"{GetFullUrl()}/{id}", content);

        if (response.IsSuccessStatusCode)
        {
            AnsiConsole.MarkupLine("[green]Galaxy updated successfully![/]");
            return true;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            AnsiConsole.WriteLine($"Failed to update a galaxy: {response.StatusCode} {error}");
            return false;
        }
    }
}
