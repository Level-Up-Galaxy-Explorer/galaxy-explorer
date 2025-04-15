using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using galaxy_cli.Services;
using galaxy_api.DTOs;

namespace galaxy_cli.Commands.MissionCommands;

[Description("Creates a new mission")]
public class MissionCreateCommand : Command<EmptyCommandSettings>
{
    private readonly IMissionService _missionService;

    public MissionCreateCommand(IMissionService missionService)
    {
        _missionService = missionService;
    }

    public override int Execute(CommandContext context, EmptyCommandSettings settings)
    {
        AnsiConsole.MarkupLine("[blue]Create a new mission[/]");

        var name = AnsiConsole.Ask<string>("Enter the [green]mission name[/]:");
        var missionTypeId = AnsiConsole.Ask<int>("Enter the [green]mission type ID[/]:");
        var planetId = AnsiConsole.Ask<int>("Enter the [green]destination planet ID[/]:");
        var launchDate = AnsiConsole.Prompt(
            new TextPrompt<DateTime>("Enter the [green]launch date[/] (yyyy-MM-dd):")
                .Validate(date =>
                {
                    return date > DateTime.MinValue
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid date[/]");
                })
        );
        var createdBy = AnsiConsole.Ask<string>("Enter the [green]creator name[/]:");

        var mission = new MissionDTO
        {
            Name = name,
            Mission_Type_Id = missionTypeId,
            Launch_Date = launchDate,
            Destination_Planet_Id = planetId,
            Status_Id = 1,
            Reward_Credit = "",
            Feedback = "",
            Created_By = createdBy
        };

        try
        {
            var success = _missionService.CreateMissionAsync(mission).GetAwaiter().GetResult();
            return success ? 0 : -1;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"An unexpected error occurred: {ex.Message}");
            return -1;
        }
    }
}
