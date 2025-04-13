using galaxy_api.DTOs;
using galaxy_api.Models;
using galaxy_api.Repositories;
using System.Collections.Generic;
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
            return planets.Select(p => new PlanetDTO
            {
                Name = p.Name,
                Galaxy = p.Galaxy,
                PlanetType = p.PlanetType,
                HasLife = p.HasLife,
                Coordinates = p.Coordinates
            });
        }

        public async Task<PlanetDTO> AddPlanetAsync(PlanetDTO planetDto)
        {
            var planet = new Planet
            {
                Name = planetDto.Name,
                Galaxy = planetDto.Galaxy,
                PlanetType = planetDto.PlanetType,
                HasLife = planetDto.HasLife,
                Coordinates = planetDto.Coordinates
            };

            var createdPlanet = await _repository.AddPlanetAsync(planet);
            return new PlanetDTO
            {
                Name = createdPlanet.Name,
                Galaxy = createdPlanet.Galaxy,
                PlanetType = createdPlanet.PlanetType,
                HasLife = createdPlanet.HasLife,
                Coordinates = createdPlanet.Coordinates
            };
        }

        public async Task<bool> UpdatePlanetAsync(int planetId, PlanetDTO planetDto)
        {
            var planet = new Planet
            {
                Name = planetDto.Name,
                Galaxy = planetDto.Galaxy,
                PlanetType = planetDto.PlanetType,
                HasLife = planetDto.HasLife,
                Coordinates = planetDto.Coordinates
            };
            return await _repository.UpdatePlanetAsync(planetId, planet);
        }
    }
}