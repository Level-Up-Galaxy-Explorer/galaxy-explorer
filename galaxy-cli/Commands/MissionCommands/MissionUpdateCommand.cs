using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using galaxy_cli.Services;
using galaxy_api.DTOs;

namespace galaxy_cli.Commands.MissionCommands;

[Description("Updates an existing mission (only name, type, launch date, or destination planet)")]
public class MissionUpdateCommand : Command<EmptyCommandSettings>
{
    private readonly IMissionService _missionService;

    public MissionUpdateCommand(IMissionService missionService)
    {
        _missionService = missionService;
    }

    public override int Execute(CommandContext context, EmptyCommandSettings settings)
    {
        AnsiConsole.MarkupLine("[blue]Update a mission[/]");

        var missionId = AnsiConsole.Ask<int>("Enter the [green]mission ID[/] to update:");
        var existingMission = _missionService.GetMissionByIdAsync(missionId).GetAwaiter().GetResult();

        if (existingMission == null)
        {
            AnsiConsole.MarkupLine($"[red]Mission with ID {missionId} not found.[/]");
            return -1;
        }

        var name = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]new mission name[/] ([grey]leave blank to keep unchanged[/]):")
                .AllowEmpty());

        var missionType = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]new mission type[/] ([grey]leave blank to keep unchanged[/]):")
                .AllowEmpty());

        var planetType = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]new destination planet[/] ([grey]leave blank to keep unchanged[/]):")
                .AllowEmpty());

        var launchDateStr = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the [green]new launch date[/] (yyyy-MM-dd) ([grey]leave blank to keep unchanged[/]):")
                .AllowEmpty());

        var mission = new MissionDTO
        {
            Mission_Id = missionId,
            Name = string.IsNullOrWhiteSpace(name) ? existingMission.Name : name,
            Mission_Type = string.IsNullOrWhiteSpace(missionType) ? existingMission.Mission_Type : missionType,
            Planet_Type = string.IsNullOrWhiteSpace(planetType) ? existingMission.Planet_Type : planetType,
            Launch_Date = DateTime.TryParse(launchDateStr, out var dt) ? dt : existingMission.Launch_Date,
            Status_Type = existingMission.Status_Type,
            Reward_Credit = existingMission.Reward_Credit,
            Feedback = existingMission.Feedback,
            Created_By_Name = existingMission.Created_By_Name
        };

        try
        {
            var success = _missionService.UpdateMissionDetailsAsync(missionId, mission).GetAwaiter().GetResult();
            return success ? 0 : -1;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"An unexpected error occurred: {ex.Message}");
            return -1;
        }
    }
}