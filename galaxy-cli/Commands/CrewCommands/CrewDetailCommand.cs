using System.ComponentModel;
using galaxy_cli.Settings;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.CrewCommands;

[Description("Shows detailed information about a specific crew.")]
public class CrewDetailCommand : Command<EmptyCommandSetting>
{
    public override int Execute(CommandContext context, EmptyCommandSetting settings)
    {
        
        return 0;
    }
}