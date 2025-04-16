using System.ComponentModel;
using galaxy_cli.Commands.Base;
using galaxy_cli.DTO.Crews;
using galaxy_cli.Models;
using galaxy_cli.Services;
using galaxy_cli.Settings;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.CrewCommands;

[Description("Create a new crew")]
public class CrewCreateCommand : BaseApiCommand<EmptyCommandSetting>
{

    private readonly ICrewsService _crewService;
    private readonly IUserService _userService;
    public CrewCreateCommand(ICrewsService crewsService, IUserService userService, ILogger<CrewDetailCommand> logger) : base(logger)
    {
        _crewService = crewsService;
        _userService = userService;
    }

    protected override async Task<int> ExecuteApiLogic(CommandContext context, EmptyCommandSetting settings)
    {

        List<Users>? users = [];

        var crew_name = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter a crew name.")
                .PromptStyle("green"));

        var addCrewMembers = AnsiConsole.Prompt(
            new TextPrompt<bool>("Would you like to add crew members now?")
            .AddChoice(true)
            .AddChoice(false)
            .DefaultValue(true)
            .WithConverter(choice => choice ? "y" : "n"));

        await AnsiConsole.Status()
        .StartAsync("Calling API...", async ctx =>
        {
            ctx.Status("Processing...");
            users = await _userService.GetAllUsersAsync();
        });


        var crewDto = new CreateCrewDto(
            crew_name, []
        );


        if (users.Count != 0)
        {

            var user = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Users>()
                    .Title("Choose user to add:")
                    .PageSize(5)
                    .MoreChoicesText("[grey](Move up and down to reveal more users)[/]")
                    .InstructionsText("[grey](Press [blue]<space>[/] to select a user, [green]<enter>[/] to accept)[/]")
                    .AddChoices(users)
                    .UseConverter(s => $" {s.Full_Name} ({((s.Is_Active ?? false) ? "[green]available[/]" : "[grey]not available[/]")}) "));

            var selectedUserIdss = user.Select(u => u.User_Id);

            crewDto.MemberUserIds = selectedUserIdss.ToList();

            if (selectedUserIdss.Any())
            {
                var makeCrewAvalaible = AnsiConsole.Prompt(
                    new TextPrompt<bool>("Would you like to set crew status as available?")
                    .AddChoice(true)
                    .AddChoice(false)
                    .DefaultValue(true)
                    .WithConverter(choice => choice ? "y" : "n"));

                crewDto.IsAvailable = makeCrewAvalaible;
            }
        }
        else
        {
            AnsiConsole.MarkupLine($"Coudn't find any users... ");
            AnsiConsole.MarkupLine($"[orange]Crew without any members will be created[/]");
        }

        await AnsiConsole.Status()
        .StartAsync("Calling API...", async ctx =>
        {
            ctx.Status("Processing...");
            await _crewService.CreateCrewAsync(crewDto);
            AnsiConsole.MarkupLine($"[b][green]Crew created successfully.[/][/]");
        });


        return 0;
    }
}