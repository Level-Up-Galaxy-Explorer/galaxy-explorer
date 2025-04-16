using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

public class ManageCrewSettings : CommandSettings
{
    [CommandOption("-c|--crewId <IDENTIFIER>")]
    [Description("The unique ID of the crew")]
    public int? CrewId { get; set; } 

    [CommandOption("-u|--userId <IDENTIFIER>")]
    [Description("The unique ID of user")]
    public int? UserId { get; set; } 
                                                         
}