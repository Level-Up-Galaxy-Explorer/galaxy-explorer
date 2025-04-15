namespace galaxy_api.DTOs.Crews;

public class CrewDetailDTO
{
    public int CrewId { get; set; }
    public required string CrewName { get; set; }
    public bool CrewIsAvailable { get; set; }

    List<UserSummaryDTO> Members { get; set;} = [];

    public int TotalMissionsParticipated { get; set; }
    public int SuccessfulMissions { get; set; }
    public int FailedOrAbortedMissions { get; set; }
}