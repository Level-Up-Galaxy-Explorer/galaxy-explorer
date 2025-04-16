using System.ComponentModel;
using galaxy_cli.Commands.Base;
using galaxy_cli.DTO.Crews;
using galaxy_cli.Services;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.CrewCommands;

[Description("Fire user to a crew")]
public class CrewFireCommand : BaseApiCommand<ManageCrewSettings>
{

    private readonly ICrewsService _crewService;

    public CrewFireCommand(ICrewsService crewService, ILogger<CrewFireCommand> logger) : base(logger)
    {
        _crewService = crewService;
    }

    protected override async Task<int> ExecuteApiLogic(CommandContext context, ManageCrewSettings settings)
    {
        var selectedCredId = settings.CrewId;
        var selectedUserId = settings.UserId;

        IEnumerable<int> selectedUserIds = [];

        List<CrewSummaryDTO> crewItems = [];

        if (!settings.CrewId.HasValue)
        {

            crewItems = await GetCrew() ?? [];

            if (crewItems.Count != 0)
            {

                var crew = AnsiConsole.Prompt(
                    new SelectionPrompt<CrewSummaryDTO>()
                        .Title("Choose a crew:")
                        .PageSize(5)
                        .MoreChoicesText("[grey](Move up and down to reveal more crews)[/]")
                        .AddChoices(crewItems).UseConverter(s => s.Name));

                selectedCredId = crew.Crew_Id;

            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Error: There are no crew to load.[/]");
                return 1;
            }

        }

        if (!settings.UserId.HasValue)
        {
            if (crewItems.Count == 0)
            {
                crewItems = await GetCrew() ?? [];
            }

            var crew = crewItems.Where(c => c.Crew_Id == selectedCredId).FirstOrDefault();


            if (crew != null && crew.Members.Count != 0)
            {

                var user = AnsiConsole.Prompt(
                new MultiSelectionPrompt<UserSummaryDTO>()
                    .Title("Choose user(s) to remove:")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more users)[/]")
                    .InstructionsText("[grey](Press [blue]<space>[/] to select a user, [green]<enter>[/] to accept)[/]")
                    .AddChoices(crew.Members)
                    .UseConverter(s => $" {s.Full_Name} "));

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
            await _crewService.RemoveCrewMembers(selectedCredId ?? -1, new UpdateCrewMembersDto(selectedUserIds.ToList()));
            AnsiConsole.MarkupLine($"[green]Users have been successfully removed from the crew.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Error: Crew id is required. Please provide a crew id using --crewId flag or by selecting from list.[/]");
        }

        return 0;

    }

    async Task<List<CrewSummaryDTO>?> GetCrew()
    {
        List<CrewSummaryDTO>? crewItems = [];

        await AnsiConsole.Status()
        .StartAsync("Calling API...", async ctx =>
        {
            ctx.Status("Processing...");
            crewItems = await _crewService.GetAllCrewsAsync();
            AnsiConsole.MarkupLine($"[green]API call successful[/]");
        });

        return crewItems;

    }

}