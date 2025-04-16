using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using galaxy_cli.Services;

namespace galaxy_cli.Commands.UserCommands;

[Description("Assigns a new rank to a user.")]
public class UserAssignRankCommand : Command<EmptyCommandSettings>
{
    private readonly IUserService _userService;

    public UserAssignRankCommand(IUserService userService)
    {
        _userService = userService;
    }

    public override int Execute(CommandContext context, EmptyCommandSettings settings)
    {
        var userId = AnsiConsole.Ask<int>("Enter the [green]user ID[/] to assign a rank:");
        var rankId = AnsiConsole.Ask<int>("Enter the [green]rank ID[/] to assign:");

        try
        {
            var success = _userService.AssignRankAsync(userId, rankId).GetAwaiter().GetResult();
            return success ? 0 : -1;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"An unexpected error occurred: {ex.Message}");
            return -1;
        }
    }
}