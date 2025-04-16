using galaxy_api.Models;

namespace galaxy_api.Repositories
{
    public interface IRankRepository
    {
        Task<IEnumerable<Rank>> GetAllRanksAsync();
        Task<Rank?> GetRankByIdAsync(int id);
        Task AddRankAsync(Rank rank);
        Task UpdateRankDetailsAsync(int id, Rank rank);
    }
}