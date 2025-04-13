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
            await _repository.UpdateMissionDetailsAsync(id,missions);
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
    }
}
