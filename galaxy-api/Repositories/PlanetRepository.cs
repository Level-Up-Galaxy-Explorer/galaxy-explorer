using galaxy_api.Models;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace galaxy_api.Repositories
{
    public class PlanetRepository : IPlanetRepository
    {
        private readonly string _connectionString;

        public PlanetRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<Planet>> GetAllPlanetsAsync()
        {
            var planets = new List<Planet>();
            var query = @"SELECT p.planet_id, p.name, g.name as galaxy, pt.name as planet_type, 
                          p.has_life, p.coordinates 
                          FROM planets p
                          JOIN galaxy g ON p.galaxy_id = g.galaxy_id
                          JOIN planet_type pt ON p.planet_type_id = pt.planet_type_id";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(query, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                planets.Add(new Planet
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Galaxy = reader.GetString(2),
                    PlanetType = reader.GetString(3),
                    HasLife = reader.GetBoolean(4),
                    Coordinates = reader.GetString(5)
                });
            }

            return planets;
        }

        public async Task<Planet?> GetPlanetByIdAsync(int id)
        {
            var query = @"SELECT p.planet_id, p.name, g.name as galaxy, pt.name as planet_type, 
                          p.has_life, p.coordinates 
                          FROM planets p
                          JOIN galaxy g ON p.galaxy_id = g.galaxy_id
                          JOIN planet_type pt ON p.planet_type_id = pt.planet_type_id
                          WHERE p.planet_id = @planetId";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@planetId", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Planet
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Galaxy = reader.GetString(2),
                    PlanetType = reader.GetString(3),
                    HasLife = reader.GetBoolean(4),
                    Coordinates = reader.GetString(5)
                };
            }

            return null;
        }

        public async Task<IEnumerable<Planet>> SearchPlanetsAsync(string name)
        {
            var planets = new List<Planet>();
            var query = @"SELECT p.planet_id, p.name, g.name as galaxy, pt.name as planet_type, 
                          p.has_life, p.coordinates 
                          FROM planets p
                          JOIN galaxy g ON p.galaxy_id = g.galaxy_id
                          JOIN planet_type pt ON p.planet_type_id = pt.planet_type_id
                          WHERE LOWER(p.name) LIKE LOWER(@name)";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", $"%{name}%");

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                planets.Add(new Planet
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Galaxy = reader.GetString(2),
                    PlanetType = reader.GetString(3),
                    HasLife = reader.GetBoolean(4),
                    Coordinates = reader.GetString(5)
                });
            }

            return planets;
        }

        public async Task<Planet> AddPlanetAsync(Planet planet)
        {
            var query = @"INSERT INTO planets (name, galaxy_id, planet_type_id, has_life, coordinates)
                          VALUES (@name, 
                                 (SELECT galaxy_id FROM galaxy WHERE name = @galaxy),
                                 (SELECT planet_type_id FROM planet_type WHERE name = @planetType),
                                 @hasLife, @coordinates)
                          RETURNING planet_id, name, 
                                   (SELECT name FROM galaxy WHERE galaxy_id = planets.galaxy_id) as galaxy,
                                   (SELECT name FROM planet_type WHERE planet_type_id = planets.planet_type_id) as planet_type,
                                   has_life,
                                   coordinates";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", planet.Name);
            cmd.Parameters.AddWithValue("@galaxy", planet.Galaxy);
            cmd.Parameters.AddWithValue("@planetType", planet.PlanetType);
            cmd.Parameters.AddWithValue("@hasLife", planet.HasLife);
            cmd.Parameters.AddWithValue("@coordinates", planet.Coordinates);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Planet
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Galaxy = reader.GetString(2),
                    PlanetType = reader.GetString(3),
                    HasLife = reader.GetBoolean(4),
                    Coordinates = reader.GetString(5)
                };
            }

            throw new Exception("Failed to create the planet.");
        }

        public async Task<bool> UpdatePlanetAsync(int planetId, Planet planet)
        {
            var setClause = new List<string>();
            var parameters = new List<NpgsqlParameter>();

            if (!string.IsNullOrEmpty(planet.Name))
            {
                setClause.Add("name = @name");
                parameters.Add(new NpgsqlParameter("@name", planet.Name));
            }

            if (!string.IsNullOrEmpty(planet.Galaxy))
            {
                setClause.Add("galaxy_id = (SELECT galaxy_id FROM galaxy WHERE name = @galaxy)");
                parameters.Add(new NpgsqlParameter("@galaxy", planet.Galaxy));
            }

            if (!string.IsNullOrEmpty(planet.PlanetType))
            {
                setClause.Add("planet_type_id = (SELECT planet_type_id FROM planet_type WHERE name = @planetType)");
                parameters.Add(new NpgsqlParameter("@planetType", planet.PlanetType));
            }

            setClause.Add("has_life = @hasLife");
            parameters.Add(new NpgsqlParameter("@hasLife", planet.HasLife));

            if (!string.IsNullOrEmpty(planet.Coordinates))
            {
                setClause.Add("coordinates = @coordinates");
                parameters.Add(new NpgsqlParameter("@coordinates", planet.Coordinates));
            }

            var query = $@"UPDATE planets
                           SET {string.Join(", ", setClause)}
                           WHERE planet_id = @planetId";

            parameters.Add(new NpgsqlParameter("@planetId", planetId));

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddRange(parameters.ToArray());

            var affectedRows = await cmd.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }
    }
}