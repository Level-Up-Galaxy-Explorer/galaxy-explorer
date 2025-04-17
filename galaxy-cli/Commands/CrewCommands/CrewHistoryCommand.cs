using System.ComponentModel;
using galaxy_api.DTOs.Crews;
using galaxy_cli.Commands.Base;
using galaxy_cli.DTO.Crews;
using galaxy_cli.Services;
using galaxy_cli.Settings;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.CrewCommands;

[Description("Shows mission history of a crew")]
public class CrewMissionHistoryCommand : BaseApiCommand<IdSettings>
{
    ICrewsService _crewService;

    public CrewMissionHistoryCommand(ILogger<CrewMissionHistoryCommand> logger, ICrewsService crewsService) : base(logger)
    {
        _crewService = crewsService;
    }

    protected override async Task<int> ExecuteApiLogic(CommandContext context, IdSettings settings)
    {
        CrewMissionSummaryDTO? crewMissionSummary = null;

        List<CrewSummaryDTO>? crewItems = [];

        var selectedCredId = settings.Id;

        if (settings.Id == 0)
        {
            await AnsiConsole.Status()
            .StartAsync("Calling API...", async ctx =>
            {
                ctx.Status("Processing...");
                crewItems = await _crewService.GetAllCrewsAsync();
                AnsiConsole.MarkupLine($"[green]API call successful[/]");
            });

            if (crewItems.Count != 0)
            {

                var crew = AnsiConsole.Prompt(
                    new SelectionPrompt<CrewSummaryDTO>()
                        .Title("Choose a crew:")
                        .PageSize(5)
                        .MoreChoicesText("[grey](Move up and down to reveal more crews)[/]")
                        .AddChoices(crewItems).UseConverter(s => s.Name));

                selectedCredId = crew.Crew_Id;

                AnsiConsole.MarkupLine($"[yellow]{selectedCredId}[/]");

            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Error: There are no crew to load.[/]");
                return 1;
            }
        }

        if (selectedCredId != 0)
        {

            CrewMissionSummaryDTO? history = null;

            await AnsiConsole.Status()
            .StartAsync("Fetching crew history...", async ctx =>
            {
                ctx.Status("Processing...");
                history = await _crewService.GetCrewMissionHistory(selectedCredId);

            });

            var inProgressMissions = 0;
            var abortedMissions = 0;
            var successfulMissions = 0;


            if (history != null)
            {

                var grid = new Grid()
                .AddColumn(new GridColumn().NoWrap().PadRight(4))
                .AddColumn()
                .AddRow("[b][grey]Crew Name[/][/]", $"{history.Name}")
                .AddEmptyRow();

                var simple = new Table()
                    .Title("[[ [yellow]Mission History[/] ]]")
                    .Border(TableBorder.SimpleHeavy)
                    .BorderColor(Color.Yellow)
                    .AddColumn("Mission Name")
                    .AddColumn("Type")
                    .AddColumn("Destination Planet")
                    .AddColumn("Mission Status");

                if (history.Missions.Count > 0)
                {

                    inProgressMissions = history.Missions.Count(m => m.OverallMissionStatusName == "Launched");
                    abortedMissions = history.Missions.Count(m => m.OverallMissionStatusName == "Aborted");
                    successfulMissions = history.Missions.Count(m => m.OverallMissionStatusName == "Completed");


                    history.Missions.ForEach(h => simple.AddRow(h.MissionName, h.MissionTypeName, h.DestinationPlanetName, h.AssignmentStatusName));


                    simple.Caption("[[ [blue]THE END[/] ]]");


                }
                else
                {
                    simple.Caption($"[grey]Seems this crew isn't interested in doing missions.[/]");
                }


                grid.AddRow(simple);

                AnsiConsole.Write(new Panel(grid).Header("Crew Information"));
                AnsiConsole.MarkupLine($"Total number of missions: {history.Missions.Count}");
                AnsiConsole.MarkupLine($"Number of successful missions:{successfulMissions}");
                AnsiConsole.MarkupLine($"Number of aborted or cancelled missions: {abortedMissions}");
            }



        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Error: There are no crew to load.[/]");
        }



        return 0;
    }
}