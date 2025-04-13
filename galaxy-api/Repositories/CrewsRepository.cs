using Dapper;
using galaxy_api.DTOs;
using Npgsql;


namespace galaxy_api.Repositories;

public class CrewsRepository : ICrewsRepositoty
{

    private readonly string _connectionString;

    public CrewsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<IEnumerable<CrewDTO>> GetAllCrewsAsync()
    {

        string query = GetCrewsSQL();
        var crews = await CrewsTaskQueryAsync(query);
        return crews;
    }

    public async Task<CrewDTO?> GetCrewAsync(int crew_id)
    {
        string query = GetCrewsSQL(true);
        var crews = await CrewsTaskQueryAsync(query, new { Id = crew_id });
        return crews.FirstOrDefault();
    }


    private string GetCrewsSQL(bool getSingle = false)
    {
        var query = @"
                SELECT c.*, uc.*, u.*, r.*
                FROM crew c
                LEFT JOIN user_crew uc ON uc.crew_id = c.crew_id
                LEFT JOIN users u ON uc.user_id = u.user_id
                LEFT JOIN rank r on u.rank_id = r.rank_id";

        if (getSingle)
        {
            query += " WHERE c.crew_id = @Id";
        }

        return query;
    }

    private async Task<IEnumerable<CrewDTO>> CrewsTaskQueryAsync(string query, object? parameters = null)
    {
        await using var conn = new NpgsqlConnection(_connectionString);

        var crewDictionary = new Dictionary<int, CrewDTO>();

        await conn.QueryAsync<CrewDTO, UserDTO, RankDTO, CrewDTO>(query,
        (crew, user, rank) =>
        {

            if (!crewDictionary.TryGetValue(crew.Crew_Id, out var currentCreew))
            {
                currentCreew = crew;
                currentCreew.Members = [];
                crewDictionary.Add(crew.Crew_Id, currentCreew);
            }

            if (rank != null)
            {
                user.Rank = rank;
            }

            if (user is not null && !currentCreew.Members.Any(m => m.User_Id == user.User_Id))
            {

                currentCreew.Members.Add(user);

            }

            return currentCreew;

        },
        parameters,
        splitOn: "User_Id, Rank_Id"
        );

        return crewDictionary.Values;
    }

}