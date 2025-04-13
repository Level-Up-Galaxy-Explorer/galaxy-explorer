using System.ComponentModel;
using galaxy_cli.Settings;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.CrewCommands;

[Description("Assign user to a crew")]
public class CrewAssignCommand : Command<EmptyCommandSetting>
{
    public override int Execute(CommandContext context, EmptyCommandSetting settings)
    {
        
        return 0;
    }
}