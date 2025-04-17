using System.ComponentModel;
using galaxy_cli.DTO.Galaxy;
using galaxy_cli.Services;
using galaxy_cli.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.GalaxyCommands;

[Description("Updates an existing galaxy.")]
public class GalaxyUpdateCommand : AsyncCommand<IdSettings>
{
    private readonly IGalaxyService _galaxyService;

    public GalaxyUpdateCommand(IGalaxyService galaxyService)
    {
        _galaxyService = galaxyService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, IdSettings settings)
    {
        var galaxyId = settings.Id != 0 
            ? settings.Id 
            : AnsiConsole.Ask<int>("Enter the [green]galaxy ID[/] to update:");

        var existingGalaxy = await _galaxyService.GetGalaxyByIdAsync(galaxyId);
        if (existingGalaxy == null)
        {
            AnsiConsole.MarkupLine($"[red]Galaxy with ID {galaxyId} not found.[/]");
            return -1;
        }

        var galaxyTypes = await _galaxyService.GetGalaxyTypesAsync();
        if (!galaxyTypes.Any())
        {
            AnsiConsole.MarkupLine("[red]Could not fetch galaxy types.[/]");
            return -1;
        }

        var galaxy = new GalaxyDTO
        {
            Id = galaxyId,
            Name = AnsiConsole.Ask("Enter [green]galaxy name[/]:", existingGalaxy.Name),
            GalaxyType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select [green]galaxy type[/]:")
                    .AddChoices(galaxyTypes)
                    .HighlightStyle(new Style(foreground: Color.Green))
                    .UseConverter(x => x == existingGalaxy.GalaxyType ? $"[green]{x}[/]" : x)),
            Distance_From_Earth = AnsiConsole.Ask("Enter [green]distance from Earth[/] (light years):", existingGalaxy.Distance_From_Earth),
            Description = AnsiConsole.Ask("Enter [green]description[/]:", existingGalaxy.Description)
        };

        var success = await _galaxyService.UpdateGalaxyAsync(galaxyId, galaxy);
        return success ? 0 : -1;
    }
}