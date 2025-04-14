using galaxy_api.DTOs;
using galaxy_api.Models;

namespace galaxy_api.Services
{
    public interface IMissionService
    {
        Task<IEnumerable<Missions>> GetAllMissionsAsync();
        Task<Missions?> GetMissionByIdAsync(int id);
        Task CreateMissionAsync(Missions missions);
        Task UpdateMissionDetailsAsync(int id, Missions missions);
        Task ProvideMissionFeedbackAsync(int id, Missions missions);
        Task UpdateMissionStatusAsync(int id, Missions missions);
        Task RewardCreditMissionAsync(int id, Missions missions);
        Task<IEnumerable<MissionStatusReport>> GetMissionStatusReportAsync(string? missionType, string? status, string? groupBy);



    }
}