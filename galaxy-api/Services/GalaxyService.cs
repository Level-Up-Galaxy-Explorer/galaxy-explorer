using galaxy_api.Models;
using galaxy_api.Repositories;

namespace galaxy_api.Services
{
    public class GalaxyService : IGalaxyService
    {
        private readonly IGalaxyRepository _repository;

        public GalaxyService(IGalaxyRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Galaxy>> GetAllGalaxyAsync()
        {
            var galaxy = await _repository.GetAllGalaxyAsync();

            return galaxy.OrderBy(g => g.Distance_From_Earth);
        }

        public async Task<Galaxy?> GetGalaxyByIdAsync(int id)
        {
            return await _repository.GetGalaxyByIdAsync(id);
        }

        public async Task<IEnumerable<Galaxy>> GetGalaxyByTypeAsync(string galaxyType)
        {
            var all = await _repository.GetAllGalaxyAsync();

            return all.Where(g => g.Description.Contains(galaxyType, StringComparison.OrdinalIgnoreCase));
        }

        public async Task AddGalaxyAsync(Galaxy galaxy)
        {
            await _repository.AddGalaxyAsync(galaxy);
        }

        public async Task UpdateGalaxyAsync(int id, Galaxy galaxy)
        {
            await _repository.UpdateGalaxyAsync(id, galaxy);
        }

        public async Task<IEnumerable<string>> GetGalaxyTypesAsync()
        {
            return await _repository.GetGalaxyTypesAsync();
        }
    }
}
