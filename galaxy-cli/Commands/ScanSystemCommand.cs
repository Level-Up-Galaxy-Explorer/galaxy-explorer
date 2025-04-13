using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands;

[Description("Scans galaxies.")]
public class ScanGalaxyCommand : AsyncCommand<ScanGalaxyCommand.ScanGalaxySettings>
{

    public class ScanGalaxySettings : CommandSettings
    {
        [CommandOption("-g|--galaxy <GALAXY_NAME>")]
        [Description("The name of the galaxy to filter on")]
        public string GalaxyName { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, ScanGalaxySettings settings)
    {
        AnsiConsole.MarkupLine("[underline blue]Scanning galaxies...[/]");

        await AnsiConsole.Status()
                    .StartAsync("Thinking...", async ctx =>
                    {
                        AnsiConsole.MarkupLine("Doing step 1...");
                        await Task.Delay(1000);
                        ctx.Spinner(Spinner.Known.Star);
                        ctx.Status("Doing step 2...");
                        AnsiConsole.MarkupLine("Doing step 2...");
                        await Task.Delay(1500);
                        ctx.Spinner(Spinner.Known.Clock);
                        ctx.Status("Finishing up...");
                        AnsiConsole.MarkupLine("Finishing up...");
                        await Task.Delay(1000);
                    });
        AnsiConsole.MarkupLine("[green]Scan complete![/]");

        var galaxy = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose a galaxy")
                .PageSize(5)
                .MoreChoicesText("[grey](Move up and down to reveal more galaxies)[/]")
                .AddChoices(new[] { "Milky Way", "Andromeda", "Pinwheel", "Sombrero", "Tadpole" }));

        // --- Logic to list bodies in current system ---
        var table = new Table().Expand().Border(TableBorder.Rounded);
        table.AddColumn("Name").AddColumn("Type").AddColumn("Status");
        table.AddRow("Earth", "Terrestrial - Earth-like", "[green]Scanned[/]");
        table.AddRow("Mars", "Terrestrial - Arid", "[grey]Unscanned[/]");
        AnsiConsole.Write(table);
        // --- End Logic ---
        return 0;
    }
}