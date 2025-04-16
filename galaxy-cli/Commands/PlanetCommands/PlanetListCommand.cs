using System.ComponentModel;
using galaxy_api.DTOs;
using galaxy_cli.Commands.Base;
using galaxy_cli.Services;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.PlanetCommands;

[Description("Displays planets")]
public class PlanetListCommand : BaseApiCommand<EmptyCommandSettings>
{
    private readonly IPlanetService _planetService;

    public PlanetListCommand(IPlanetService planetService, ILogger<PlanetListCommand> logger) : base(logger)
    {
        _planetService = planetService;
    }
    protected override async Task<int> ExecuteApiLogic(CommandContext context, EmptyCommandSettings settings)
    {

        IEnumerable<PlanetDTO> planets = [];
        await AnsiConsole.Status()
        .StartAsync("Calling API...", async ctx =>
        {
            ctx.Status("Processing...");
            planets = _planetService.GetPlanetsAsync().GetAwaiter().GetResult();
            AnsiConsole.MarkupLine($"[green]API call successful[/]");
        });

        var table = new Table();

        table.AddColumns("Name", "Type", "Galaxy");

        if (planets.Any())
        {
            AnsiConsole.MarkupLine($"\n[blue]Planets:[/]");
            foreach (var planet in planets)
            {
                table.AddRow(planet.Name, planet.PlanetType, planet.Galaxy);
            }
            AnsiConsole.Write(table);
        }
        else
        {
            Console.WriteLine("No planets found.");
        }
        return 0;
    }
}