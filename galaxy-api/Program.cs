using galaxy_api.Repositories;
using galaxy_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IPlanetRepository, PlanetRepository>();
builder.Services.AddScoped<IPlanetService, PlanetService>();

builder.Services.AddScoped<IGalaxyRepository, GalaxyRepository>();
builder.Services.AddScoped<IGalaxyService, GalaxyService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IMissionRepository, MissionRepository>();
builder.Services.AddScoped<IMissionService, MissionService>();

builder.Services.AddScoped<ICrewsRepositoty, CrewsRepository>();
builder.Services.AddScoped<ICrewsService, CrewsService>();

builder.Services.AddSingleton<IGoogleAuthProvider, GoogleAuthProvider>();

builder.Services.AddHealthChecks()
       .AddCheck<DatabaseHealthCheck>("Database");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var serviceProvider = builder.Services.BuildServiceProvider();
    var googleAuthProvider = serviceProvider.GetRequiredService<IGoogleAuthProvider>();
    googleAuthProvider.addJwtBearerOptions(options);
});

builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
