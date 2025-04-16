using galaxy_api.DTOs;
using galaxy_api.Models;
using galaxy_api.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace galaxy_api.Services
{
    public class PlanetService : IPlanetService
    {
        private readonly IPlanetRepository _repository;

        public PlanetService(IPlanetRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PlanetDTO>> GetAllPlanetsAsync()
        {
            var planets = await _repository.GetAllPlanetsAsync();
            return planets.Select(MapToDTO);
        }

        public async Task<PlanetDTO> AddPlanetAsync(PlanetDTO planetDto)
        {
            var planet = MapToEntity(planetDto);
            var createdPlanet = await _repository.AddPlanetAsync(planet);
            return MapToDTO(createdPlanet);
        }
        public async Task<PlanetDTO?> GetPlanetAsync(int id)
        {
            var planet = await _repository.GetPlanetByIdAsync(id);
            return planet != null ? MapToDTO(planet) : null;
        }
        public async Task<IEnumerable<PlanetDTO>> SearchPlanetsAsync(string name)
        {
            var planets = await _repository.SearchPlanetsAsync(name);
            return planets.Select(MapToDTO);
        }

        public async Task<bool> UpdatePlanetAsync(int planetId, PlanetDTO planetDto)
        {
            var planet = MapToEntity(planetDto);
            return await _repository.UpdatePlanetAsync(planetId, planet);
        }

        private PlanetDTO MapToDTO(Planet planet)
        {
            return new PlanetDTO
            {
                Id = planet.Id,
                Name = planet.Name,
                Galaxy = planet.Galaxy,
                PlanetType = planet.PlanetType,
                HasLife = planet.HasLife,
                Coordinates = planet.Coordinates
            };
        }

        private Planet MapToEntity(PlanetDTO planetDto)
        {
            return new Planet
            {
                Id = planetDto.Id,
                Name = planetDto.Name,
                Galaxy = planetDto.Galaxy,
                PlanetType = planetDto.PlanetType,
                HasLife = planetDto.HasLife,
                Coordinates = planetDto.Coordinates
            };
        }
    }
}