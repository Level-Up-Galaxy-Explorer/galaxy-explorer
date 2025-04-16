using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using galaxy_cli.Services;

namespace galaxy_cli.Commands.MissionCommands;

[Description("Updates the status, feedback, and reward credit of a mission.")]
public class MissionUpdateStatusCommand : Command<EmptyCommandSettings>
{
    private readonly IMissionService _missionService;

    public MissionUpdateStatusCommand(IMissionService missionService)
    {
        _missionService = missionService;
    }

    public override int Execute(CommandContext context, EmptyCommandSettings settings)
    {
        AnsiConsole.MarkupLine("[blue]Update mission status[/]");

        var missionId = AnsiConsole.Ask<int>("Enter the [green]mission ID[/] to update status:");
        var statusId = AnsiConsole.Ask<int>("Enter the [green]new status ID[/]:");

        string? feedback = null;
        string? rewardCredit = null;

        if (statusId != 2)
        {
            feedback = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [green]feedback[/] ([grey]leave blank if not updating[/]):")
                    .AllowEmpty());

            if (statusId != 4)
            {
                rewardCredit = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter [green]reward credit[/] ([grey]leave blank if not updating[/]):")
                        .AllowEmpty());
            }
        }

        try
        {
            var success = _missionService.UpdateMissionStatusAsync(
                missionId,
                statusId,
                string.IsNullOrWhiteSpace(feedback) ? null : feedback,
                string.IsNullOrWhiteSpace(rewardCredit) ? null : rewardCredit
            ).GetAwaiter().GetResult();

            return success ? 0 : -1;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"An unexpected error occurred: {ex.Message}");
            return -1;
        }
    }
}