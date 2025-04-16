using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using galaxy_api.Models;

namespace galaxy_api.Services;

class AuthorizationService : IAuthorizationService
{
    private readonly IUserService _userService;
    private readonly IRankService _rankService;

    public AuthorizationService(IUserService userService, IRankService rankService)
    {
        _userService = userService;
        _rankService = rankService;
    }
    public async Task<bool> allowRequest(HttpContext httpContext, int[] allowedRanks)
    {
        Claim nameIdentifierClaim = httpContext.User.Claims.Where<Claim>(e => e.Type == ClaimTypes.NameIdentifier).First();
        Users user = await _userService.GetUserByGoogleIdAsync(nameIdentifierClaim.Value);
        Rank? userRank = await _rankService.GetRankByIdAsync(user.Rank_Id);
        if (userRank != null)
        {
            foreach (int rank in allowedRanks)
            {
                if (userRank.Rank_Id == rank)
                {
                    return true;
                }
            }
        }
        return false;
    }


    public async Task<bool> denyRequest(HttpContext httpContext, int[] allowedRanks)
    {
        return !(await allowRequest(httpContext, allowedRanks));
    }

}