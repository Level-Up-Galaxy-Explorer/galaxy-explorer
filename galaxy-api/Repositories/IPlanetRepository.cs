using galaxy_api.DTOs;

namespace galaxy_api.Repositories
{
    public interface IPlanetRepository
    {
        Task<IEnumerable<PlanetDTO>> GetAllPlanetsAsync();
    }
}
