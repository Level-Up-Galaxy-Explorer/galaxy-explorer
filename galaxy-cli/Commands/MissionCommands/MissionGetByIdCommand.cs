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
        table.AddRow("Name", mission.Name ?? "");
        table.AddRow("Type ID", mission.Mission_Type_Id.ToString());
        table.AddRow("Status ID", mission.Status_Id.ToString());
        table.AddRow("Launch Date", mission.Launch_Date.ToString("yyyy-MM-dd"));
        table.AddRow("Destination Planet ID", mission.Destination_Planet_Id.ToString());
        table.AddRow("Created By", mission.Created_By ?? "");
        table.AddRow("Reward Credit", string.IsNullOrWhiteSpace(mission.Reward_Credit) ? "[grey]N/A[/]" : mission.Reward_Credit ?? "");
        table.AddRow("Feedback", string.IsNullOrWhiteSpace(mission.Feedback) ? "[grey]N/A[/]" : mission.Feedback ?? "");

        AnsiConsole.Write(table);
        return 0;
    }
}