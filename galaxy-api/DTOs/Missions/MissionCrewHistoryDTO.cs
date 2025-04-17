namespace galaxy_api.DTOs.Missions;

public class MissionCrewHistoryItemDTO
{
    public required string CrewName { get; set; }
    public bool CrewIsAvailable { get; set; } 

    public DateTime AssignedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public required string AssignmentStatusName { get; set; } 
}