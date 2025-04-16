using Spectre.Console;
using Spectre.Console.Cli;
using galaxy_cli.Services;
using galaxy_cli.DTO.Planets;

namespace galaxy_cli.Commands.PlanetCommands;

public class PlanetAddCommand : AsyncCommand
{
    private readonly IPlanetService _planetService;

    public PlanetAddCommand(IPlanetService planetService)
    {
        _planetService = planetService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        AnsiConsole.MarkupLine("[bold yellow]Adding a new planet:[/]");

        var name = AnsiConsole.Prompt(
            new TextPrompt<string>("[green]Enter planet name:[/]")
                .Validate(input =>
                {
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        return ValidationResult.Error("[red]Planet name cannot be empty.[/]");
                    }
                    return ValidationResult.Success();
                })
        );

        var planetTypes = await _planetService.GetPlanetTypesAsync();
        if (!planetTypes.Any())
        {
            AnsiConsole.MarkupLine("[red]No planet types found in the database.[/]");
            return 1;
        }

        var type = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Select planet type:[/]")
                .AddChoices(planetTypes)
        );

        AnsiConsole.MarkupLine($"[yellow]Planet type:[/] [bold green]{type}[/]");

        var galaxies = await _planetService.GetGalaxiesAsync();
        if (!galaxies.Any())
        {
            AnsiConsole.MarkupLine("[red]No galaxies found in the database.[/]");
            return 1;
        }

        var galaxy = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Select galaxy:[/]")
                .AddChoices(galaxies)
        );

        AnsiConsole.MarkupLine($"[yellow]Galaxy:[/] [bold green]{galaxy}[/]");

        var hasLife = AnsiConsole.Prompt(
            new ConfirmationPrompt("[green]Does this planet have life? (y/n):[/]")
        );

        var coordinates = AnsiConsole.Prompt(
            new TextPrompt<string>("[green]Enter coordinates (e.g., X:10,Y:20,Z:30):[/]")
                .Validate(input =>
                {
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        return ValidationResult.Error("[red]Coordinates cannot be empty.[/]");
                    }
                    return ValidationResult.Success();
                })
        );

        var newPlanet = new PlanetDTO
        {
            Name = name,
            PlanetType = type,
            Galaxy = galaxy,
            HasLife = hasLife,
            Coordinates = coordinates
        };
        
        var addedPlanet = await _planetService.AddPlanetAsync(newPlanet);

        if (addedPlanet != null)
        {
            AnsiConsole.MarkupLine("[green]Planet added successfully![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to add the planet.[/]");
        }

        return 0;
    }
}