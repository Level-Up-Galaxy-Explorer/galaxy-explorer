namespace galaxy_cli.DTO.Crews;

public class RankDTO {
    public int Rank_Id {get; set;}
    public required string Title {get; set;}

    public string Description {get; set;} = string.Empty;
}