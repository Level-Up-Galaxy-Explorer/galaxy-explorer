using System.ComponentModel;
using System.Text.Json;
using galaxy_cli.DTO;
using galaxy_cli.Services;
using galaxy_cli.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.GalaxyCommands;

[Description("Displays galaxies")]
public class GalaxyListCommand : AsyncCommand<EmptyCommandSetting>
{
    private readonly IGalaxyService _galaxyService;

    public GalaxyListCommand(IGalaxyService galaxyService)
    {
        _galaxyService = galaxyService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, EmptyCommandSetting settings)
    {

        IEnumerable<GalaxyDTO>? galaxyItems = [];

        await AnsiConsole.Status()
        .StartAsync("Calling API...", async ctx =>
        {
            ctx.Status("Processing...");
            galaxyItems = await _galaxyService.GetAllGalaxyAsync();
            AnsiConsole.MarkupLine($"[green]API call successful[/]");
        });



        if (galaxyItems.Any())
        {

            var galaxy = AnsiConsole.Prompt(
                new SelectionPrompt<GalaxyDTO>()
                    .Title("Choose a galaxy:")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more galaxy)[/]")
                    .AddChoices(galaxyItems).UseConverter(s => s.Name));

            var grid = ConsoleUiHelper.BuildGalaxyGrid(galaxy);
            AnsiConsole.Write(
                new Panel(grid)
                    .Header("Galaxy Information"));
        }

        return 0;
    }
}