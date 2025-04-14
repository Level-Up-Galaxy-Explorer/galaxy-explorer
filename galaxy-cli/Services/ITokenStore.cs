namespace galaxy_cli.Services;

public interface ITokenStore
{
    Task<string?> GetTokenAsync();
    Task SaveTokenAsync(string token);
    Task ClearTokenAsync();
}