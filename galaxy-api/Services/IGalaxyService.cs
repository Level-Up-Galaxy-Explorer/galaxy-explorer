using galaxy_api.Models;

namespace galaxy_api.Services
{
    public interface IGalaxyService
    {
        Task<IEnumerable<Galaxy>> GetAllGalaxyAsync();
        Task<Galaxy?> GetGalaxyByIdAsync(int id);
        Task AddGalaxyAsync(Galaxy galaxy);
        Task UpdateGalaxyAsync(int id, Galaxy galaxy);
        Task<IEnumerable<string>> GetGalaxyTypesAsync();
    }
}