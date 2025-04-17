using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using galaxy_cli.Services;
using galaxy_api.DTOs;
using galaxy_cli.Settings;

namespace galaxy_cli.Commands.MissionCommands;

[Description("Displays details for a specific mission by ID.")]
public class MissionGetByIdCommand : Command<IdSettings>
{
    private readonly IMissionService _missionService;

    public MissionGetByIdCommand(IMissionService missionService)
    {
        _missionService = missionService;
    }

    public override int Execute(CommandContext context, IdSettings settings)
    {
        var missionId = settings.Id != 0
            ? settings.Id
            : AnsiConsole.Ask<int>("Enter the [green]mission ID[/] to view:");

        var mission = _missionService.GetMissionByIdAsync(missionId).GetAwaiter().GetResult();

        if (mission == null)
        {
            AnsiConsole.MarkupLine($"[red]Mission with ID {missionId} not found.[/]");
            return -1;
        }

        var table = new Table();
        table.AddColumns("Field", "Value");
        table.AddRow("Name", mission.Name);
        table.AddRow("Type", mission.Mission_Type);
        table.AddRow("Status", mission.Status_Type);
        table.AddRow("Launch Date", mission.Launch_Date.ToString("yyyy-MM-dd"));
        table.AddRow("Planet", mission.Planet_Type);
        table.AddRow("Created By", mission.Created_By_Name);
        table.AddRow("Reward Credit", string.IsNullOrWhiteSpace(mission.Reward_Credit) ? "[grey]N/A[/]" : mission.Reward_Credit);
        table.AddRow("Feedback", string.IsNullOrWhiteSpace(mission.Feedback) ? "[grey]N/A[/]" : mission.Feedback);

        AnsiConsole.Write(table);
        return 0;
    }
}