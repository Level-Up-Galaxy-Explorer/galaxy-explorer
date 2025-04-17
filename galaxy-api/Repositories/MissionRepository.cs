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
                    m.name,
                    mt.name AS mission_type,
                    m.launch_date,
                    p.name AS planet_type,
                    s.name AS status_type,
                    m.reward_credit,
                    m.feedback,
                    u.full_name AS created_by_name
                FROM missions m
                JOIN mission_type mt ON mt.mission_type_id = m.mission_type_id
                JOIN planets p ON p.planet_id = m.destination_planet_id
                JOIN status s ON s.status_id = m.status_id
                JOIN users u ON u.user_id = m.created_by
                ORDER BY m.name";

            await using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryAsync<Missions>(query);
        }
        
        public async Task<Missions?> GetMissionByIdAsync(int id)
        {
            const string query = @"
                SELECT 
                    m.name,
                    mt.name AS mission_type,
                    m.launch_date,
                    p.name AS planet_type,
                    s.name AS status_type,
                    m.reward_credit,
                    m.feedback,
                    u.full_name AS created_by_name
                FROM missions m
                JOIN mission_type mt ON mt.mission_type_id = m.mission_type_id
                JOIN planets p ON p.planet_id = m.destination_planet_id
                JOIN status s ON s.status_id = m.status_id
                JOIN users u ON u.user_id = m.created_by
                WHERE m.mission_id = @Id";

            await using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<Missions>(query, new { id });
        }

        public async Task CreateMissionAsync(Missions missions)
        {
            const string query = @"
                INSERT INTO missions (
                    name, 
                    mission_type_id, 
                    launch_date, 
                    destination_planet_id, 
                    status_id, 
                    created_by)
                VALUES (
                    @Name, 
                    (SELECT mission_type_id FROM mission_type WHERE name = @Mission_Type), 
                    @Launch_Date, 
                    (SELECT planet_id FROM planets WHERE name = @Planet_Type),
                    (SELECT status_id FROM status WHERE name = @Status_Type),
                    (SELECT user_id FROM users WHERE full_name = @Created_By_Name))";

            await using var conn = new NpgsqlConnection(_connectionString);
            
            try 
            {
                await conn.ExecuteAsync(query, new
                {
                    missions.Name,
                    missions.Mission_Type,
                    missions.Launch_Date,
                    missions.Planet_Type,
                    missions.Status_Type,
                    missions.Created_By_Name 
                });
            }
            catch (PostgresException ex) when (ex.SqlState == "23503")
            {
                var errorMessage = ex.Message.ToLower() switch
                {
                    var msg when msg.Contains("mission_type") => 
                        $"Mission type '{missions.Mission_Type}' does not exist.",
                    var msg when msg.Contains("planet") => 
                        $"Planet '{missions.Planet_Type}' does not exist.",
                    var msg when msg.Contains("status") => 
                        $"Status '{missions.Status_Type}' does not exist.",
                    var msg when msg.Contains("user") => 
                        $"User '{missions.Created_By_Name}' does not exist.",
                    _ => "One or more referenced items do not exist."
                };
                throw new InvalidOperationException(errorMessage);
            }
        }

        public async Task UpdateMissionDetailsAsync(int id, Missions missions)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            
            const string selectQuery = @"
                SELECT 
                    m.mission_id,
                    m.name,
                    mt.name AS mission_type,
                    m.launch_date,
                    p.name AS planet_type,
                    m.mission_type_id,
                    m.destination_planet_id
                FROM missions m
                JOIN mission_type mt ON mt.mission_type_id = m.mission_type_id
                JOIN planets p ON p.planet_id = m.destination_planet_id
                WHERE m.mission_id = @Id";

            var existingMission = await conn.QuerySingleOrDefaultAsync<Missions>(selectQuery, new { Id = id });
            if (existingMission == null)
                throw new Exception($"Mission with ID {id} not found.");

            int missionTypeId;
            int planetId;

            if (!string.IsNullOrWhiteSpace(missions.Mission_Type))
            {
                const string verifyMissionTypeQuery = @"
                    SELECT mission_type_id FROM mission_type WHERE name = @Mission_Type";
                var newMissionTypeId = await conn.QuerySingleOrDefaultAsync<int?>(verifyMissionTypeQuery, new { missions.Mission_Type });
                if (newMissionTypeId == null)
                    throw new InvalidOperationException($"Mission type '{missions.Mission_Type}' does not exist.");
                missionTypeId = newMissionTypeId.Value;
            }
            else
            {
                missionTypeId = existingMission.Mission_Type_Id;
            }

            if (!string.IsNullOrWhiteSpace(missions.Planet_Type))
            {
                const string verifyPlanetQuery = @"
                    SELECT planet_id FROM planets WHERE name = @Planet_Type";
                var newPlanetId = await conn.QuerySingleOrDefaultAsync<int?>(verifyPlanetQuery, new { missions.Planet_Type });
                if (newPlanetId == null)
                    throw new InvalidOperationException($"Planet '{missions.Planet_Type}' does not exist.");
                planetId = newPlanetId.Value;
            }
            else
            {
                planetId = existingMission.Destination_Planet_Id;
            }

            const string updateQuery = @"
                UPDATE missions
                SET 
                    name = CASE WHEN @Name = '' THEN name ELSE @Name END,
                    mission_type_id = @MissionTypeId,
                    launch_date = @Launch_Date,
                    destination_planet_id = @PlanetId
                WHERE mission_id = @Id";

            try
            {
                await conn.ExecuteAsync(updateQuery, new
                {
                    Id = id,
                    missions.Name,
                    MissionTypeId = missionTypeId,
                    missions.Launch_Date,
                    PlanetId = planetId
                });
            }
            catch (PostgresException ex)
            {
                throw new InvalidOperationException($"Error updating mission: {ex.Message}");
            }
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
                    status_id = @Status_Id,
                    feedback = CASE 
                        WHEN @Status_Id IN (3, 4) THEN @Feedback 
                        ELSE feedback 
                    END,
                    reward_credit = CASE 
                        WHEN @Status_Id = 3 THEN @Reward_Credit 
                        ELSE reward_credit 
                    END
                WHERE mission_id = @Id
                RETURNING mission_id, name, 
                    (SELECT name FROM status WHERE status_id = @Status_Id) as status_type,
                    feedback, reward_credit";

            var parameters = new
            {
                Id = id,
                missions.Status_Id,
                missions.Feedback,
                missions.Reward_Credit
            };

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.QuerySingleOrDefaultAsync<Missions>(query, parameters);
        }

        public async Task<bool> ValidateStatusTransitionAsync(int missionId, int newStatusId)
        {
            const string query = @"
                SELECT s.name as status_type
                FROM missions m
                JOIN status s ON s.status_id = m.status_id
                WHERE m.mission_id = @MissionId";

            await using var conn = new NpgsqlConnection(_connectionString);
            var currentStatus = await conn.QuerySingleOrDefaultAsync<string>(query, new { MissionId = missionId });

            return (currentStatus, newStatusId) switch
            {
                ("Planning", 2) => true,      
                ("Ready", 3) => true,         
                ("Launched", 4) => true,     
                _ => false
            };
        }

        public async Task<IEnumerable<string>> GetAvailableStatusesAsync()
        {
            const string query = "SELECT name FROM status ORDER BY status_id";

            await using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QueryAsync<string>(query);
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