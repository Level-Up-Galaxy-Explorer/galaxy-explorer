using galaxy_api.DTOs;
using Npgsql;

namespace galaxy_api.Repositories
{
    public class PlanetRepository : IPlanetRepository
    {
        private readonly string _connectionString;

        public PlanetRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<PlanetDTO>> GetAllPlanetsAsync()
        {
            var planets = new List<PlanetDTO>();

            var query = @"
                SELECT 
                    p.name AS name,
                    g.name AS galaxy,
                    pt.name AS planet_type,
                    p.has_life,
                    p.coordinates
                FROM planets p
                JOIN galaxy g ON p.galaxy_id = g.galaxy_id
                JOIN planet_type pt ON p.planet_type_id = pt.planet_type_id";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(query, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                planets.Add(new PlanetDTO
                {
                    Name = reader.GetString(0),
                    Galaxy = reader.GetString(1),
                    PlanetType = reader.GetString(2),
                    HasLife = reader.GetBoolean(3),
                    Coordinates = reader.GetString(4)
                });
            }

            return planets;
        }
    }
}
