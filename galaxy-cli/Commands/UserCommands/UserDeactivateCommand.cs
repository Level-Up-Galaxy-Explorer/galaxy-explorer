using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using galaxy_cli.Services;

namespace galaxy_cli.Commands.UserCommands;

[Description("Deactivates a user by ID.")]
public class UserDeactivateCommand : Command<IdentifierSettings>
{
    private readonly IUserService _userService;

    public UserDeactivateCommand(IUserService userService)
    {
        _userService = userService;
    }

    public override int Execute(CommandContext context, IdentifierSettings settings)
    {
        var userId = settings.Id != 0
            ? settings.Id
            : AnsiConsole.Ask<int>("Enter the [green]user ID[/] to deactivate:");

        var confirm = AnsiConsole.Confirm($"Are you sure you want to deactivate user with ID {userId}?");
        if (!confirm)
        {
            AnsiConsole.MarkupLine("[yellow]Operation cancelled.[/]");
            return 0;
        }

        try
        {
            var success = _userService.DeactivateUserAsync(userId).GetAwaiter().GetResult();
            return success ? 0 : -1;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"An unexpected error occurred: {ex.Message}");
            return -1;
        }
    }
}

public class IdentifierSettings : CommandSettings
{
    [CommandArgument(0, "[id]")]
    public int Id { get; set; }
}