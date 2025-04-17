namespace galaxy_api.DTOs.Missions;

public class MissionCrewStatusUpdateDto
    {
        public int Status_Id { get; set; }
        public int CrewId {get; set;}
        public string? Feedback { get; set; }
        public string? Reward_Credit { get; set; }
    }