using System.ComponentModel;
using galaxy_cli.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.MissionCommands;

[Description("Initiates the off-screen resolution of the mission.")]
public class MissionLaunchCommand : Command<IdentifierSettings>
{

    public override int Execute(CommandContext context, IdentifierSettings settings)
    {
        AnsiConsole.MarkupLine($"Listing crews with filter:");
        return 0;
    }
}