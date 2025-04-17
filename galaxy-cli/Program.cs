using System.Reflection;
using galaxy_cli.Commands;
using galaxy_cli.Commands.CrewCommands;
using galaxy_cli.Commands.MissionCommands;
using galaxy_cli.Commands.PlanetCommands;
using galaxy_cli.Commands.UserCommands;
using galaxy_cli.DI;
using galaxy_cli.Runner;
using galaxy_cli.Services;
using galaxy_cli.Services.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console.Cli;

namespace galaxy_cli;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var configuration = BuildConfiguration();
        var services = ConfigureServices(configuration);

        var app = RegisterCommands(services);

        if (args.Length > 0)
        {
            return await app.RunAsync(args);
        }
        else
        {
            var interactiveRunner = new InteractiveAppRunner(app);
            await interactiveRunner.RunAsync();
            return 0;
        }
    }
    private static IConfiguration BuildConfiguration()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables().AddCommandLine([]);

        var baseSettingsStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("galaxy_cli.appsettings.json");
        var envSettingsStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"galaxy_cli.appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development"}.json");

        if (baseSettingsStream != null) builder.AddJsonStream(baseSettingsStream);
        if (envSettingsStream != null) builder.AddJsonStream(envSettingsStream);

        return builder.Build();
    }

    private static ServiceCollection ConfigureServices(IConfiguration configuration)
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddLogging(builder =>
            {
                // builder.ClearProviders();
                // builder.AddConfiguration(configuration.GetSection("Logging"));
                // builder.AddConsole();
                // builder.SetMinimumLevel(LogLevel.None);
                // // #if DEBUG
                // //     builder.SetMinimumLevel(LogLevel.Debug);
                // // #else
                // //     builder.SetMinimumLevel(LogLevel.Information);
                // // #endif
            });

        serviceCollection.AddHttpClient();

        serviceCollection.AddHttpClient("BaseApiService")
        .AddHttpMessageHandler<BearerTokenHandler>();

        serviceCollection.AddSingleton<IConfiguration>(configuration);

        serviceCollection.AddOptions<ApiSettings>();

        serviceCollection.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));
        serviceCollection.AddSingleton<BearerTokenHandler>();

        serviceCollection.AddSingleton<IAuthService, GoogleAuthService>();
        serviceCollection.AddSingleton<IBackendAuthService, BackendAuthService>();
        serviceCollection.AddSingleton<ITokenStore, CredentialManagerTokenStore>();
        serviceCollection.AddSingleton<ICrewsService, CrewsService>();
        serviceCollection.AddSingleton<IPlanetService, PlanetService>();
        serviceCollection.AddSingleton<IMissionService, MissionService>();
        serviceCollection.AddSingleton<IUserService, UserService>();

        return serviceCollection;
    }

    private static CommandApp RegisterCommands(ServiceCollection services)
    {

        var registrar = new TypeRegistrar(services);

        var app = new CommandApp(registrar);

        app.Configure(config =>
        {

            config.AddCommand<LoginCommand>("login");
            config.AddCommand<StatusCommand>("status");

            config.AddBranch("scan", scan =>
            {
                scan.SetDescription("Perform scans.");
                scan.AddCommand<ScanGalaxyCommand>("galaxy");
            });

            config.AddBranch("crew", crew =>
            {
                crew.SetDescription("Manage your crew.");
                crew.AddCommand<CrewListCommand>("list");
                crew.AddCommand<CrewDetailCommand>("details");
                crew.AddCommand<CrewAssignCommand>("assign");
                crew.AddCommand<CrewCreateCommand>("create");
                crew.AddCommand<CrewFireCommand>("fire");
                crew.AddCommand<CrewMissionHistoryCommand>("history");

            });

            config.AddBranch("missions", mission =>
            {
                mission.SetDescription("View and manage missions.");
                mission.AddCommand<MissionListCommand>("list-all");
                mission.AddCommand<MissionCreateCommand>("create");
                mission.AddCommand<MissionUpdateCommand>("update");
                mission.AddCommand<MissionsAssignCommand>("assign");
                mission.AddCommand<MissionUpdateStatusCommand>("update-status");
                mission.AddCommand<MissionReportCommand>("report");
                mission.AddCommand<MissionGetByIdCommand>("list");
                
            });

            config.AddBranch("planets", mission =>
            {
                mission.SetDescription("View and manage planets.");
                mission.AddCommand<PlanetListCommand>("list");
            });

            config.AddBranch("users", users =>
            {
                users.SetDescription("View and manage users.");
                users.AddCommand<UserListCommand>("list-all");
                users.AddCommand<UserGetByIdCommand>("list");
                users.AddCommand<UserDeactivateCommand>("deactivate");
                users.AddCommand<UserAssignRankCommand>("assign");
            });

        });

        return app;

    }

}
