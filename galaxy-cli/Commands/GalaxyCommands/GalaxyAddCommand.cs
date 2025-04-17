using System.ComponentModel;
using galaxy_cli.DTO.Galaxy;
using galaxy_cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.GalaxyCommands;

[Description("Adds a new galaxy.")]
public class GalaxyAddCommand : AsyncCommand
{
    private readonly IGalaxyService _galaxyService;

    public GalaxyAddCommand(IGalaxyService galaxyService)
    {
        _galaxyService = galaxyService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var galaxy = new GalaxyDTO
        {
            Name = AnsiConsole.Ask<string>("Enter [green]galaxy name[/]:"),
            GalaxyType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select [green]galaxy type[/]:")
                    .AddChoices(new[] { "Spiral", "Elliptical", "Irregular", "Lenticular" })),
            Distance_From_Earth = AnsiConsole.Ask<double>("Enter [green]distance from Earth[/] (light years):"),
            Description = AnsiConsole.Ask<string>("Enter [green]description[/]:")
        };

        var success = await _galaxyService.AddGalaxyAsync(galaxy);
        return success ? 0 : -1;
    }
}