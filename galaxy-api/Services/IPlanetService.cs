using galaxy_api.DTOs;

namespace galaxy_api.Services
{
    public interface IPlanetService
    {
        Task<IEnumerable<PlanetDTO>> GetAllPlanetsAsync();
    }
}
