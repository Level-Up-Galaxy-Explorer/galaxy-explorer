using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands;

[Description("Shows current user information/status")]
public class StatusCommand : Command<EmptyCommandSettings>
{
    public override int Execute(CommandContext context, EmptyCommandSettings settings)
    {
        AnsiConsole.MarkupLine("[grey]Status command placeholder.[/]");
        return 0;
    }
}