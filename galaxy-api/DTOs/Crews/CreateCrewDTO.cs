using System.ComponentModel.DataAnnotations;

namespace galaxy_api.DTOs;

public class CreateCrewDto
{
    [Required]
    [StringLength(64)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public bool IsAvailable { get; set; }

    public List<int> MemberUserIds { get; set; } = new List<int>();

    
}