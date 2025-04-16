
namespace galaxy_cli.DTO.Crews;

public class UpdateCrewMembersDto
{
    public List<int> UserIds { get; set; } = [];

    public UpdateCrewMembersDto(List<int> userIds)
    {
        UserIds = userIds;

    }
}