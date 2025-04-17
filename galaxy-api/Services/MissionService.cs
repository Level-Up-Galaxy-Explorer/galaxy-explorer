using ErrorOr;
using galaxy_api.DTOs;
using galaxy_api.DTOs.Missions;
using galaxy_api.Models;
using galaxy_api.Repositories;

namespace galaxy_api.Services
{
    public class MissionService : IMissionService
    {
        private readonly IMissionRepository _repository;

        public MissionService(IMissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Missions>> GetAllMissionsAsync()
        {
            var missions = await _repository.GetAllMissionsAsync();

            return missions.OrderBy(m => m.Mission_Id);
        }

        public async Task<Missions?> GetMissionByIdAsync(int id)
        {
            return await _repository.GetMissionByIdAsync(id);
        }

        public async Task CreateMissionAsync(Missions missions)
        {
            await _repository.CreateMissionAsync(missions);
        }

        public async Task UpdateMissionDetailsAsync(int id, Missions missions)
        {
            await _repository.UpdateMissionDetailsAsync(id, missions);
        }

        public async Task ProvideMissionFeedbackAsync(int id, Missions missions)
        {
            await _repository.ProvideMissionFeedbackAsync(id, missions);
        }

        public async Task UpdateMissionStatusAsync(int id, Missions missions)
        {
            await _repository.UpdateMissionStatusAsync(id, missions);
        }

        public async Task RewardCreditMissionAsync(int id, Missions missions)
        {
            await _repository.RewardCreditMissionAsync(id, missions);
        }
        public async Task<IEnumerable<MissionStatusReport>> GetMissionStatusReportAsync(string? missionType, string? status, string? groupBy)
        {
            var data = await _repository.GetMissionStatusReportAsync(missionType, status, groupBy);

            return groupBy?.ToLower() switch
            {
                "month" => data.OrderBy(r => r.Period).ThenBy(r => r.MissionType),
                "quarter" => data.OrderBy(r => r.Period).ThenBy(r => r.MissionType),
                _ => data.OrderBy(r => r.MissionType).ThenBy(r => r.Status)
            };
        }

        public async Task<ErrorOr<MissionDetailsWithCrewHistoryDTO>> GetMissionDetailsWithCrewHistoryAsync(int missionId)
        {
            return await _repository.GetMissionDetailsWithCrewHistoryAsync(missionId);
        }

        public async Task<ErrorOr<Success>> AssignCrewToMissionAsync(int missionId, int crewId)
        {
            return await _repository.AssignCrewToMissionAsync(missionId, crewId);
        }

        public async Task<ErrorOr<Success>> UpdateMissionsStatusAsync(int missionId, int crewId, MissionStatusUpdateDto missions)
        {
            return await _repository.UpdateMissionsStatusAsync(missionId, crewId, missions);
        }
    }
}
