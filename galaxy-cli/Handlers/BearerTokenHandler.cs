using System.Net.Http.Headers;
using galaxy_cli.Services;

public class BearerTokenHandler : DelegatingHandler
{
    private readonly ITokenStore _tokenStore;

    public BearerTokenHandler(ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? bearerToken = await _tokenStore.GetTokenAsync();
        if (!string.IsNullOrEmpty(bearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}