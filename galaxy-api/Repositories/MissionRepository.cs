using Dapper;
using galaxy_api.DTOs;
using galaxy_api.Models;
using Npgsql;

namespace galaxy_api.Repositories
{
    public class MissionRepository : IMissionRepository
    {
        private readonly string _connectionString;

        public MissionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<Missions>> GetAllMissionsAsync()
        {
            const string query = @"
                SELECT 
                    m.mission_id,
                    m.name,
                    m.mission_type_id,
                    m.launch_date,
                    m.destination_planet_id,
                    m.status_id,
                    m.reward_credit,
                    m.feedback,
                    u.full_name AS created_by
                FROM missions m
                JOIN users u on m.created_by = u.full_name";

            await using var conn = new NpgsqlConnection(_connectionString);
            var missions = await conn.QueryAsync<Missions, Users, Missions>(
            query,
            (mission, user) =>
            {
                mission.Created_By = user.Full_Name;
                return mission;
            },
            splitOn: "created_by"
            );

            return missions;
        }
        
        public async Task<Missions?> GetMissionByIdAsync(int id)
        {
            const string query = @"
        SELECT 
            m.mission_id,
            m.name,
            m.mission_type_id,
            m.launch_date,
            m.destination_planet_id,
            m.status_id,
            m.reward_credit,
            m.feedback,
            u.full_name AS created_by
        FROM missions m
        JOIN users u ON m.created_by = u.full_name
        WHERE m.mission_id = @Id";

    await using var conn = new NpgsqlConnection(_connectionString);

    var result = await conn.QueryAsync<Missions, Users, Missions>(
        query,
        (mission, user) =>
        {
            mission.Created_By = user.Full_Name;
            return mission;
        },
        new { id },
        splitOn: "created_by"
        );

            return result.FirstOrDefault();
        }   
        public async Task CreateMissionAsync(Missions missions)
        {
            const string query = @"
                INSERT INTO missions (name, mission_type_id, launch_date, destination_planet_id, status_id, created_by)
                VALUES (@Name, @Mission_Type_Id, @Launch_Date, @Destination_Planet_Id, @Status_Id, @Created_By)";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(query, missions);
        }

        public async Task UpdateMissionDetailsAsync(int id, Missions missions)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            
            const string selectQuery = @"
                SELECT mission_id, name, mission_type_id, launch_date, destination_planet_id
                FROM missions
                WHERE mission_id = @Id";

            var existingMission = await conn.QuerySingleOrDefaultAsync<Missions>(selectQuery, new { Id = id });

            if (existingMission == null)
            throw new Exception($"Mission with ID {id} not found.");

            var updatedMission = new Missions
            {
                Mission_Id = id,
                Name = missions.Name ?? existingMission.Name,
                Mission_Type_Id = missions.Mission_Type_Id != 0 ? missions.Mission_Type_Id : existingMission.Mission_Type_Id,
                Launch_Date = missions.Launch_Date != default ? missions.Launch_Date : existingMission.Launch_Date,
                Destination_Planet_Id = missions.Destination_Planet_Id != 0 ? missions.Destination_Planet_Id : existingMission.Destination_Planet_Id
            };

            const string updateQuery = @"
                UPDATE missions
                SET 
                    name = @Name,
                    mission_type_id = @Mission_Type_Id,
                    launch_date = @Launch_Date,
                    destination_planet_id = @Destination_Planet_Id
                WHERE mission_id = @Mission_Id";

            await conn.ExecuteAsync(updateQuery, updatedMission);
        }
        public async Task ProvideMissionFeedbackAsync(int id, Missions missions)
        {
            const string query = @"
                UPDATE missions
                SET 
                    feedback = @Feedback
                WHERE mission_id = @Id";

            var parameters = new
            {
                Id = id,
                missions.Feedback
            };

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(query, parameters);
        }

        public async Task UpdateMissionStatusAsync(int id, Missions missions)
        {
            const string query = @"
                UPDATE missions
                SET 
                    status_id = @Status_Id
                WHERE mission_id = @Id";

            var parameters = new
            {
                Id = id,
                missions.Status_Id
            };

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(query, parameters);
        }

        public async Task RewardCreditMissionAsync(int id, Missions missions)
        {
            const string query = @"
                UPDATE missions
                SET 
                    reward_credit = @Reward_Credit
                WHERE mission_id = @Id";

            var parameters = new
            {
                Id = id,
                missions.Reward_Credit
            };

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteAsync(query, parameters);
        }
        public async Task<IEnumerable<MissionStatusReport>> GetMissionStatusReportAsync(string? missionType, string? status, string? groupBy)
        {
            var filterClauses = new List<string>();
            if (!string.IsNullOrWhiteSpace(missionType))
            filterClauses.Add("mt.name ILIKE @MissionType");
            if (!string.IsNullOrWhiteSpace(status))
            filterClauses.Add("s.name ILIKE @Status");

            var whereClause = filterClauses.Any()
            ? "WHERE " + string.Join(" AND ", filterClauses)
            : string.Empty;

            string periodSelect = string.Empty;
            string periodGroup = string.Empty;

            if (groupBy?.ToLower() == "month")
            {
                periodSelect = "TO_CHAR(m.launch_date, 'YYYY-MM') AS Period,";
                periodGroup = "TO_CHAR(m.launch_date, 'YYYY-MM'),";
            }
            else if (groupBy?.ToLower() == "quarter")
            {
                periodSelect = "CONCAT('Q', EXTRACT(QUARTER FROM m.launch_date), '-', EXTRACT(YEAR FROM m.launch_date)) AS Period,";
                periodGroup = "EXTRACT(QUARTER FROM m.launch_date), EXTRACT(YEAR FROM m.launch_date),";
            }

            var query = $@"
    SELECT 
        mt.name AS MissionType,
        s.name AS Status,
        COUNT(*) AS Count
        {(string.IsNullOrEmpty(periodSelect) ? "" : ", " + periodSelect.TrimEnd(','))},
        p.name AS Planet
    FROM missions m
    JOIN mission_type mt ON m.mission_type_id = mt.mission_type_id
    JOIN status s ON m.status_id = s.status_id
    JOIN planets p ON m.destination_planet_id = p.planet_id
    {whereClause}
    GROUP BY {periodGroup} mt.name, s.name, p.name
    ORDER BY mt.name, s.name, p.name";


            var parameters = new
            {
                MissionType = missionType,
                Status = status
            };

            await using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryAsync<MissionStatusReport>(query, parameters);
        }
    }
}