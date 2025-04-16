using galaxy_cli.DTO.Planets;

namespace galaxy_cli.Services;

public interface IPlanetService
{
    Task<IEnumerable<PlanetDTO>> GetAllPlanetsAsync();
    Task<PlanetDTO> AddPlanetAsync(PlanetDTO planetDto);
    Task<bool> UpdatePlanetAsync(int planetId, PlanetDTO planetDto);

    Task<IEnumerable<string>> GetPlanetTypesAsync();
    Task<IEnumerable<string>> GetGalaxiesAsync();
}
