using System.Net.Http.Json;
using System.Text.Json;
using galaxy_cli.Commands.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace galaxy_cli.Services.Base;

public abstract class BaseApiService
{
    protected readonly IHttpClientFactory HttpClientFactory;
    protected readonly ApiSettings ApiSettings;
    protected readonly ILogger Logger;

    protected abstract string EndpointPath { get; }

    protected BaseApiService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, ILogger<BaseApiService> logger)
    {
        HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        ApiSettings = apiSettings?.Value ?? throw new ArgumentNullException(nameof(apiSettings));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (string.IsNullOrWhiteSpace(ApiSettings.BaseUrl))
        {
            throw new InvalidOperationException("API BaseUrl is not configured.");
        }
    }

    protected virtual HttpClient CreateClient()
    {
        return HttpClientFactory.CreateClient("BaseApiService");
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

    protected virtual void EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            ApiException exception = ApiException.CreateFromHttpResponse(response);
            response.Dispose();
            throw exception;
        }
    }

    private async Task<HttpResponseMessage> ExecuteRequestAsync(Func<HttpClient, Task<HttpResponseMessage>> requestFunc, Uri requestUri, CancellationToken cancellationToken)
    {
        HttpClient client = CreateClient();
        HttpResponseMessage? response = null;
        try
        {
            Logger.LogDebug("Sending request to {ApiUrl}", requestUri);
            response = await requestFunc(client).ConfigureAwait(false);
            Logger.LogDebug("Received response {StatusCode} from {ApiUrl}", response.StatusCode, requestUri);

            EnsureSuccessStatusCode(response);

            return response;
        }
        catch (HttpRequestException httpEx)
        {
            response?.Dispose();
            Logger.LogError(httpEx, "HTTP request failed for URL: {ApiUrl} (likely network issue).", requestUri);
            throw new ApiNetworkException($"Network error calling API URL: {requestUri}. Error: {httpEx.Message}", requestUri, httpEx);
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            response?.Dispose();
            Logger.LogWarning("API request cancelled for URL: {ApiUrl}", requestUri);
            throw;
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            response?.Dispose();
            Logger.LogError(ex, "An unexpected error occurred calling API URL: {ApiUrl}", requestUri);
            throw new ApiException($"An unexpected error occurred calling {requestUri}.", requestUri, ex);
        }
    }

    protected virtual async Task<TResponse> GetAndDeserializeAsync<TResponse>(string? relativePath = null, CancellationToken cancellationToken = default)
    {
        string url = GetFullUrl(relativePath);
        Uri requestUri = new Uri(url, UriKind.Absolute);

        using var response = await ExecuteRequestAsync(
            client => client.GetAsync(requestUri, cancellationToken),
            requestUri,
            cancellationToken).ConfigureAwait(false);

        try
        {
            TResponse? result = await response.Content.ReadFromJsonAsync<TResponse>().ConfigureAwait(false);
            if (result == null)
            {
                throw new ApiDeserializationException($"API returned unexpected null content for URL: {requestUri}", requestUri, new NullReferenceException("ReadFromJsonAsync returned null"));
            }
            Logger.LogDebug("Successfully deserialized GET response from {ApiUrl}", requestUri);
            return result;
        }
        catch (JsonException jsonEx)
        {
            throw new ApiDeserializationException($"Failed to deserialize GET JSON response from {requestUri}. Error: {jsonEx.Message}", requestUri, jsonEx);
        }
        catch (NotSupportedException nsEx)
        {
            throw new ApiDeserializationException($"Content type not supported for JSON deserialization from {requestUri}. Error: {nsEx.Message}", requestUri, nsEx);
        }
    }

    protected virtual async Task PostAsync<TRequest>(TRequest requestData, string? relativePath = null, CancellationToken cancellationToken = default)
    {
        string url = GetFullUrl(relativePath);
        Uri requestUri = new Uri(url, UriKind.Absolute);
        using var _ = await ExecuteRequestAsync(
            client => client.PostAsJsonAsync(requestUri, requestData),
            requestUri,
            cancellationToken).ConfigureAwait(false);
        Logger.LogDebug("Successfully executed POST request to {ApiUrl}", requestUri);
    }

    protected virtual async Task<TResponse> PostAndDeserializeAsync<TRequest, TResponse>(TRequest requestData, string? relativePath = null, CancellationToken cancellationToken = default)
    {
        string url = GetFullUrl(relativePath);
        Uri requestUri = new Uri(url, UriKind.Absolute);
        using var response = await ExecuteRequestAsync(
            client => client.PostAsJsonAsync(requestUri, requestData),
            requestUri,
            cancellationToken).ConfigureAwait(false);
        try
        {
            TResponse? result = await response.Content.ReadFromJsonAsync<TResponse>()
            .ConfigureAwait(false) ??

            throw new ApiDeserializationException(
                    $"API returned unexpected null content for URL: {requestUri}",
                    requestUri,
                    new NullReferenceException("ReadFromJsonAsync returned null"));

            Logger.LogDebug("Successfully executed POST and deserialized response from {ApiUrl}", requestUri);
            return result;
        }
        catch (JsonException jsonEx)
        {
            throw new ApiDeserializationException($"Failed to deserialize GET JSON response from {requestUri}. Error: {jsonEx.Message}", requestUri, jsonEx);
        }
        catch (NotSupportedException nsEx)
        {
            throw new ApiDeserializationException($"Content type not supported for JSON deserialization from {requestUri}. Error: {nsEx.Message}", requestUri, nsEx);
        }
    }

    protected virtual async Task PutAsync<TRequest>(TRequest requestData, string? relativePath = null, CancellationToken cancellationToken = default)
    {
        string url = GetFullUrl(relativePath);
        Uri requestUri = new Uri(url, UriKind.Absolute);
        using var _ = await ExecuteRequestAsync(
            client => client.PutAsJsonAsync(requestUri, requestData),
            requestUri,
            cancellationToken).ConfigureAwait(false);
        Logger.LogDebug("Successfully executed PUT request to {ApiUrl}", requestUri);
    }

    protected virtual async Task PatchAsync<TRequest>(TRequest requestData, string? relativePath = null, CancellationToken cancellationToken = default)
    {
        string url = GetFullUrl(relativePath);
        Uri requestUri = new Uri(url, UriKind.Absolute);
        using var _ = await ExecuteRequestAsync(
            client => client.PatchAsJsonAsync(requestUri, requestData),
            requestUri,
            cancellationToken).ConfigureAwait(false);
        Logger.LogDebug("Successfully executed PATCH request to {ApiUrl}", requestUri);
    }

}