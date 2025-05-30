using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace galaxy_cli.Settings;

public class IdentifierSettings : CommandSettings
{
    [CommandOption("-i|--id <IDENTIFIER>")]
    [Description("The unique ID or name of the target item (system, planet, crew, mission, etc.).")]
    public int? Identifier { get; set; } 

    public override ValidationResult Validate()
    {
        try
        {
            if (!Identifier.HasValue)
                return ValidationResult.Error("--id option is required.");
            else
                return ValidationResult.Success();
        }
        catch (Exception e)
        {
            return ValidationResult.Error($"Illegal characters or {e.Message}");
        }    
    }
}