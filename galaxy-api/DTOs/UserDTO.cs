namespace galaxy_api.DTOs;

public class UserDTO
{
    public int User_Id { get; set; }
    public required string Full_Name { get; set; }
    public required string Email_Address { get; set; }

    public required string Google_Id { get; set; }

    public RankDTO? Rank { get; set; }

    public bool Is_Active { get; set; }

    public DateTime Created_At { get; set; }
}