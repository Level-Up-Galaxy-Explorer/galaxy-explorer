using System.ComponentModel;
using galaxy_cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.MissionCommands;

[Description("Displays missions")]
public class MissionListCommand : Command<EmptyCommandSettings>
{
    private readonly IMissionService _missionService;

    public MissionListCommand(IMissionService missionService)
    {
        _missionService = missionService;
    }

    public override int Execute(CommandContext context, EmptyCommandSettings settings)
    {
        var missions = _missionService.GetMissionsAsync().GetAwaiter().GetResult();
        var table = new Table();

        table.AddColumns("Name", "Type", "Status", "Launch Date", "Planet", "Created By", "Reward Credit", "Feedback");

        if (missions.Any())
        {
            AnsiConsole.MarkupLine($"\n[green]Missions:[/]");
            foreach (var mission in missions)
            {
                table.AddRow(
                    mission.Name ?? "",
                    mission.Mission_Type_Id.ToString(),
                    mission.Status_Id.ToString(),
                    mission.Launch_Date.ToString("yyyy-MM-dd"),
                    mission.Destination_Planet_Id.ToString(),
                    mission.Created_By ?? "",
                    string.IsNullOrWhiteSpace(mission.Reward_Credit) ? "[grey]N/A[/]" : mission.Reward_Credit ?? "",
                    string.IsNullOrWhiteSpace(mission.Feedback) ? "[grey]N/A[/]" : mission.Feedback ?? ""
                );
            }

            AnsiConsole.Write(table);
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]No missions found.[/]");
        }

        return 0;
    }
}
