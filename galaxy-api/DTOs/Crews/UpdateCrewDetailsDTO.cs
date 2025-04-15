using System.ComponentModel.DataAnnotations;

namespace galaxy_api.DTOs.Crews;

public class UpdateCrewDetailsDTO
{
    [StringLength(20)]
    public string? Name { get; set; }

    public bool? IsAvailable { get; set; } = null;

}