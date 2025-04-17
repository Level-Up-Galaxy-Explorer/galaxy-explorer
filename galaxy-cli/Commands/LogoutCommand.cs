using System.ComponentModel;
using galaxy_cli.Services;
using galaxy_cli.Settings;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands;

[Description("Sign in")]
public class LogoutCommand : Command<EmptyCommandSetting>
{

    private ITokenStore _tokenStore;

    public LogoutCommand(ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    public override int Execute(CommandContext context, EmptyCommandSetting settings)
    {
        _tokenStore.ClearTokenAsync();
        AnsiConsole.MarkupLine($"[green]Logout was successful![/]");
        return 1;
    }

}