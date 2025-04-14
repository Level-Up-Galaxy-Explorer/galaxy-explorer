using System.ComponentModel;
using System.Text.Json;
using galaxy_cli.DTO.Crews;
using galaxy_cli.Services;
using galaxy_cli.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.CrewCommands;

[Description("Shows detailed information about a specific crew.")]
public class CrewDetailCommand : AsyncCommand<IdentifierSettings>
{
    private readonly ICrewsService _crewService;

    public CrewDetailCommand(ICrewsService crewsService)
    {
        _crewService = crewsService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, IdentifierSettings settings)
    {
        CrewSummaryDTO? crew = null;

        await AnsiConsole.Status()
        .StartAsync("Calling API...", async ctx =>
        {
            try
            {
                ctx.Status("Processing...");

                crew = await _crewService.GetCrewItemAsync(settings.Identifier!.Value);

                if (crew == null)
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

        if (crew != null)
        {

            var grid = ConsoleUiHelper.BuildCrewGrid(crew);
            AnsiConsole.Write(
            new Panel(grid)
                .Header("Crew Information"));
        }

        return 0;
    }
}