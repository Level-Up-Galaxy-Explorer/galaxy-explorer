namespace galaxy_api.DTOs.Crews;

public class CrewMissionSummaryDTO {

    public int Crew_Id {get; set;}
    public required string Name {get; set;}
    public bool Is_Available {get; set;}

    public List<CrewMissionHistoryItemDTO> Missions {get; set;} = [];

}