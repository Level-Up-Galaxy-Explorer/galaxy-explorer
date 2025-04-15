﻿using System.Reflection;
using galaxy_cli.Commands;
using galaxy_cli.Commands.CrewCommands;
using galaxy_cli.Commands.MissionCommands;
using galaxy_cli.Commands.PlanetCommands;
using galaxy_cli.DI;
using galaxy_cli.Runner;
using galaxy_cli.Services;
using galaxy_cli.Services.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace galaxy_cli;

class Program
{
    static async Task<int> Main(string[] args)
    {

        var services = ConfigureServices();

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

    private static ServiceCollection ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables().AddCommandLine([]);

        var baseSettingsStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("galaxy_cli.appsettings.json");
        var envSettingsStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"galaxy_cli.appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development"}.json");

        if (baseSettingsStream != null) configurationBuilder.AddJsonStream(baseSettingsStream);
        if (envSettingsStream != null) configurationBuilder.AddJsonStream(envSettingsStream);

        IConfiguration configuration = configurationBuilder.Build();

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
        serviceCollection.AddSingleton<IPlanetService, PlanetService>();

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
                crew.SetDescription("Manage your ship crew.");
                crew.AddCommand<CrewListCommand>("list");
                crew.AddCommand<CrewDetailCommand>("details");
                crew.AddCommand<CrewAssignCommand>("assign");
                crew.AddCommand<CrewCreateCommand>("create");
            });

            config.AddBranch("missions", mission =>
            {
                mission.SetDescription("View and manage missions.");
                mission.AddCommand<MissionListCommand>("list");
                mission.AddCommand<MissionDetailCommand>("detail");
                mission.AddCommand<MissionAcceptCommand>("accept");
                mission.AddCommand<MissionAbortCommand>("abort");
                mission.AddCommand<MissionLaunchCommand>("launch");
                mission.AddCommand<MissionsAssignCommand>("assign");
            });

            config.AddBranch("planets", mission =>
            {
                mission.SetDescription("View and manage planets.");
                mission.AddCommand<PlanetListCommand>("list");
            });

        });

        return app;

    }

}
