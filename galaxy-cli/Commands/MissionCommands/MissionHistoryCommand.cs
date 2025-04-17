
using System.ComponentModel;
using galaxy_cli.Commands.Base;
using galaxy_cli.DTO.Missions;
using galaxy_cli.Services;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using galaxy_cli.Settings;
using galaxy_api.DTOs;

namespace galaxy_cli.Commands.MissionCommands;


[Description("Shows history of mission")]
public class MissionDetailsWithHistory : BaseApiCommand<EmptyCommandSetting>
{

    IMissionService _missionService;
    public MissionDetailsWithHistory(IMissionService missionService, ILogger<MissionDetailsWithHistory> logger) : base(logger)
    {
        _missionService = missionService;
    }

    protected async override Task<int> ExecuteApiLogic(CommandContext context, EmptyCommandSetting settings)
    {


        var missions = _missionService.GetMissionsAsync().GetAwaiter().GetResult();

        if (!missions.Any())
        {
            AnsiConsole.MarkupLine($"[grey]Couldn't find any missions.[/]");
            return 0;

        }

        var mission = AnsiConsole.Prompt(
                new SelectionPrompt<MissionDTO>()
                    .Title("Choose a mission:")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more mission)[/]")
                    .AddChoices(missions).UseConverter(s => s.Name));
        MissionDetailsWithCrewHistoryDTO? history = null;

        await AnsiConsole.Status()
        .StartAsync("Fetching mission history...", async ctx =>
        {
            ctx.Status("Processing...");
            history = await _missionService.GetMissionDetailsWithCrewHistory(mission.Mission_Id);

        });

        if (history != null)
        {

            var grid = new Grid()
                .AddColumn(new GridColumn().NoWrap().PadRight(4))
                .AddColumn()
                .AddRow("[b][grey]Mission Name[/][/]", $"{history.Name}")
                .AddRow("[b][grey]Mission Type[/][/]", $"{history.MissionTypeName}")
                .AddRow("[b][grey]Destination Planet[/][/]", history.DestinationPlanetName)
                .AddEmptyRow();

            var simple = new Table()
            .Title("[[ [yellow]Crew that have picked up this mission[/] ]]")
            .Border(TableBorder.SimpleHeavy)
            .BorderColor(Color.Yellow)
            .AddColumn("Crew Name")
            .AddColumn("Date Assigned")
            .AddColumn("Date ended")
            .AddColumn("Mission Status");


            if (history.CrewHistory.Count > 0)
            {

                var abortedMissions = history.CrewHistory.Count(m => m.AssignmentStatusName == "Aborted");

                var dateFormat = "yyyy-MM-dd HH:mm";

                history.CrewHistory.ForEach(h => simple.AddRow(h.CrewName, h.AssignedAt.ToString(dateFormat), h.EndedAt?.ToString(dateFormat) ?? "", h.AssignmentStatusName));


                simple.Caption("[[ [blue]THE END[/] ]]");



            }
            else
            {
                simple.Caption("[[ [blue]Seems no one wants this mission.[/] ]]");
            }
            grid.AddRow(simple);

            AnsiConsole.Write(new Panel(grid).Header("Crew Information"));

        }
        else
        {
            AnsiConsole.MarkupLine($"[red]There was a problem fetching mission details.[/]");
        }

        return 0;
    }
}