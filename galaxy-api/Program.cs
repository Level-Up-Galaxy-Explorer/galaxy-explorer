using galaxy_api.Repositories;
using galaxy_api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IPlanetRepository, PlanetRepository>();
builder.Services.AddScoped<IPlanetService, PlanetService>();


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
