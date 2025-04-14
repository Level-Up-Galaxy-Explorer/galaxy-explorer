using System.ComponentModel.DataAnnotations;

namespace galaxy_api.DTOs.Crews;

public class UpdateCrewMembersDto
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one user ID must be provided.")]
    public List<int> UserIds { get; set; } = new List<int>();
}