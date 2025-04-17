using System.Data.Common;
using System.Text;
using Dapper;
using ErrorOr;
using galaxy_api.DTOs;
using galaxy_api.DTOs.Missions;
using galaxy_api.Errors;
using galaxy_api.Models;
using Npgsql;

namespace galaxy_api.Repositories
{
    public class MissionRepository : IMissionRepository
    {
        private readonly string _connectionString;

        private const int PlannedStatusId = 1;
        private const int Active = 2;
        private const int CompletedStatusId = 3;
        private const int FailedStatusId = 4;
        private const int AbortedStatusId = 5;

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

        public async Task<ErrorOr<MissionDetailsWithCrewHistoryDTO>> GetMissionDetailsWithCrewHistoryAsync(int missionId)
        {
            const string query = @"
            SELECT
                m.name AS Name,
                mt.name AS MissionTypeName,
                m.launch_date AS LaunchDate,
                p.name AS DestinationPlanetName,
                m.reward_credit AS RewardCredit,
                m.feedback AS Feedback,
                s_mission.name AS OverallMissionStatusName,
                u_creator.full_name AS CreatedByFullName 
            FROM Missions m
            JOIN Mission_Type mt ON m.mission_type_id = mt.mission_type_id
            JOIN Planets p ON m.destination_planet_id = p.planet_id
            JOIN Status s_mission ON m.status_id = s_mission.status_id
            LEFT JOIN Users u_creator ON m.created_by = u_creator.user_id
            WHERE m.mission_id = @MissionId;

            SELECT
                c.name AS CrewName,
                c.is_available AS CrewIsAvailable,
                mc.assigned_at AS AssignedAt,
                mc.ended_at AS EndedAt,
                s_crew.name AS AssignmentStatusName
            FROM Mission_Crew mc
            JOIN Crew c ON mc.crew_id = c.crew_id
            JOIN Status s_crew ON mc.mission_status_id = s_crew.status_id
            WHERE mc.mission_id = @MissionId
            ORDER BY mc.assigned_at ASC;
            ";

            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);

                using var multi = await conn.QueryMultipleAsync(query, new { MissionId = missionId });
                var missionDetails = await multi.ReadFirstOrDefaultAsync<MissionDetailsWithCrewHistoryDTO>();

                if (missionDetails == null)
                {
                    return DomainErrros.NotFound.Resource("Mission", new { });
                }

                var crewHistory = (await multi.ReadAsync<MissionCrewHistoryItemDTO>()).ToList();

                missionDetails.CrewHistory = crewHistory;

                return missionDetails;
            }
            catch (NpgsqlException ex)
            {
                return Error.Unexpected("CrewService.CreateFailed", $"An unexpected error occurred: {ex.Message}");
            }
            catch (DbException ex)
            {
                return Error.Unexpected("CrewService.CreateFailed", $"An unexpected error occurred: {ex.Message}");
            }
            catch (TimeoutException ex)
            {
                return Error.Unexpected("CrewService.CreateFailed", $"An unexpected error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                return DomainErrros.Database.Unexpected(ex.Message);
            }
        }

        async Task<ErrorOr<int>> CrewPreChecks(int missionId, int crewId)
        {

            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);

                const string missionCheckSql = "SELECT 1 FROM Missions WHERE mission_id = @MissionId LIMIT 1;";
                var missionExists = await conn.ExecuteScalarAsync<int?>(missionCheckSql, new { MissionId = missionId });
                if (!missionExists.HasValue)
                {
                    return DomainErrros.NotFound.Resource("Mission", new { });
                }

                const string crewCheckSql = "SELECT is_available FROM Crew WHERE crew_id = @CrewId LIMIT 1;";
                var crewAvailability = await conn.QuerySingleOrDefaultAsync<bool?>(crewCheckSql, new { CrewId = crewId });

                if (crewAvailability == null)
                {
                    return CrewErrors.NotFound;
                }

                if (!crewAvailability.Value)
                {
                    return CrewErrors.NotAvailable;
                }

                const string activeAssignmentCheckSql = @"
                SELECT 1 FROM Mission_Crew
                WHERE mission_id = @MissionId AND crew_id = @CrewId AND ended_at IS NULL
                LIMIT 1;";


                var existingActive = await conn.ExecuteScalarAsync<int?>(activeAssignmentCheckSql, new { MissionId = missionId, CrewId = crewId });
                if (existingActive.HasValue)
                {
                    return DomainErrros.Assignment.Conflict;
                }

                return 0;

            }
            catch (NpgsqlException)
            {
                return DomainErrros.Database.QueryFailure;
            }
            catch (
                DbException)
            {
                return DomainErrros.Database.QueryFailure;
            }
            catch (TimeoutException)
            {
                return DomainErrros.Database.Timeout;
            }
            catch (Exception ex)
            {
                return DomainErrros.Database.Unexpected(ex.Message);
            }
        }

        public async Task<ErrorOr<Success>> AssignCrewToMissionAsync(int missionId, int crewId)
        {
            var error = await CrewPreChecks(missionId, crewId);

            if (error.IsError)
            {
                return error.Errors;
            }

            const string insertSql = @"
            INSERT INTO Mission_Crew (mission_id, crew_id, assigned_at, ended_at, mission_status_id)
            VALUES (@MissionId, @CrewId, NOW(), NULL, @InitialStatusId);
            ";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var transaction = conn.BeginTransaction();


            try
            {

                var rowsAffected = await conn.ExecuteAsync(
                    insertSql,
                    new
                    {
                        MissionId = missionId,
                        CrewId = crewId,
                        InitialStatusId = 2
                    },
                    transaction
                );

                if (rowsAffected != 1)
                {
                    await transaction.RollbackAsync();
                    return DomainErrros.Database.Unexpected("No changes were made");
                }

                const string updateMissionSql = @"
                    UPDATE Missions
                    SET status_id = @NewStatusId
                    WHERE mission_id = @MissionId;
                    ";

                var rowsAffectedUpdate = await conn.ExecuteAsync(
                   updateMissionSql,
                   new
                   {
                       NewStatusId = 2,
                       MissionId = missionId,
                       ExpectedCurrentStatusId = 1
                   },
                   transaction
                );

                if (rowsAffectedUpdate != 1)
                {
                    await transaction.RollbackAsync();
                    return DomainErrros.Database.Unexpected("Unexpected");
                }

                await transaction.CommitAsync();

                return Result.Success;
            }
            catch (NpgsqlException npgEx)
            {
                await transaction.RollbackAsync();
                if (npgEx.SqlState == PostgresErrorCodes.UniqueViolation) return DomainErrros.Assignment.Conflict;
                if (npgEx.SqlState == PostgresErrorCodes.ForeignKeyViolation)
                {
                    return DomainErrros.Database.ConstraintViolation("");
                }

                return DomainErrros.Database.QueryFailure;
            }
            catch (DbException)
            {
                return DomainErrros.Database.QueryFailure;
            }
            catch (TimeoutException)
            {
                return DomainErrros.Database.Timeout;
            }
            catch (Exception ex)
            {
                
                return DomainErrros.Database.Unexpected(ex.Message);
            }

        }

        public async Task<ErrorOr<Success>> UpdateMissionsStatusAsync(int missionId, int crewId, MissionStatusUpdateDto missions)
        {

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var transaction = conn.BeginTransaction();

            bool assignmentIsEnded = false;
            bool requiresMissionUpdate = false;
            long targetMissionStatusId = 0;

            try
            {
                const string statusCheckSql = "SELECT 1 FROM Status WHERE status_id = @StatusId LIMIT 1;";
                var statusExists = await conn.ExecuteScalarAsync<int?>(statusCheckSql, new { StatusId = missions.Status_Id }, transaction);
                if (!statusExists.HasValue)
                {
                    await transaction.RollbackAsync();
                    return DomainErrros.Assignment.InvalidStatus;
                }

                const string assignmentCheckSql = @"
                SELECT ended_at IS NOT NULL AS IsEnded
                FROM Mission_Crew
                WHERE mission_id = @MissionId AND crew_id = @CrewId
                LIMIT 1 FOR UPDATE;";
                var currentState = await conn.QuerySingleOrDefaultAsync<bool?>(assignmentCheckSql,
                new { MissionId = missionId, CrewId = crewId }, transaction);

                if (currentState == null)
                {
                    await transaction.RollbackAsync();
                    return DomainErrros.Assignment.NotFound;
                }

                assignmentIsEnded = currentState.Value;
                if (assignmentIsEnded)
                {
                    await transaction.RollbackAsync();
                    return DomainErrros.Assignment.AlreadyEnded;
                }


                bool isTerminalStatus = missions.Status_Id == CompletedStatusId ||
                                         missions.Status_Id == FailedStatusId ||
                                         missions.Status_Id == AbortedStatusId;

                var setClauses = new StringBuilder("SET mission_status_id = @NewStatusId");
                var parameters = new DynamicParameters();
                parameters.Add("NewStatusId", missions.Status_Id);
                parameters.Add("MissionId", missionId);
                parameters.Add("CrewId", crewId);

                if (isTerminalStatus)
                {
                    setClauses.Append(", ended_at = NOW()");
                    setClauses.Append(", feedback = @Feedback");
                }
                string updateAssignmentSql = $@"
                UPDATE Mission_Crew
                {setClauses.ToString()}
                WHERE mission_id = @MissionId AND crew_id = @CrewId AND ended_at IS NULL;";

                var rowsAffectedAssignment = await conn.ExecuteAsync(
                    updateAssignmentSql,
                    new { NewStatusId = missions.Status_Id, MissionId = missionId, CrewId = crewId, feedback = missions.Feedback },
                    transaction
                );

                if (rowsAffectedAssignment != 1)
                {
                    await transaction.RollbackAsync();
                    return DomainErrros.Database.Unexpected("");
                }

                if (missions.Status_Id == CompletedStatusId)
                {
                    requiresMissionUpdate = true;
                    targetMissionStatusId = CompletedStatusId;

                }
                else if (missions.Status_Id == AbortedStatusId || missions.Status_Id == FailedStatusId)
                {
                    requiresMissionUpdate = true;
                    targetMissionStatusId = PlannedStatusId;
                }

                if (requiresMissionUpdate)
                {

                    const string updateMissionSql = @"
                         UPDATE Missions SET status_id = @NewStatusId
                         WHERE mission_id = @MissionId";



                    var rowsAffectedMission = await conn.ExecuteAsync(
                        updateMissionSql,
                        new
                        {
                            NewStatusId = targetMissionStatusId,
                            MissionId = missionId,
                        },
                        transaction
                    );

                    if (rowsAffectedMission != 1)
                    {
                        await transaction.RollbackAsync();
                        return DomainErrros.Database.Unexpected($"Failed to update Mission.");
                    }
                }

                await transaction.CommitAsync();
                return Result.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.ToString());
                try { await transaction.RollbackAsync(); } catch (Exception) { }

                if (ex is NpgsqlException npgEx)
                {
                    if (npgEx.SqlState == PostgresErrorCodes.ForeignKeyViolation) return DomainErrros.Assignment.InvalidStatus;

                    return DomainErrros.Database.QueryFailure;
                }
                if (ex is DbException) return DomainErrros.Database.QueryFailure;
                if (ex is TimeoutException) return DomainErrros.Database.Timeout;
                return DomainErrros.Database.Unexpected(ex.Message);
            }

        }
    }
}