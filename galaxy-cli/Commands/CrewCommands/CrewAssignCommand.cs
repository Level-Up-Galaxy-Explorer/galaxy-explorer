using System.ComponentModel;
using galaxy_cli.Commands.Base;
using galaxy_cli.DTO;
using galaxy_cli.DTO.Crews;
using galaxy_cli.Models;
using galaxy_cli.Services;
using galaxy_cli.Settings;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.CrewCommands;

[Description("Assign user to a crew")]
public class CrewAssignCommand : BaseApiCommand<ManageCrewSettings>
{
    private readonly ICrewsService _crewService;
    private readonly IUserService _userService;

    public CrewAssignCommand(ICrewsService crewsService, IUserService userService, ILogger<CrewAssignCommand> logger) : base(logger)
    {
        _crewService = crewsService;
        _userService = userService;

    }

    protected override async Task<int> ExecuteApiLogic(CommandContext context, ManageCrewSettings settings)
    {

        var selectedCredId = settings.CrewId;
        var selectedUserId = settings.UserId;

        IEnumerable<int> selectedUserIds = [];

        if (!settings.CrewId.HasValue)
        {

            List<CrewSummaryDTO>? crewItems = [];

            await AnsiConsole.Status()
            .StartAsync("Calling API...", async ctx =>
            {
                ctx.Status("Processing...");
                crewItems = await _crewService.GetAllCrewsAsync();
            });



            if (crewItems.Count != 0)
            {

                var crew = AnsiConsole.Prompt(
                    new SelectionPrompt<CrewSummaryDTO>()
                        .Title("Choose a crew:")
                        .PageSize(5)
                        .MoreChoicesText("[grey](Move up and down to reveal more crews)[/]")
                        .AddChoices(crewItems).UseConverter(s => s.Name));

                selectedCredId = crew.Crew_Id;
                AnsiConsole.MarkupLine($"You selected the [yellow]{crew.Name}[/] crew.");

            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Error: There are no crew to load.[/]");
                return 1;
            }

        }

        if (!settings.UserId.HasValue)
        {
            IEnumerable<UserDTO>? users = [];

            await AnsiConsole.Status()
            .StartAsync("Calling API...", async ctx =>
            {
                ctx.Status("Processing...");
                users = await _userService.GetUsersAsync();
            });



            if (users.Any())
            {

                var user = AnsiConsole.Prompt(
                new MultiSelectionPrompt<UserDTO>()
                    .Title("Choose user to add:")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more users)[/]")
                    .InstructionsText("[grey](Press [blue]<space>[/] to select a user, [green]<enter>[/] to accept)[/]")
                    .AddChoices(users)
                    .UseConverter(s => $" {s.Full_Name} ({((s.Is_Active ?? false) ? "[green]available[/]" : "[grey]not available[/]")}) "));

                selectedUserIds = user.Select(u => u.User_Id);

            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Error: There are no users to load.[/]");
                return 1;
            }

        }

        if (selectedCredId != null)
        {

            await _crewService.AddCrewMembers(selectedCredId ?? -1, new UpdateCrewMembersDto(selectedUserIds.ToList()));
        }

        return 0;

    }
}