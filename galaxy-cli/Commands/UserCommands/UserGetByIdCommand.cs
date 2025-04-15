using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using galaxy_cli.Services;
using galaxy_api.Models;
using galaxy_cli.Settings;

namespace galaxy_cli.Commands.UserCommands;

[Description("Displays details for a specific user by ID.")]
public class UserGetByIdCommand : Command<IdentifierSettings>
{
    private readonly IUserService _userService;

    public UserGetByIdCommand(IUserService userService)
    {
        _userService = userService;
    }

    public override int Execute(CommandContext context, IdentifierSettings settings)
    {
        var userId = settings.Id != 0
            ? settings.Id
            : AnsiConsole.Ask<int>("Enter the [green]user ID[/] to view:");

        var user = _userService.GetUserByIdAsync(userId).GetAwaiter().GetResult();

        if (user == null)
        {
            AnsiConsole.MarkupLine($"[red]User with ID {userId} not found.[/]");
            return -1;
        }

        var table = new Table();
        table.AddColumns("Field", "Value");
        table.AddRow("ID", user.User_Id.ToString());
        table.AddRow("Full Name", user.Full_Name ?? "");
        table.AddRow("Email", user.Email_Address ?? "");
        table.AddRow("Google ID", user.Google_Id ?? "");
        table.AddRow("Rank ID", user.Rank_Id.ToString());
        table.AddRow("Active", user.Is_Active?.ToString() ?? "");
        table.AddRow("Created At", user.Created_At?.ToString("yyyy-MM-dd") ?? "");

        AnsiConsole.Write(table);
        return 0;
    }
}
