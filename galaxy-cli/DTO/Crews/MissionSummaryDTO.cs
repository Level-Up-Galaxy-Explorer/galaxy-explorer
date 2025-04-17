namespace galaxy_api.DTOs.Crews;

public class CrewMissionHistoryItemDTO
{
    // Mission Details
    public long MissionId { get; set; }
    public string MissionName { get; set; }
    public string MissionTypeName { get; set; }
    public DateTime LaunchDate { get; set; }
    public string DestinationPlanetName { get; set; }
    public string? RewardCredit { get; set; }
    public string? Feedback { get; set; }
    public string OverallMissionStatusName { get; set; } 


    public DateTime AssignedAt { get; set; }
    public DateTime? EndedAt { get; set; } 
    public string AssignmentStatusName { get; set; }
}