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
                    g.galaxy_type_id,
                    g.distance_from_earth,
                    g.description
                FROM galaxy g";

            await using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryAsync<Galaxy>(query);
        }

        public async Task<Galaxy?> GetGalaxyByIdAsync(int id)
        {
            const string query = @"
                SELECT 
                    g.galaxy_id,
                    g.name,
                    g.galaxy_type_id,
                    g.distance_from_earth,
                    g.description
                FROM galaxy g
                WHERE g.galaxy_id = @id";

            await using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<Galaxy>(query, new { id });
        }

        public async Task AddGalaxyAsync(Galaxy galaxy)
        {
            const string query = @"
                INSERT INTO galaxy (name, galaxy_type_id, distance_from_earth, description)
                VALUES (@Name, @Galaxy_Type_Id, @Distance_From_Earth, @Description)";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(query, galaxy);
        }

        public async Task UpdateGalaxyAsync(int id, Galaxy galaxy)
        {
            const string query = @"
                UPDATE galaxy
                SET name = @Name,
                    galaxy_type_id = @Galaxy_Type_Id,
                    distance_from_earth = @Distance_From_Earth,
                    description = @Description
                WHERE galaxy_id = @Id";

            var parameters = new
            {
                Id = id,
                galaxy.Name,
                galaxy.Galaxy_Type_Id,
                galaxy.Distance_From_Earth,
                galaxy.Description
            };

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(query, parameters);
        }
    }
}
