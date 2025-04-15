using System.ComponentModel;
using Spectre.Console.Cli;

namespace galaxy_cli.Settings;

public class IdentifierSettings : CommandSettings
{
    [CommandOption("-i|--id <IDENTIFIER>")]
    [Description("The unique ID or name of the target item (system, planet, crew, mission, etc.).")]
    [CommandArgument(0, "[id]")]
    
    public int Id { get; set; }
    public required string Identifier { get; set; } = string.Empty;                                                      
}