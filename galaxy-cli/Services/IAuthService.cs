using System.Data;
namespace galaxy_cli.Services;
public interface IAuthService
{
    public Task<String?> authenticate(IDictionary<string, string>? options);
}