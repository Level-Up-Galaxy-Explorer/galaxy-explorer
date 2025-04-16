using Spectre.Console;
using Spectre.Console.Cli;
using galaxy_cli.Services;
using galaxy_cli.DTO.Planets;

namespace galaxy_cli.Commands.PlanetCommands;

public class PlanetUpdateCommand : AsyncCommand
{
    private readonly IPlanetService _planetService;

    public PlanetUpdateCommand(IPlanetService planetService)
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

        var selectedPlanet = AnsiConsole.Prompt(
            new SelectionPrompt<PlanetDTO>()
                .Title("[yellow]Select a planet to update:[/]")
                .PageSize(10)
                .AddChoices(planets)
                .UseConverter(planet => planet.Name)
        );

        AnsiConsole.MarkupLine($"\n[bold yellow]Updating details for {selectedPlanet.Name}:[/]");

        var newName = AnsiConsole.Prompt(
            new TextPrompt<string>("[green]Enter new name (leave blank to keep current):[/]")
                .AllowEmpty()
        );
        var newType = AnsiConsole.Prompt(
            new TextPrompt<string>("[green]Enter new type (leave blank to keep current):[/]")
                .AllowEmpty()
        );
        var newGalaxy = AnsiConsole.Prompt(
            new TextPrompt<string>("[green]Enter new galaxy (leave blank to keep current):[/]")
                .AllowEmpty()
        );

        var hasLifeInput = AnsiConsole.Prompt(
            new TextPrompt<string>($"[green]Does this planet have life? (current: {(selectedPlanet.HasLife ? "yes" : "no")}) (yes/no, leave blank to keep current):[/]")
                .AllowEmpty() 
                .Validate(input =>
                {
                    if (string.IsNullOrWhiteSpace(input) || input.Trim().ToLower() == "yes" || input.Trim().ToLower() == "no")
                    {
                        return ValidationResult.Success();
                    }
                    return ValidationResult.Error("[red]Please enter 'yes', 'no', or leave blank to keep the current value.[/]");
                })
        );

        bool? hasLife = null;
        if (!string.IsNullOrWhiteSpace(hasLifeInput))
        {
            hasLife = hasLifeInput.Trim().ToLower() == "yes";
        }

        var newCoordinates = AnsiConsole.Prompt(
            new TextPrompt<string>("[green]Enter new coordinates (e.g., X:10,Y:20,Z:30) (leave blank to keep current):[/]")
                .AllowEmpty()
        );

        var updatedPlanet = new PlanetDTO
        {
            Name = string.IsNullOrWhiteSpace(newName) ? selectedPlanet.Name : newName,
            PlanetType = string.IsNullOrWhiteSpace(newType) ? selectedPlanet.PlanetType : newType,
            Galaxy = string.IsNullOrWhiteSpace(newGalaxy) ? selectedPlanet.Galaxy : newGalaxy,
            HasLife = hasLife ?? selectedPlanet.HasLife, 
            Coordinates = string.IsNullOrWhiteSpace(newCoordinates) ? selectedPlanet.Coordinates : newCoordinates
        };

        if (updatedPlanet.Name == selectedPlanet.Name &&
            updatedPlanet.PlanetType == selectedPlanet.PlanetType &&
            updatedPlanet.Galaxy == selectedPlanet.Galaxy &&
            updatedPlanet.HasLife == selectedPlanet.HasLife &&
            updatedPlanet.Coordinates == selectedPlanet.Coordinates)
        {
            AnsiConsole.MarkupLine("[yellow]No changes were made to the planet.[/]");
            return 0;
        }

        var success = await _planetService.UpdatePlanetAsync(selectedPlanet.Id, updatedPlanet);

        if (success)
        {
            AnsiConsole.MarkupLine("[green]Planet updated successfully![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to update the planet.[/]");
        }

        return 0;
    }
}