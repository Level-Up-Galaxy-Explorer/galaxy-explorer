using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

public interface IGoogleAuthProvider
{
    Task<string?> ExchangeCodeForIdTokenAsync(string code, string codeVerifier, string redirectUri);
    Task<IEnumerable<SecurityKey>?> GetGooglePublicKeysAsync();
    Task<ClaimsPrincipal?> ValidateGoogleIdTokenAsync(string idToken);

    TokenValidationParameters GetTokenValidationParameters();

    public void addJwtBearerOptions(JwtBearerOptions options);

}