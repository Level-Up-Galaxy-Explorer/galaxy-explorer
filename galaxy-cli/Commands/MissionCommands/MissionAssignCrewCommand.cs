
using System.ComponentModel;
using galaxy_api.DTOs;
using galaxy_api.DTOs.Missions;
using galaxy_cli.Commands.Base;
using galaxy_cli.Commands.UserCommands;
using galaxy_cli.DTO.Crews;
using galaxy_cli.Services;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

[Description("Shows history of mission")]
public class AssignMissionToCrew : BaseApiCommand<IdentifierSettings>
{
    private readonly IMissionService _missionService;
    private readonly ICrewsService _crewService;


    public AssignMissionToCrew(IMissionService missionService, ICrewsService crewsService, ILogger<AssignMissionToCrew> logger) : base(logger)
    {
        _missionService = missionService;
        _crewService = crewsService;
    }

    protected override async Task<int> ExecuteApiLogic(CommandContext context, IdentifierSettings settings)
    {
        var missions = _missionService.GetMissionsAsync().GetAwaiter().GetResult().Where(m => m.Status_Id == 1);

        if (!missions.Any())
        {
            AnsiConsole.MarkupLine($"[grey]There aren't any missions available to assign[/]");
            return 0;

        }

        var mission = AnsiConsole.Prompt(
                new SelectionPrompt<MissionDTO>()
                    .Title("Choose a mission:")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more mission)[/]")
                    .AddChoices(missions).UseConverter(s => s.Name));


        List<CrewSummaryDTO>? crewItems = [];

        await AnsiConsole.Status()
        .StartAsync("Calling API...", async ctx =>
        {
            ctx.Status("Processing...");
            crewItems = await _crewService.GetAllCrewsAsync();
            AnsiConsole.MarkupLine($"[green]API call successful[/]");
        });

        var availableCrews = crewItems.Where(c => c.Is_Available = true);

        if (!availableCrews.Any())
        {
            AnsiConsole.MarkupLine($"[grey]There aren't any crews available to assign[/]");
            return 0;
        }

        var crew = AnsiConsole.Prompt(
            new SelectionPrompt<CrewSummaryDTO>()
                .Title("Choose a crew:")
                .PageSize(5)
                .MoreChoicesText("[grey](Move up and down to reveal more crews)[/]")
                .AddChoices(crewItems).UseConverter(s => s.Name));

        await AnsiConsole.Status().StartAsync("Calling API...", async ctx =>
        {
            ctx.Status("Processing...");
            await _missionService.AssignCrewToMission(mission.Mission_Id, new AssignCrewRequest(crew.Crew_Id));
            AnsiConsole.MarkupLine($"[green]Crew {crew.Name} successfully assign to mission {mission.Name}.[/]");
        });

        return 0;


    }
}