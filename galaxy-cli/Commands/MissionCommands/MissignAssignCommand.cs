using galaxy_cli.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.MissionCommands;

public class MissionsAssignCommand : Command<MissionsAssignSettings>
{
    public override int Execute(CommandContext context, MissionsAssignSettings settings)
    {
        AnsiConsole.MarkupLine($"Assigning Crew [yellow]{settings.CrewId}[/] to Mission [yellow]{settings.MissionId}[/]...");
        AnsiConsole.MarkupLine("[green]Assignment successful.[/]");
        return 0;
    }
}