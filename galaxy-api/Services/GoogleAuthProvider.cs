using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication.JwtBearer;

public class GoogleAuthProvider : IGoogleAuthProvider
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<GoogleAuthProvider> _logger;
    private const string GooglePublicKeysCacheKey = "GooglePublicKeys";

    public GoogleAuthProvider(IConfiguration configuration, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ILogger<GoogleAuthProvider> logger)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<string?> ExchangeCodeForIdTokenAsync(string code, string codeVerifier, string redirectUri)
    {
        var googleTokenEndpoint = "https://oauth2.googleapis.com/token";
        var tokenRequestParameters = new Dictionary<string, string>
        {
            {"client_id", _configuration["Authentication:Google:ClientId"]??""},
            {"client_secret", _configuration["Authentication:Google:ClientSecret"]??""},
            {"code", code},
            {"grant_type", "authorization_code"},
            {"redirect_uri", redirectUri},
            {"code_verifier", codeVerifier}
        };

        var tokenRequestContent = new FormUrlEncodedContent(tokenRequestParameters);
        var tokenResponse = await _httpClient.PostAsync(googleTokenEndpoint, tokenRequestContent);

        if (!tokenResponse.IsSuccessStatusCode)
        {
            var error = await tokenResponse.Content.ReadAsStringAsync();
            _logger.LogError($"Failed to exchange code with Google: {error}");
            return null;
        }

        var tokenResponseJson = await tokenResponse.Content.ReadFromJsonAsync<JsonDocument>();
        if (tokenResponseJson == null || !tokenResponseJson.RootElement.TryGetProperty("id_token", out var idTokenElement))
        {
            _logger.LogError("Google token response did not contain an ID token.");
            return null;
        }

        return idTokenElement.GetString();
    }

    public async Task<IEnumerable<SecurityKey>?> GetGooglePublicKeysAsync()
    {
        if (_memoryCache.TryGetValue<IEnumerable<SecurityKey>>(GooglePublicKeysCacheKey, out IEnumerable<SecurityKey>? cachedKeys))
        {
            return cachedKeys;
        }

        var client = new HttpClient();
        var response = await client.GetAsync("https://www.googleapis.com/oauth2/v3/certs");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        var keys = document.RootElement.GetProperty("keys").EnumerateArray();
        var signingKeys = new List<SecurityKey>();
        foreach (var key in keys)
        {
            if (key.TryGetProperty("kty", out var kty) && kty.GetString() == "RSA" &&
                key.TryGetProperty("n", out var n) && key.TryGetProperty("e", out var e))
            {
                var rsa = new System.Security.Cryptography.RSACryptoServiceProvider();
                rsa.ImportParameters(new System.Security.Cryptography.RSAParameters
                {
                    Modulus = Base64UrlEncoder.DecodeBytes(n.GetString()),
                    Exponent = Base64UrlEncoder.DecodeBytes(e.GetString())
                });
                signingKeys.Add(new RsaSecurityKey(rsa));
            }
        }

        var cacheEntryOptions = new MemoryCacheEntryOptions();

        if (response.Headers.CacheControl != null && response.Headers.CacheControl.MaxAge != null)
        {
            cacheEntryOptions.SetAbsoluteExpiration(response.Headers.CacheControl.MaxAge.Value);
        }
        else
        {
            cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromHours(24));
        }
        _memoryCache.Set(GooglePublicKeysCacheKey, signingKeys, cacheEntryOptions);

        return signingKeys;
    }

    public async Task<ClaimsPrincipal?> ValidateGoogleIdTokenAsync(string idToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var parameters = GetTokenValidationParameters();

            var principal = tokenHandler.ValidateToken(idToken, parameters, out SecurityToken validatedToken);
            return principal;
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogError($"Invalid Google ID token: {ex.Message}");
            return null;
        }
    }

    public TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://accounts.google.com",
            ValidateAudience = true,
            ValidAudience = _configuration["Authentication:Google:ClientId"],
            ValidateLifetime = true,
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                return GetGooglePublicKeysAsync().GetAwaiter().GetResult();
            }
        };
    }

    public void addJwtBearerOptions(JwtBearerOptions options)
    {

        options.Authority = "https://accounts.google.com";
        options.Audience = _configuration["Authentication:Google:ClientId"];
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = GetTokenValidationParameters();

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier) ?? context.Principal?.FindFirst("sub");
                if (userIdClaim != null)
                {
                    // add custom roles to principal claims?
                }
            }
        };
    }

}