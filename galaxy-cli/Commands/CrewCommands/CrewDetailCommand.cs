using System.ComponentModel;
using System.Text.Json;
using galaxy_cli.Commands.Base;
using galaxy_cli.DTO.Crews;
using galaxy_cli.Services;
using galaxy_cli.Settings;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.CrewCommands;

[Description("Shows detailed information about a specific crew.")]
public class CrewDetailCommand : BaseApiCommand<IdentifierSettings>
{
    private readonly ICrewsService _crewService;

    public CrewDetailCommand(ICrewsService crewsService, ILogger<CrewDetailCommand> logger) : base(logger)
    {
        _crewService = crewsService;
    }

    protected override async Task<int> ExecuteApiLogic(CommandContext context, IdentifierSettings settings)
    {
        CrewSummaryDTO? crew = null;

        await AnsiConsole.Status()
        .StartAsync("Calling API...", async ctx =>
        {
            ctx.Status("Processing...");
            crew = await _crewService.GetCrewItemAsync(settings.Identifier!.Value);
            AnsiConsole.MarkupLine($"[green]API call successful[/]");
        });

        if (crew != null)
        {

            var grid = ConsoleUiHelper.BuildCrewGrid(crew);
            AnsiConsole.Write(
            new Panel(grid)
                .Header("Crew Information"));
        }

        return 0;
    }
}