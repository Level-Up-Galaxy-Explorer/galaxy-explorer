using galaxy_api.DTOs;
using galaxy_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace galaxy_api.Services
{
    public interface IPlanetService
    {
        Task<IEnumerable<PlanetDTO>> GetAllPlanetsAsync();
        Task<PlanetDTO> AddPlanetAsync(PlanetDTO planetDto);
        Task<bool> UpdatePlanetAsync(int planetId, PlanetDTO planetDto);
        Task<PlanetDTO?> GetPlanetAsync(int id);
        Task<IEnumerable<PlanetDTO>> SearchPlanetsAsync(string name);
    }
}