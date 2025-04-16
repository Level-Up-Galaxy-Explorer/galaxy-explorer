using Spectre.Console;
using Spectre.Console.Cli;
using galaxy_cli.Services;
using galaxy_cli.DTO.Planets;

namespace galaxy_cli.Commands.PlanetCommands;

public class PlanetListCommand : AsyncCommand
{
    private readonly IPlanetService _planetService;

    public PlanetListCommand(IPlanetService planetService, ILogger<PlanetListCommand> logger) : base(logger)
    {
        _planetService = planetService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var planets = await _planetService.GetAllPlanetsAsync();

        if (!planets.Any())
        {
            AnsiConsole.MarkupLine("[red]No planets found.[/]");
            return 0;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumns("[bold]Name[/]", "[bold]Type[/]", "[bold]Galaxy[/]");

        foreach (var planet in planets)
        {
            table.AddRow(
                $"[green]{planet.Name}[/]",
                $"[yellow]{planet.PlanetType}[/]",
                $"[grey]{planet.Galaxy}[/]"
            );
        }

        AnsiConsole.Write(table);

        var selectedPlanet = AnsiConsole.Prompt(
            new SelectionPrompt<PlanetDTO>()
                .Title("[yellow]Select a planet to view more details:[/]")
                .PageSize(10)
                .AddChoices(planets)
                .UseConverter(planet => planet.Name) 
        );

        AnsiConsole.MarkupLine($"\n[bold yellow]Details for {selectedPlanet.Name}:[/]");
        AnsiConsole.MarkupLine($"  [green]Type:[/] {selectedPlanet.PlanetType}");
        AnsiConsole.MarkupLine($"  [green]Galaxy:[/] {selectedPlanet.Galaxy}");
        AnsiConsole.MarkupLine($"  [green]Has Life:[/] {selectedPlanet.HasLife}");
        AnsiConsole.MarkupLine($"  [green]Coordinates:[/] {selectedPlanet.Coordinates}");

        return 0;
    }
}