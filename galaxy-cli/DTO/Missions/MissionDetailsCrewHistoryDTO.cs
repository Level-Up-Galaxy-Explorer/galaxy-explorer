namespace galaxy_cli.DTO.Missions;

public class MissionDetailsWithCrewHistoryDTO
{

    public required string Name { get; set; }
    public required string MissionTypeName { get; set; }
    public DateTime LaunchDate { get; set; }
    public required string DestinationPlanetName { get; set; }
    public string? RewardCredit { get; set; } 
    public string? Feedback { get; set; }
    public required string OverallMissionStatusName { get; set; } 
    public string? CreatedByFullName { get; set; }

    
    public List<MissionCrewHistoryItemDTO> CrewHistory { get; set; } = [];
}