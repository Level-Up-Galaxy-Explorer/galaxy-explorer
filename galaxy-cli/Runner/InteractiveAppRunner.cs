using System.Collections.Generic;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Runner;

public class InteractiveAppRunner
{
    private readonly CommandApp _app;
    private readonly List<string> _commandHistory = new();
    private int _historyIndex = -1;

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
            string input = ReadInputWithHistory();

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            _commandHistory.Add(input);
            _historyIndex = _commandHistory.Count;

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
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                await _app.RunAsync(new[] { "--help" });
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An unexpected error occurred:[/]");
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            }

            if (keepRunning)
            {
                AnsiConsole.WriteLine();
            }
        }
        AnsiConsole.MarkupLine("[dim]Goodbye![/]");
    }

    private string ReadInputWithHistory()
    {
        var input = new List<char>();
        ConsoleKeyInfo key;

        AnsiConsole.Markup("[green]>[/] ");

        while (true)
        {
            key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (input.Count > 0)
                {
                    input.RemoveAt(input.Count - 1);
                    Console.Write("\b \b");
                }
            }
            else if (key.Key == ConsoleKey.UpArrow)
            {
                if (_historyIndex > 0)
                {
                    _historyIndex--;
                    ReplaceInputWithHistory(input, _commandHistory[_historyIndex]);
                }
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                if (_historyIndex < _commandHistory.Count - 1)
                {
                    _historyIndex++;
                    ReplaceInputWithHistory(input, _commandHistory[_historyIndex]);
                }
                else
                {
                    _historyIndex = _commandHistory.Count;
                    ReplaceInputWithHistory(input, string.Empty);
                }
            }
            else
            {
                input.Add(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }

        return new string(input.ToArray());
    }

    private void ReplaceInputWithHistory(List<char> input, string history)
    {
        while (input.Count > 0)
        {
            Console.Write("\b \b");
            input.RemoveAt(input.Count - 1);
        }

        input.AddRange(history);
        Console.Write(history);
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
        e.Cancel = false;
    }
}