using Dapper;
using galaxy_api.Models;
using Npgsql;

namespace galaxy_api.Repositories
{
    public class GalaxyRepository : IGalaxyRepository
    {
        private readonly string _connectionString;

        public GalaxyRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<Galaxy>> GetAllGalaxyAsync()
        {
            const string query = @"
                SELECT 
                    g.galaxy_id,
                    g.name,
                    gt.name as galaxy_type,
                    g.distance_from_earth,
                    g.description
                FROM galaxy g
                JOIN galaxy_type gt ON gt.galaxy_type_id = g.galaxy_type_id;";

            await using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryAsync<Galaxy>(query);
        }

        public async Task<Galaxy?> GetGalaxyByIdAsync(int id)
        {
            const string query = @"
                SELECT 
                    g.galaxy_id,
                    g.name,
                    gt.name as galaxy_type,
                    g.distance_from_earth,
                    g.description
                FROM galaxy g
                JOIN galaxy_type gt ON gt.galaxy_type_id = g.galaxy_type_id
                WHERE g.galaxy_id = @Id";

            await using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<Galaxy>(query, new { id });
        }

        public async Task AddGalaxyAsync(Galaxy galaxy)
        {
            const string query = @"
                INSERT INTO galaxy (
                    name, 
                    galaxy_type_id, 
                    distance_from_earth, 
                    description)
                VALUES (
                    @Name,
                    (SELECT galaxy_type_id FROM galaxy_type WHERE name = @Galaxy_Type),
                    @Distance_From_Earth,
                    @Description)";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(query, galaxy);
        }

        public async Task UpdateGalaxyAsync(int id, Galaxy galaxy)
        {
            const string query = @"
                UPDATE galaxy
                SET
                    name = @Name,
                    galaxy_type_id = (SELECT galaxy_type_id FROM galaxy_type WHERE name = @Galaxy_Type),
                    distance_from_earth = @Distance_From_Earth,
                    description = @Description
                WHERE galaxy_id = @Id";

            var parameters = new
            {
                Id = id,
                galaxy.Name,
                galaxy.Galaxy_Type,
                galaxy.Distance_From_Earth,
                galaxy.Description
            };

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(query, parameters);
        }

        public async Task<IEnumerable<string>> GetGalaxyTypesAsync()
        {
            const string query = "SELECT name FROM galaxy_type";

            await using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryAsync<string>(query);
        }
    }
}
