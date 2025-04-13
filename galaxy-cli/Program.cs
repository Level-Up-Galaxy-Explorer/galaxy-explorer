using galaxy_cli.Commands;
using galaxy_cli.Commands.CrewCommands;
using galaxy_cli.Commands.MissionCommands;
using galaxy_cli.DI;
using galaxy_cli.Runner;
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


    private static ServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddHttpClient();

        return serviceCollection.BuildServiceProvider();
    }

    private static CommandApp RegisterCommands(ServiceProvider serviceProvider)
    {
        var services = new ServiceCollection();

        services.AddHttpClient();

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

        });

        return app;

    }

}
