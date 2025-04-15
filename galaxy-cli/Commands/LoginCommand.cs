using System.ComponentModel;
using galaxy_cli.Services;
using galaxy_cli.Settings;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Commands;

[Description("Sign in")]
public class LoginCommand : Command<EmptyCommandSetting>
{

    private IAuthService _authService;
    private ITokenStore _tokenStore;

    public LoginCommand(IAuthService authService, ITokenStore tokenStore)
    {
        _authService = authService;
        _tokenStore = tokenStore;
    }

    public override int Execute(CommandContext context, EmptyCommandSetting settings)
    {
        AnsiConsole.Status()
                  .StartAsync("Signing in with Google...", async ctx =>
                  {
                      ctx.Spinner(Spinner.Known.Clock);
                      await Task.Delay(2500);

                      string? jwtToken = await _authService.authenticate(null);
                      if (jwtToken != null)
                      {
                          ctx.Status("Finishing up...");
                          await _tokenStore.SaveTokenAsync(jwtToken);
                          AnsiConsole.MarkupLine($"[green]Login successful Welcome User![/]");
                      }
                      else
                      {
                          AnsiConsole.MarkupLine($"[red]Login failed. Please try again.[/]");
                      }
                  }).GetAwaiter().GetResult();
        return 0;
    }

}