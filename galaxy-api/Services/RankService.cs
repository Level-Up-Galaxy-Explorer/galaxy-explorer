using galaxy_api.Models;
using galaxy_api.Repositories;

namespace galaxy_api.Services
{
    public class RankService : IRankService
    {
        private readonly IRankRepository _repository;

        public RankService(IRankRepository repository)
        {
            _repository = repository;
        }

        public Task AddRankAsync(Rank rank)
        {
            return _repository.AddRankAsync(rank);
        }

        public Task<IEnumerable<Rank>> GetAllRanksAsync()
        {
            return _repository.GetAllRanksAsync();
        }

        public Task<Rank?> GetRankByIdAsync(int id)
        {
            return _repository.GetRankByIdAsync(id);
        }

        public Task UpdateRankDetailsAsync(int id, Rank rank)
        {
            return _repository.UpdateRankDetailsAsync(id, rank);
        }
    }
}
