using galaxy_api.Models;

namespace galaxy_api.Repositories
{
    public interface IGalaxyRepository
    {
        Task<IEnumerable<Galaxy>> GetAllGalaxyAsync();
        Task<Galaxy?> GetGalaxyByIdAsync(int id);
        Task AddGalaxyAsync(Galaxy galaxy);
        Task UpdateGalaxyAsync(int id, Galaxy galaxy);
    }
}