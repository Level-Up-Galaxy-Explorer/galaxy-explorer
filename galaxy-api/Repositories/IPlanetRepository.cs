using galaxy_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace galaxy_api.Repositories
{
    public interface IPlanetRepository
    {
        Task<IEnumerable<Planet>> GetAllPlanetsAsync();
        Task<Planet?> GetPlanetByIdAsync(int id); 
        Task<IEnumerable<Planet>> SearchPlanetsAsync(string name);
        Task<Planet> AddPlanetAsync(Planet planet);
        Task<bool> UpdatePlanetAsync(int planetId, Planet planet);
    }
}