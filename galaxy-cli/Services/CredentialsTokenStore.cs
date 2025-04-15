using GitCredentialManager;

namespace galaxy_cli.Services;
public class CredentialManagerTokenStore : ITokenStore
{
    private readonly string _serviceName = "https://galaxy-explorer-cli.com";
    private readonly string _accountName = "explorer";
    private readonly ICredentialStore _store;
    private string? _token;

    public CredentialManagerTokenStore()
    {
        _store = CredentialManager.Create("galaxy-explorer-cli");
    }

    public Task<string?> GetTokenAsync()
    {
        if (_token == null)
        {
            ICredential credential = _store.Get(_serviceName, _accountName);
            _token = credential?.Password;

        }
        return Task.FromResult(_token);
    }

    public Task SaveTokenAsync(string token)
    {
        _store.AddOrUpdate(_serviceName, _accountName, token);
        _token = token;
        return Task.CompletedTask;
    }

    public Task ClearTokenAsync()
    {
        _store.Remove(_serviceName, _accountName);
        _token = null;
        return Task.CompletedTask;
    }
}