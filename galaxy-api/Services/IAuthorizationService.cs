using galaxy_api.Models;

namespace galaxy_api.Services;

public interface IAuthorizationService
{
    public Task<bool> allowRequest(HttpContext httpContext, int[] allowedRanks);
    public Task<bool> denyRequest(HttpContext httpContext, int[] allowedRanks);
}