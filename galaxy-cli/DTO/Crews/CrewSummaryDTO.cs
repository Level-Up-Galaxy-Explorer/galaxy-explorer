
namespace galaxy_cli.DTO.Crews;

public class CrewSummaryDTO
{
    public int Crew_Id {get; set;}
    public required string Name {get; set;}
    public bool Is_Available {get; set;}
    public List<UserSummaryDTO> Members {get; set;} = [];
}