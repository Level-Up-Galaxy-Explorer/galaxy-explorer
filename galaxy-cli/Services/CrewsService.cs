using System.Net.Http.Json;
using galaxy_cli.DTO.Crews;
using galaxy_cli.Services.Base;
using Microsoft.Extensions.Options;

namespace galaxy_cli.Services;

public class CrewsService : BaseApiService, ICrewsService
{
    public CrewsService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings) : base(httpClientFactory, apiSettings)
    {
    }

    protected override string EndpointPath => "crews";

    public async Task<List<CrewSummaryDTO>?> GetAllCrewsAsync()
    {
        try
        {
            HttpClient client = CreateClient();
            string url = GetFullUrl();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var crewItems = await response.Content.ReadFromJsonAsync<List<CrewSummaryDTO>>();

            return crewItems;

        }
        catch (HttpRequestException ex)
        {
            //TODO Add error handling
            return null;
        }
    }

    public async Task<CrewSummaryDTO?> GetCrewItemAsync(int id)
    {
        try
        {
            HttpClient client = CreateClient();
            string url = GetFullUrl(id.ToString());
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var crewItems = await response.Content.ReadFromJsonAsync<CrewSummaryDTO>();

            return crewItems;

        }
        catch (HttpRequestException ex)
        {

            return null;
        }
    }
}