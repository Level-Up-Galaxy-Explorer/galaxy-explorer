using Microsoft.Extensions.Options;

namespace galaxy_cli.Services.Base;

public abstract class BaseApiService
{
    protected readonly IHttpClientFactory HttpClientFactory;
    protected readonly ApiSettings ApiSettings;

    protected abstract string EndpointPath { get; }

    protected BaseApiService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
    {
        HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        ApiSettings = apiSettings?.Value ?? throw new ArgumentNullException(nameof(apiSettings));

        if (string.IsNullOrWhiteSpace(ApiSettings.BaseUrl))
        {
            throw new InvalidOperationException("API BaseUrl is not configured.");
        }
    }

    protected virtual HttpClient CreateClient()
    {
        return HttpClientFactory.CreateClient();
    }

    protected virtual string GetFullUrl(string? relativePath = null)
    {
        if (string.IsNullOrWhiteSpace(EndpointPath))
        {
            throw new InvalidOperationException("EndpointPath must be defined in the derived service.");
        }

        string baseUrl = ApiSettings.BaseUrl!.TrimEnd('/') + "/";
        string endpoint = EndpointPath.TrimStart('/');

        string combined = baseUrl + endpoint;

        if (!string.IsNullOrWhiteSpace(relativePath))
        {
            combined = combined.TrimEnd('/') + "/" + relativePath.TrimStart('/');
        }

        return combined;
    }


}