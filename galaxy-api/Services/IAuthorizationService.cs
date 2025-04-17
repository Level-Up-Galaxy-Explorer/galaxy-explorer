using galaxy_api.Models;

namespace galaxy_api.Services;

public interface IAuthorizationService
{
    public Task<bool> AllowRequest(HttpContext httpContext, int[] allowedRanks);
    public Task<bool> DenyRequest(HttpContext httpContext, int[] allowedRanks);
}