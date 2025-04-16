using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Runner;

public class InteractiveAppRunner
{
    private readonly CommandApp _app;

    public InteractiveAppRunner(CommandApp app)
    {
        _app = app ?? throw new ArgumentNullException(nameof(app));
    }

    public async Task RunAsync()
    {
        SetupCtrlCHandler();
        ShowWelcomeMessage();

        bool keepRunning = true;
        while (keepRunning)
        {
            var input = AnsiConsole.Prompt(
                new TextPrompt<string>("[green]>[/]")
                    .PromptStyle("green")
                    .AllowEmpty()
                );

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            var simulatedArgs = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var commandName = simulatedArgs[0].ToLowerInvariant();

            if (commandName == "exit" || commandName == "quit")
            {
                AnsiConsole.MarkupLine("[yellow]Exiting interactive mode...[/]");
                keepRunning = false;
                continue;
            }

            if (commandName == "clear" || commandName == "cls")
            {
                AnsiConsole.Clear();
                ShowWelcomeMessage(); 
                continue;
            }

            try
            {
                await _app.RunAsync(simulatedArgs);
            }
            catch (CommandParseException ex)
            {
                // Handle errors specifically from Spectre's parsing/validation
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                // Optionally show help after a parse error
                await _app.RunAsync(new[] { commandName = "--help" });
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An unexpected error occurred:[/]");
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);

                // keepRunning = false;
            }

            if (keepRunning)
            {
                AnsiConsole.WriteLine();
            }
        }
        AnsiConsole.MarkupLine("[dim]Goodbye![/]");
    }

    private void ShowWelcomeMessage()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("Galaxy Explorer")
                .Centered()
                .Color(Color.Blue));

        AnsiConsole.Write(new Rule("[yellow]Welcome to Galaxy Explorer CLI[/]").RuleStyle("grey").Centered());
                
        AnsiConsole.MarkupLine("[grey]Type a command (e.g., 'crew list', 'mission list'), '--help/-h', or 'exit'/'quit'.[/]");
        AnsiConsole.WriteLine();
    }

    private void SetupCtrlCHandler()
    {
        
        Console.CancelKeyPress -= Console_CancelKeyPress;
        Console.CancelKeyPress += Console_CancelKeyPress;
    }

    private void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        AnsiConsole.MarkupLine("\n[yellow]Ctrl+C detected. Exiting...[/]");
        // Perform cleanup here if needed
        e.Cancel = false;
                          // Environment.Exit(1); // Force exit if necessary
    }

}