using System.ComponentModel;
using System.Text.Json;
using galaxy_cli.DTO.Crews;
using galaxy_cli.Services;
using galaxy_cli.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.CrewCommands;

[Description("Displays crews available for hire.")]
public class CrewListCommand : AsyncCommand<EmptyCommandSetting>
{
    private readonly ICrewsService _crewService;

    public CrewListCommand(ICrewsService crewsService)
    {
        _crewService = crewsService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, EmptyCommandSetting settings)
    {
        
        List<CrewSummaryDTO>? crewItems = [];

        await AnsiConsole.Status()
        .StartAsync("Calling API...", async ctx =>
        {
            try
            {
                ctx.Status("Processing...");

                crewItems = await _crewService.GetAllCrewsAsync();

                if (crewItems == null)
                {

                    AnsiConsole.MarkupLine("[yellow]Could not deserialize API response.[/]");
                }

                AnsiConsole.MarkupLine($"[green]API call successful[/]");


            }
            catch (HttpRequestException ex)
            {
                AnsiConsole.MarkupLine($"[red]API Request Error:[/]");
                AnsiConsole.MarkupLine($"[red]({ex.StatusCode?.ToString() ?? "N/A"}) {ex.Message}[/]");
            }
            catch (JsonException ex)
            {
                AnsiConsole.MarkupLine($"[red]JSON Deserialization Error: {ex.Message}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An unexpected error occurred:[/]");
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            }
        });



        if (crewItems != null)
        {

            var crew = AnsiConsole.Prompt(
                new SelectionPrompt<CrewSummaryDTO>()
                    .Title("Choose a crew:")
                    .PageSize(crewItems.Count)
                    .MoreChoicesText("[grey](Move up and down to reveal more crews)[/]")
                    .AddChoices(crewItems).UseConverter(s => s.Name));

            var grid = ConsoleUiHelper.BuildCrewGrid(crew);
            AnsiConsole.Write(
            new Panel(grid)
                .Header("Crew Information"));
        }

        return 0;
    }
}