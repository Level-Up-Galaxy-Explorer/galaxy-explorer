
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace galaxy_api.Authentication;

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly IGoogleAuthProvider _googleAuthProvider;

    public ConfigureJwtBearerOptions(IGoogleAuthProvider googleAuthProvider)
    {
        _googleAuthProvider = googleAuthProvider;
    }

    public void Configure(JwtBearerOptions options)
    {
        _googleAuthProvider.AddJwtBearerOptions(options);

        options.SaveToken = true;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}