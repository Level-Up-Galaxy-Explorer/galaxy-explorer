using System.ComponentModel;
using galaxy_cli.DTO.Galaxy;
using galaxy_cli.Services;
using galaxy_cli.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.GalaxyCommands;

[Description("Displays details for a specific galaxy by ID.")]
public class GalaxyGetByIdCommand : AsyncCommand<IdSettings>
{
    private readonly IGalaxyService _galaxyService;

    public GalaxyGetByIdCommand(IGalaxyService galaxyService)
    {
        _galaxyService = galaxyService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, IdSettings settings)
    {
        var galaxyId = settings.Id != 0 
            ? settings.Id 
            : AnsiConsole.Ask<int>("Enter the [green]galaxy ID[/] to view:");

        var galaxy = await _galaxyService.GetGalaxyByIdAsync(galaxyId);

        if (galaxy == null)
        {
            AnsiConsole.MarkupLine($"[red]Galaxy with ID {galaxyId} not found.[/]");
            return -1;
        }

        var table = new Table();
        table.AddColumns("Field", "Value");
        table.AddRow("ID", galaxy.Id.ToString());
        table.AddRow("Name", galaxy.Name);
        table.AddRow("Type", galaxy.GalaxyType);
        table.AddRow("Distance From Earth", $"{galaxy.Distance_From_Earth:N0} KM");
        table.AddRow("Description", galaxy.Description);

        AnsiConsole.Write(table);
        return 0;
    }
}