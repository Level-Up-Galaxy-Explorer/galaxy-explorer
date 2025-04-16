using System.Text.Json;
using galaxy_api.DTOs;
using galaxy_cli.Services.Base;
using Microsoft.Extensions.Options;
using System.Text; 
using Spectre.Console; 
using galaxy_cli.Services;
using Microsoft.Extensions.Logging;

namespace galaxy_cli.Services;

public sealed class MissionService : BaseApiService, IMissionService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public MissionService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, ILogger<MissionService> logger) 
        : base(httpClientFactory, apiSettings, logger)
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _httpClient = this.CreateClient();
    }

    protected override string EndpointPath => "/mission";

    public async Task<IEnumerable<MissionDTO>> GetMissionsAsync()
    {
        var response = await _httpClient.GetAsync(GetFullUrl());

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<IEnumerable<MissionDTO>>>(content, _jsonOptions);
                return apiResponse?.Data ?? new List<MissionDTO>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing mission data: {ex.Message}");
                return new List<MissionDTO>();
            }
        }

        Console.WriteLine($"Error fetching missions: {response.StatusCode}");
        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine(errorContent);
        return new List<MissionDTO>();
    }

    public async Task<MissionDTO?> GetMissionByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{GetFullUrl()}/{id}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<MissionDTO>>(content, _jsonOptions);
                return apiResponse?.Data;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing mission data: {ex.Message}");
                return null;
            }
        }

        Console.WriteLine($"Error fetching mission: {response.StatusCode}");
        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine(errorContent);
        return null;
    }

    public async Task <bool>CreateMissionAsync(MissionDTO mission)
    {
        var jsonContent = JsonSerializer.Serialize(mission, _jsonOptions);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(GetFullUrl(), content);

        if (response.IsSuccessStatusCode)
        {
            AnsiConsole.MarkupLine("[green]Mission created successfully![/]");
            return true;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            AnsiConsole.WriteLine($"Failed to create mission: {response.StatusCode} {error}");
            return false;
        }
    }

    public async Task<bool> UpdateMissionDetailsAsync(int id, MissionDTO mission)
    {
        var jsonContent = JsonSerializer.Serialize(mission, _jsonOptions);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"{GetFullUrl()}/{id}", content);

        if (response.IsSuccessStatusCode)
        {
            AnsiConsole.MarkupLine("[green]Mission updated successfully![/]");
            return true;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            AnsiConsole.WriteLine($"Failed to update mission: {response.StatusCode} {error}");
            return false;
        }
    }

    public async Task<bool> UpdateMissionStatusAsync(int id, int statusId, string? feedback = null, string? rewardCredit = null)
    {
        var dto = new
        {
            Status_Id = statusId,
            Feedback = feedback,
            Reward_Credit = rewardCredit
        };

        var jsonContent = JsonSerializer.Serialize(dto, _jsonOptions);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PatchAsync($"{GetFullUrl()}/{id}/status", content);

        if (response.IsSuccessStatusCode)
        {
            AnsiConsole.MarkupLine("[green]Mission status updated successfully![/]");
            return true;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            AnsiConsole.WriteLine($"Failed to update mission status: {response.StatusCode} {error}");
            return false;
        }
    }

    public async Task<IEnumerable<MissionStatusReport>> GetMissionStatusReportAsync(string? missionType, string? status, string? groupBy)
    {
        var url = $"{GetFullUrl()}/report";
        var queryParams = new List<string>();
        if (!string.IsNullOrWhiteSpace(missionType))
            queryParams.Add($"missionType={Uri.EscapeDataString(missionType)}");
        if (!string.IsNullOrWhiteSpace(status))
            queryParams.Add($"status={Uri.EscapeDataString(status)}");
        if (!string.IsNullOrWhiteSpace(groupBy))
            queryParams.Add($"groupBy={Uri.EscapeDataString(groupBy)}");
        if (queryParams.Any())
            url += "?" + string.Join("&", queryParams);

        var response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<IEnumerable<MissionStatusReport>>>(content, _jsonOptions);
                return apiResponse?.Data ?? new List<MissionStatusReport>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing mission status report: {ex.Message}");
                return new List<MissionStatusReport>();
            }
        }

        Console.WriteLine($"Error fetching mission status report: {response.StatusCode}");
        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine(errorContent);
        return new List<MissionStatusReport>();
    }
}

public interface IMissionService
{
    Task<IEnumerable<MissionDTO>> GetMissionsAsync();
    Task<MissionDTO?> GetMissionByIdAsync(int id);
    Task<bool> CreateMissionAsync(MissionDTO mission);
    Task<bool> UpdateMissionDetailsAsync(int id, MissionDTO mission);
    Task<bool> UpdateMissionStatusAsync(int id, int statusId, string? feedback = null, string? rewardCredit = null);
    Task<IEnumerable<MissionStatusReport>> GetMissionStatusReportAsync(string? missionType, string? status, string? groupBy);
}

