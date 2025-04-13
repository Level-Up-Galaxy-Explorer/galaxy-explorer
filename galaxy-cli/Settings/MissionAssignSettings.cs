using System.ComponentModel;
using Spectre.Console.Cli;

namespace galaxy_cli.Settings;

public class MissionsAssignSettings : CommandSettings
{
    [CommandOption("-m|--mission-id <MISSION_ID>")]
    [Description("The ID of the mission to assign crew to.")]
    public int MissionId { get; set; }

    [CommandOption("-c|--crew-id <CREW_ID>")]
    [Description("The ID of the crew member to assign.")]
    public int CrewId { get; set; }
}