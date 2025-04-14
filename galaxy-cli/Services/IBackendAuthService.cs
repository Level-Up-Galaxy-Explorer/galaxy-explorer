namespace galaxy_cli.Services;
public interface IBackendAuthService
{
    Task<string?> ExchangeCodeWithBackendAsync(string code, string codeVerifier, string redirectUri);
}