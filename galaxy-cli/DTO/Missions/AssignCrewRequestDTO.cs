using System.ComponentModel.DataAnnotations;

namespace galaxy_api.DTOs.Missions;

public class AssignCrewRequest
{
    private int crew_Id;

    [Required]    
    public int CrewId { get; set; }

    public int? InitialAssignmentStatusId { get; set; }

    public AssignCrewRequest(int crewId, int? initialAssignmentStatusId)
    {
        CrewId = crewId;
        InitialAssignmentStatusId = initialAssignmentStatusId;
    }

    public AssignCrewRequest(int crew_Id)
    {
        this.crew_Id = crew_Id;
    }
}