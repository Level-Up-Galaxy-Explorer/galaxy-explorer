using System.ComponentModel.DataAnnotations;

namespace galaxy_api.DTOs.Missions;

public class AssignCrewRequestDTO
{
    [Required]    
    public int CrewId { get; set; }

    public int? InitialAssignmentStatusId { get; set; }
}