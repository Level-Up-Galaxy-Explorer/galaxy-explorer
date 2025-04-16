using System.ComponentModel;
using System.Text.Json;
using galaxy_cli.DTO.Crews;
using galaxy_cli.Services;
using galaxy_cli.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.CrewCommands;

[Description("Displays crews available for hire.")]
public class CrewListCommand : AsyncCommand<EmptyCommandSetting>
{
    private readonly ICrewsService _crewService;

    public CrewListCommand(ICrewsService crewsService)
    {
        _crewService = crewsService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, EmptyCommandSetting settings)
    {

        List<CrewSummaryDTO>? crewItems = [];

        await AnsiConsole.Status()
        .StartAsync("Calling API...", async ctx =>
        {
            ctx.Status("Processing...");
            crewItems = await _crewService.GetAllCrewsAsync();
            AnsiConsole.MarkupLine($"[green]API call successful[/]");
        });



        if (crewItems.Any())
        {

            var crew = AnsiConsole.Prompt(
                new SelectionPrompt<CrewSummaryDTO>()
                    .Title("Choose a crew:")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more crews)[/]")
                    .AddChoices(crewItems).UseConverter(s => s.Name));

            var grid = ConsoleUiHelper.BuildCrewGrid(crew);
            AnsiConsole.Write(
            new Panel(grid)
                .Header("Crew Information"));
        }

        return 0;
    }
}