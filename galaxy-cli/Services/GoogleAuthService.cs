using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace galaxy_cli.Services;

public sealed class GoogleAuthService : IAuthService
{

    private string? _googleClientId;
    private static string Base64UrlEncode(byte[] data)
    {
        return Convert.ToBase64String(data)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }
    private (string verifier, string challenge) GeneratePkceCodes()
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[32];
        rng.GetBytes(randomBytes);
        var verifier = Base64UrlEncode(randomBytes);

        var buffer = Encoding.UTF8.GetBytes(verifier);
        var hash = SHA256.Create().ComputeHash(buffer);
        var challenge = Base64UrlEncode(hash);

        return (verifier, challenge);
    }

    private string ConstructAuthUrl(string challenge, string redirectUri, string clientId)
    {
        return "https://accounts.google.com/o/oauth2/v2/auth?" +
               $"client_id={_googleClientId}&" +
               $"response_type=code&" +
               $"scope=openid profile email&" +
               $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
               $"prompt=select_account&" +
               $"code_challenge={challenge}&" +
               $"code_challenge_method=S256";
    }

    private async Task<string?> ListenForAuthCodeAsync(string redirectUri)
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add(redirectUri);
        try
        {
            listener.Start();
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            string? authorizationCode = request.QueryString.Get("code");

            string responseString = "<HTML><BODY>Authentication successful! You can return to the console.</BODY></HTML>";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            HttpListenerResponse response = context.Response;
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer);
            response.OutputStream.Close();
            return authorizationCode;
        }
        catch (HttpListenerException ex)
        {
            Console.WriteLine($"Error starting or listening on {redirectUri}: {ex.Message}");
            return null;
        }
        finally
        {
            listener.Stop();
            listener.Close();
        }
    }

    private int GetAvailablePort()
    {
        TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private readonly IBackendAuthService _backendAuthService;
    private readonly IConfiguration _configuration;

    public GoogleAuthService(IBackendAuthService backendAuthService, IConfiguration configuration)
    {
        _backendAuthService = backendAuthService;
        _configuration = configuration;
        _googleClientId = _configuration["Authentication:Google:ClientId"];
    }

    public async Task<String?> authenticate(IDictionary<string, string>? options)
    {
        var (verifier, challenge) = GeneratePkceCodes();
        int port = GetAvailablePort();
        string redirectUri = $"http://localhost:{port}/callback/";
        string googleAuthUrl = ConstructAuthUrl(challenge, redirectUri, _googleClientId ?? "");

        Process.Start(new ProcessStartInfo(googleAuthUrl) { UseShellExecute = true });

        string? authorizationCode = await ListenForAuthCodeAsync(redirectUri);

        if (!string.IsNullOrEmpty(authorizationCode))
        {
            string? jwtToken = await _backendAuthService.ExchangeCodeWithBackendAsync(authorizationCode, verifier, redirectUri);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                return jwtToken;
            }
            else
            {
                Console.WriteLine("Login failed to retrieve JWT.");
                return null;
            }
        }
        else
        {
            Console.WriteLine("Login failed or code not received.");
            return null;
        }
    }

}