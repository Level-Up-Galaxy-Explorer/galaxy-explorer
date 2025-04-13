using System.ComponentModel;
using galaxy_cli.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.CrewCommands;

[Description("Create a new crew")]
public class CrewCreateCommand : Command<EmptyCommandSetting>
{
    public override int Execute(CommandContext context, EmptyCommandSetting settings)
    {
        var crew_name = AnsiConsole.Prompt(
            new TextPrompt<int>("Enter a crew name.")
                .PromptStyle("green"));
        AnsiConsole.MarkupLine($"You entered age: [blue]{crew_name}[/]");
        return 0;
    }
}