using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace galaxy_api.Authentication;
public interface IGoogleAuthProvider
{
    Task<string?> ExchangeCodeForIdTokenAsync(string code, string codeVerifier, string redirectUri);
    Task<IEnumerable<SecurityKey>?> GetGooglePublicKeysAsync();
    ClaimsPrincipal? ValidateGoogleIdToken(string idToken);

    TokenValidationParameters GetTokenValidationParameters();

    public void AddJwtBearerOptions(JwtBearerOptions options);

}