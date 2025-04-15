using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using galaxy_cli.Services;

namespace galaxy_cli.Commands.MissionCommands;

[Description("Displays a mission status report.")]
public class MissionReportCommand : Command<EmptyCommandSettings>
{
    private readonly IMissionService _missionService;

    public MissionReportCommand(IMissionService missionService)
    {
        _missionService = missionService;
    }

    public override int Execute(CommandContext context, EmptyCommandSettings settings)
    {
        var missionType = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]mission type[/] filter ([grey]leave blank for all[/]):").AllowEmpty());
        var status = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]status[/] filter ([grey]leave blank for all[/]):").AllowEmpty());
        var groupBy = AnsiConsole.Prompt(
            new TextPrompt<string>("Group by [green]month[/], [green]quarter[/], or [green]none[/] ([grey]leave blank for none[/]):").AllowEmpty());

        var report = _missionService.GetMissionStatusReportAsync(
            string.IsNullOrWhiteSpace(missionType) ? null : missionType,
            string.IsNullOrWhiteSpace(status) ? null : status,
            string.IsNullOrWhiteSpace(groupBy) ? null : groupBy
        ).GetAwaiter().GetResult();

        if (report == null || !report.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No mission status report data found.[/]");
            return 0;
        }

        var table = new Table();
        table.AddColumns("Mission Type", "Status", "Count", "Period", "Planet");

        foreach (var row in report)
        {
            table.AddRow(
                row.MissionType ?? "",
                row.Status ?? "",
                row.Count.ToString(),
                row.Period ?? "[grey]N/A[/]",
                row.Planet ?? "[grey]N/A[/]"
            );
        }

        AnsiConsole.Write(table);
        return 0;
    }
}