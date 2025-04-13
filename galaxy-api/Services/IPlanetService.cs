using galaxy_api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace galaxy_api.Services
{
    public interface IPlanetService
    {
        Task<IEnumerable<PlanetDTO>> GetAllPlanetsAsync();
        Task<PlanetDTO> AddPlanetAsync(PlanetDTO planet);
        Task<bool> UpdatePlanetAsync(int planetId, PlanetDTO planet);
    }
}