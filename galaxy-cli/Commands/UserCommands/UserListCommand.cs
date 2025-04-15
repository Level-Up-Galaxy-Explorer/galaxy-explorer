using System.ComponentModel;
using galaxy_cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using galaxy_api.Models;

namespace galaxy_cli.Commands.UserCommands;

[Description("Displays all users")]
public class UserListCommand : Command<EmptyCommandSettings>
{
    private readonly IUserService _userService;

    public UserListCommand(IUserService userService)
    {
        _userService = userService;
    }

    public override int Execute(CommandContext context, EmptyCommandSettings settings)
    {
        var users = _userService.GetUsersAsync().GetAwaiter().GetResult();
        var table = new Table();
        table.AddColumns("ID", "Full Name", "Email", "Google ID", "Rank ID", "Active", "Created At");

        if (users.Any())
        {
            AnsiConsole.MarkupLine($"\n[blue]Users:[/]");
            foreach (var user in users)
            {
                table.AddRow(
                    user.User_Id.ToString(),
                    user.Full_Name ?? "",
                    user.Email_Address ?? "",
                    user.Google_Id ?? "",
                    user.Rank_Id.ToString(),
                    user.Is_Active?.ToString() ?? "",
                    user.Created_At?.ToString("yyyy-MM-dd") ?? ""
                );
            }
            AnsiConsole.Write(table);
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]No users found.[/]");
        }
        return 0;
    }
}