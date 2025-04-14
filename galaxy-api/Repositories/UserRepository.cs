using Dapper;
using galaxy_api.Models;
using Npgsql;

namespace galaxy_api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<Users>> GetAllUserAsync()
        {
            const string query = @"
                SELECT 
                    u.user_id,
                    u.full_name,
                    u.email_address,
                    u.google_id,
                    u.rank_id,
                    u.is_active,
                    u.created_at
                FROM users u";

            await using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryAsync<Users>(query);
        }
        public async Task<Users?> GetUserByIdAsync(int id)
        {
            const string query = @"
                SELECT 
                    u.user_id,
                    u.full_name,
                    u.email_address,
                    u.google_id,
                    u.rank_id,
                    u.is_active,
                    u.created_at
                FROM users u
                WHERE u.user_id = @id";

            await using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<Users>(query, new { id });
        }
        public async Task<Users?> GetUserByGoogleIdAsync(string id)
        {
            const string query = @"
                SELECT 
                    u.user_id,
                    u.full_name,
                    u.email_address,
                    u.google_id,
                    u.rank_id,
                    u.is_active,
                    u.created_at
                FROM users u
                WHERE u.google_id = @id";

            await using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<Users>(query, new { id });
        }
        public async Task AddUserAsync(Users users)
        {
            const string query = @"
            INSERT INTO users (full_name, email_address, google_id, rank_id, is_active, created_at)
            VALUES (@Full_Name, @Email_Address, @Google_Id, @Rank_Id, true, NOW())";


            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(query, users);
        }

        public async Task UpdateUserDetailsAsync(int id, Users users)
        {
            const string query = @"
                UPDATE users
                SET 
                    full_name = @Full_Name,
                WHERE user_id = @id";

            var parameters = new
            {
                Id = id,
                users.Full_Name,
                users.Email_Address
            };

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(query, parameters);
        }
        public async Task AssignUserAsync(int id, Users users)
        {
            const string query = @"
                UPDATE users
                SET 
                    rank_id = @Rank_Id
                WHERE user_id = @Id";

            var parameters = new
            {
                Id = id,
                users.Rank_Id
            };

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(query, parameters);
        }

        public async Task DeactivateUserAsync(int id, Users users)
        {
            const string query = @"
                UPDATE users
                SET 
                    is_active = @Is_Active
                WHERE user_id = @Id";

            var parameters = new
            {
                Id = id,
                users.Is_Active
            };

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(query, parameters);
        }

    }
}