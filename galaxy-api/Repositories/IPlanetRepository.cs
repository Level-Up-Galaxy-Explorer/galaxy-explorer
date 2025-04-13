using galaxy_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace galaxy_api.Repositories
{
    public interface IPlanetRepository
    {
        Task<IEnumerable<Planet>> GetAllPlanetsAsync();
        Task<Planet> AddPlanetAsync(Planet planet);
        Task<bool> UpdatePlanetAsync(int planetId, Planet planet);
    }
}