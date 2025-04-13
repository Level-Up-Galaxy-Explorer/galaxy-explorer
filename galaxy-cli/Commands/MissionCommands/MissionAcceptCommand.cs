using System.ComponentModel;
using galaxy_cli.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.MissionCommands;

[Description("Accepts an available mission.")]
public class MissionAcceptCommand : Command<IdentifierSettings>
{

    public override int Execute(CommandContext context, IdentifierSettings settings)
    {
        AnsiConsole.MarkupLine($"Listing crews with filter:");
        return 0;
    }
}