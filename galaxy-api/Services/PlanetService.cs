using galaxy_api.DTOs;
using galaxy_api.Repositories;
using System;

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
            return await _repository.GetAllPlanetsAsync();
        }
    }
}
