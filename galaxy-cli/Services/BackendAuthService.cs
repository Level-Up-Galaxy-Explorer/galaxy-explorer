using System.Text;
using System.Text.Json;
using galaxy_cli.Services.Base;
using Microsoft.Extensions.Options;

namespace galaxy_cli.Services;

public sealed class BackendAuthService : IBackendAuthService
{
    private readonly HttpClient _httpClient;
    private readonly string _backendBaseUrl;

    public BackendAuthService(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
    {
        _httpClient = httpClient;
        _backendBaseUrl = $"{apiSettings.Value.BaseUrl}/auth";
    }

    public async Task<string?> ExchangeCodeWithBackendAsync(string code, string codeVerifier, string redirectUri)
    {
        var payload = new { Code = code, CodeVerifier = codeVerifier, RedirectUri = redirectUri };
        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_backendBaseUrl}/google", content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            using var jsonDocument = JsonDocument.Parse(result);
            if (jsonDocument.RootElement.TryGetProperty("token", out var tokenElement))
            {
                return tokenElement.GetString();
            }
            else
            {
                Console.WriteLine("Error: Backend response did not contain a 'token' field.");
                return null;
            }
        }
        else
        {
            Console.WriteLine($"Error during backend exchange: {response.StatusCode}");
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(errorContent);
            return null;
        }
    }
}