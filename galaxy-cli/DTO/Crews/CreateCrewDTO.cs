
using System.ComponentModel.DataAnnotations;

namespace galaxy_cli.DTO.Crews;

public class CreateCrewDto
{
    [Required]
    [StringLength(20)]
    public string Name { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }

    public List<int> MemberUserIds { get; set; } = [];

    public CreateCrewDto(string name, List<int>? members, bool isAvailable = false)
    {
        Name = name;
        IsAvailable = isAvailable;
        MemberUserIds = members ?? [];
    }



}