using Dapper;
using galaxy_api.Models;
using Npgsql;

namespace galaxy_api.Repositories
{
    public class RankRepository : IRankRepository
    {
        private readonly string _connectionString;

        public RankRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<Rank>> GetAllRanksAsync()
        {
            const string query = @"
                SELECT 
                    u.rank_id,
                    u.title,
                    u.description
                FROM Rank u";

            await using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryAsync<Rank>(query);
        }
        public async Task<Rank?> GetRankByIdAsync(int id)
        {
            const string query = @"
                SELECT 
                    u.rank_id,
                    u.title,
                    u.description
                FROM Rank u
                WHERE u.rank_id = @id";

            await using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<Rank>(query, new { id });
        }

        public async Task AddRankAsync(Rank rank)
        {
            const string query = @"
            INSERT INTO Ranks (title, description)
            VALUES (@Title, @Description)";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(query, rank);
        }

        public async Task UpdateRankDetailsAsync(int id, Rank rank)
        {
            const string query = @"
                UPDATE Ranks
                SET 
                    description = @Description
                WHERE rank_id = @Id";

            var parameters = new
            {
                Id = id,
                rank.Description
            };

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(query, parameters);
        }

    }
}