using System.Text.Json;
using galaxy_api.Models;
using Microsoft.Extensions.Options;
using galaxy_cli.Services.Base;
using Spectre.Console;


namespace galaxy_cli.Services;

    public class UserService : BaseApiService, IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public UserService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
            : base(httpClientFactory, apiSettings)
        {
            _httpClient = this.CreateClient();
            _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        }

        protected override string EndpointPath => "/user";

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync(GetFullUrl());
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    return JsonSerializer.Deserialize<IEnumerable<UserDTO>>(content, _jsonOptions) ?? new List<UserDTO>();
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializing user data: {ex.Message}");
                    return new List<UserDTO>();
                }
            }
            Console.WriteLine($"Error fetching users: {response.StatusCode}");
            return new List<UserDTO>();
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{GetFullUrl()}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    return JsonSerializer.Deserialize<UserDTO>(content, _jsonOptions);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializing user data: {ex.Message}");
                    return null;
                }
            }
            Console.WriteLine($"Error fetching user: {response.StatusCode}");
            return null;
        }

        public async Task<bool> AssignRankAsync(int userId, int rankId)
        {
            var payload = new { Rank_Id = rankId };
            var jsonContent = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"{GetFullUrl()}/{userId}/assign", content);

            if (response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine("[green]Rank assigned successfully![/]");
                return true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                AnsiConsole.WriteLine($"Failed to assign rank: {response.StatusCode} {error}");
                return false;
            }
        }

        public async Task<bool> DeactivateUserAsync(int userId)
        {
            var response = await _httpClient.PatchAsync($"{GetFullUrl()}/{userId}/deactivate", null);

            if (response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine("[green]User deactivated successfully![/]");
                return true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                AnsiConsole.WriteLine($"Failed to deactivate user: {response.StatusCode} {error}");
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(int id, UserDTO user)
        {
            var jsonContent = JsonSerializer.Serialize(user, _jsonOptions);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{GetFullUrl()}/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine("[green]User updated successfully![/]");
                return true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                AnsiConsole.WriteLine($"Failed to update user: {response.StatusCode} {error}");
                return false;
            }
        }
    }
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<bool> AssignRankAsync(int userId, int rankId);
        Task<bool> DeactivateUserAsync(int userId);
        Task<bool> UpdateUserAsync(int id, UserDTO user);
    }