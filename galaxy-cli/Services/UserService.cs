
using galaxy_cli.Models;
using galaxy_cli.Services.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace galaxy_cli.Services;

public class UserService : BaseApiService, IUserService
{
    public UserService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, ILogger<UserService> logger) 
    : base(httpClientFactory, apiSettings, logger)
    {
    }

    protected override string EndpointPath => "user";

    public async Task<List<Users>> GetAllUsersAsync()
    {
        return await GetAndDeserializeAsync<List<Users>>();
    }
}