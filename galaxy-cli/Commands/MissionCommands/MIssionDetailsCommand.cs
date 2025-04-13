using System.ComponentModel;
using galaxy_cli.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.MissionCommands;


[Description("Displays specific details")]
public class MissionDetailCommand : Command<EmptyCommandSetting>
{
    public override int Execute(CommandContext context, EmptyCommandSetting settings)
    {
        AnsiConsole.MarkupLine($"Listing crews with filter:");
        return 0;
    }
}