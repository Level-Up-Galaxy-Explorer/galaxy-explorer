using System.ComponentModel;
using galaxy_cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.PlanetCommands;

[Description("Displays planets")]
public class PlanetListCommand : Command<EmptyCommandSettings>
{
    private readonly IPlanetService _planetService;

    public PlanetListCommand(IPlanetService planetService)
    {
        _planetService = planetService;
    }
    public override int Execute(CommandContext context, EmptyCommandSettings settings)
    {
        var planets = _planetService.GetPlanetsAsync().GetAwaiter().GetResult();
        var table = new Spectre.Console.Table();

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