using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands.Base;

public abstract class BaseApiCommand<TSettings> : AsyncCommand<TSettings> where TSettings : CommandSettings
{

    protected readonly ILogger Logger;

    protected BaseApiCommand(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected abstract Task<int> ExecuteApiLogic(CommandContext context, TSettings settings);

    public sealed override async Task<int> ExecuteAsync(CommandContext context, TSettings settings)
    {

        int exitCode = 1;

            
            try
            {
                exitCode = await ExecuteApiLogic(context, settings);
            }
            catch (ApiNotFoundException ex)
            {
                DisplayApiError("The requested resource was not found.", ex, settings);
            }
            catch (ApiAuthorizationException ex)
            {
                DisplayApiError($"Not authorized to access the API ({ex.StatusCode}).", ex, settings);
            }
            catch (ApiNetworkException ex)
            {
                DisplayApiError("Could not connect to the API. Check network connection.", ex, settings, includeInnerMessage: true);
            }
            catch (ApiClientException ex)
            {
                DisplayApiError($"The API reported a client error ({ex.StatusCode}).", ex, settings);
            }
            catch (ApiServerException ex)
            {
                DisplayApiError($"The API reported a server error ({ex.StatusCode}). Please try again later.", ex, settings);
            }
            catch (ApiDeserializationException ex)
            {
                DisplayApiError("Could not process the response from the API. It might be in an unexpected format.", ex, settings);
            }
            catch (ApiException ex)
            {
                DisplayApiError("An issue occurred while communicating with the API.", ex, settings);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[bold red]An unexpected error occurred.[/]");

                Logger.LogError(ex, "An unexpected error occurred during command execution.");
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            }
        
        

        return exitCode;

    }

    protected virtual void DisplayApiError(string userMessage, ApiException ex, TSettings settings, bool includeInnerMessage = false)
    {
        AnsiConsole.MarkupLine($"[red]Error: {userMessage}[/]");

        string details = includeInnerMessage ? (ex.InnerException?.Message ?? ex.Message) : ex.Message;
        if (!string.IsNullOrWhiteSpace(details) && details != userMessage)
        {
            AnsiConsole.MarkupLine($"[red dim]Details: {Markup.Escape(details)}[/]");
        }


        Logger.LogWarning(ex, "API Error encountered: {UserMessage} - Exception: {ExceptionMessage}", userMessage, ex.Message);
    }

}