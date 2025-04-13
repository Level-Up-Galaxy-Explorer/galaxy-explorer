using galaxy_api.Repositories;
using galaxy_api.Services;

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


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
