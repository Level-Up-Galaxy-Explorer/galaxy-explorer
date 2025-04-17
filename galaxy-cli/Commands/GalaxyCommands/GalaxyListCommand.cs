using galaxy_cli.DTO.Galaxy;
using galaxy_cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.GalaxyCommands;
public class GalaxyListCommand : AsyncCommand
{
    private readonly IGalaxyService _galaxyService;

    public GalaxyListCommand(IGalaxyService galaxyService)
    {
        _galaxyService = galaxyService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var galaxies = await _galaxyService.GetAllGalaxyAsync();
    
        if (!galaxies.Any())
        {
            AnsiConsole.MarkupLine("[red]No galaxies found.[/]");
            return 0;
        }

        // Display table of galaxies
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumns("[bold]Name[/]", "[bold]Type[/]", "[bold]Distance From Earth[/]");

        foreach (var galaxy in galaxies)
        {
            table.AddRow(
                $"[green]{galaxy.Name}[/]",
                $"[yellow]{galaxy.GalaxyType}[/]",
                $"[grey]{galaxy.Distance_From_Earth:N0} KM[/]"  // Format with thousands separator and add KM
            );
        }

        AnsiConsole.Write(table);

        var selectedGalaxy = AnsiConsole.Prompt(
            new SelectionPrompt<GalaxyDTO>()
                .Title("[yellow]Select a galaxy to view more details:[/]")
                .PageSize(10)
                .AddChoices(galaxies)
                .UseConverter(galaxy => galaxy.Name)
        );

        AnsiConsole.MarkupLine($"\n[bold yellow]Details for {selectedGalaxy.Name}:[/]");
        AnsiConsole.MarkupLine($"  [green]Type:[/] {selectedGalaxy.GalaxyType}");
        AnsiConsole.MarkupLine($"  [green]Distance From Earth:[/] {selectedGalaxy.Distance_From_Earth:N0} KM");
        AnsiConsole.MarkupLine($"  [green]Description:[/] {selectedGalaxy.Description}");

        return 0;
    }
}