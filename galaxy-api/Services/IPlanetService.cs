using galaxy_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace galaxy_api.Services
{
    public interface IPlanetService
    {
        Task<IEnumerable<Planet>> GetAllPlanetsAsync();
        Task<Planet> AddPlanetAsync(Planet planet);
        Task<bool> UpdatePlanetAsync(int planetId, Planet planet);
    }
}