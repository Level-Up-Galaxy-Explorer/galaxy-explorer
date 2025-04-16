using galaxy_api.Repositories;
using galaxy_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddScoped<IRankRepository, RankRepository>();
builder.Services.AddScoped<IRankService, RankService>();

builder.Services.AddSingleton<IGoogleAuthProvider, GoogleAuthProvider>();

builder.Services.AddScoped<galaxy_api.Services.IAuthorizationService, AuthorizationService>();

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
