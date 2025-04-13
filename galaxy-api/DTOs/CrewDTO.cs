namespace galaxy_api.DTOs;

public class CrewDTO
{
    public int Crew_Id {get; set;}
    public required string Name {get; set;}
    public bool Is_Available {get; set;}

    public List<UserDTO> Members {get; set;} = [];
}