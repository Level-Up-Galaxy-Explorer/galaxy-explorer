using galaxy_cli.DTO.Galaxy;

namespace galaxy_cli.Services
{
    public interface IGalaxyService
    {
        Task<IEnumerable<GalaxyDTO>> GetAllGalaxyAsync();
        Task<GalaxyDTO?> GetGalaxyByIdAsync(int id);
        Task<bool> AddGalaxyAsync(GalaxyDTO galaxy);
        Task<bool> UpdateGalaxyAsync(int id, GalaxyDTO galaxy);
        Task<IEnumerable<string>> GetGalaxyTypesAsync();
    }
}