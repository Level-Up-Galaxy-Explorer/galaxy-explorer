using System.ComponentModel;
using galaxy_cli.Settings;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands;


[Description("Sign in")]
public class LoginCommand : Command<EmptyCommandSetting>
{
    public override int Execute(CommandContext context, EmptyCommandSetting settings)
    {
        
        return 0;
    }
}