using System.Data;
using System.Text;
using Dapper;
using ErrorOr;
using galaxy_api.DTOs;
using galaxy_api.DTOs.Crews;
using galaxy_api.Errors;
using galaxy_api.Models;
using Npgsql;


namespace galaxy_api.Repositories;

public class CrewsRepository : ICrewsRepositoty
{

    private readonly string _connectionString;
    private static readonly List<int> ActiveMissionStatusIds = new List<int> { 2 };

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

    public async Task<ErrorOr<Crew>> CreateCrew(CreateCrewDto crewDto)
    {
        const string insertCrewSql = @"
        INSERT INTO crew (name, is_available)
        VALUES (@Name, @IsAvailable)
        RETURNING crew_id;";


        const string insertUserCrewSql = @"
        INSERT INTO user_crew (crew_id, user_id)
        VALUES (@CrewId, @UserId);";

        await using var conn = new NpgsqlConnection(_connectionString);

        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            var newCrewId = await conn.ExecuteScalarAsync<int>(insertCrewSql, new { crewDto.Name, crewDto.IsAvailable }, transaction: transaction);

            if (newCrewId <= 0)
            {
                await transaction.RollbackAsync();
                return CrewErrors.CreationFailed;
            }

            if (crewDto.MemberUserIds != null && crewDto.MemberUserIds.Any())
            {

                var userCrewParams = crewDto.MemberUserIds
                    .Distinct()
                    .Select(userId => new { CrewId = newCrewId, UserId = userId })
                    .ToList();

                if (userCrewParams.Any())
                {
                    int membersAffected = await conn.ExecuteAsync(
                        insertUserCrewSql,
                        userCrewParams,
                        transaction: transaction);

                    if (membersAffected != userCrewParams.Count)
                    {
                        await transaction.RollbackAsync();
                        return CrewErrors.MemberAssignmentFailed;
                    }
                }

            }

            await transaction.CommitAsync();

            var createdCrew = new Crew
            {
                CrewId = newCrewId,
                Name = crewDto.Name,
                IsAvailable = crewDto.IsAvailable
            };
            return createdCrew;
        }
        catch (PostgresException ex)
        {

            await transaction.RollbackAsync();
            return DomainErrros.Database.Unexpected(ex.Message, ex.SqlState);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"UNEXPECTED ERROR: {ex}");
            return Error.Unexpected("CrewService.CreateFailed", $"An unexpected error occurred: {ex.Message}");
        }
    }


    private async Task<ErrorOr<Success>> PerformPreUpdateChecksAsync(int crewId, IDbConnection connection, IDbTransaction transaction)
    {
        var exists = await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS (SELECT 1 FROM crew WHERE crew_id = @CrewId);",
            new { CrewId = crewId },
            transaction: transaction);

        if (!exists)
        {
            return DomainErrros.NotFound.Resource("Crew", crewId);
        }

        const string checkActiveMissionSql = @"
                SELECT EXISTS (
                    SELECT 1 FROM mission_crew mc JOIN missions m ON mc.mission_id = m.mission_id
                    WHERE mc.crew_id = @CrewId AND m.status_id = ANY(@ActiveStatusIds)
                );";
        var isOnActiveMission = await connection.ExecuteScalarAsync<bool>(
            checkActiveMissionSql,
            new { CrewId = crewId, ActiveStatusIds = ActiveMissionStatusIds },
            transaction: transaction);

        if (isOnActiveMission)
        {
            return CrewErrors.OnActiveMission(crewId);
        }

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> UpdateCrewDetailsAsync(int crewId, UpdateCrewDetailsDTO dto)
    {
        if (dto.Name == null && !dto.IsAvailable.HasValue)
        {
            return Result.Success;
        }

        var updateSqlBuilder = new StringBuilder("UPDATE crew SET ");
        var parameters = new DynamicParameters();
        parameters.Add("@CrewId", crewId);

        if (dto.Name != null)
        {
            updateSqlBuilder.Append("name = @Name, ");
            parameters.Add("@Name", dto.Name);
        }
        if (dto.IsAvailable.HasValue)
        {
            updateSqlBuilder.Append("is_available = @IsAvailable, ");
            parameters.Add("@IsAvailable", dto.IsAvailable.Value);
        }
        updateSqlBuilder.Length -= 2;
        updateSqlBuilder.Append(" WHERE crew_id = @CrewId;");

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            var checksResult = await PerformPreUpdateChecksAsync(crewId, conn, transaction);
            if (checksResult.IsError)
            {
                return checksResult.Errors;
            }

            int affectedRows = await conn.ExecuteAsync(
                updateSqlBuilder.ToString(),
                parameters,
                transaction: transaction);

            if (affectedRows == 0)
            {

                await transaction.RollbackAsync();
                return DomainErrros.NotFound.Resource("Crew", crewId);
            }

            await transaction.CommitAsync();
            return Result.Success;
        }
        catch (PostgresException ex)
        {

            await transaction.RollbackAsync();
            if (ex.SqlState == "23505" && (ex.ConstraintName?.Contains("crew_name") ?? false))
            {
                return CrewErrors.NameConflict(dto.Name ?? "(unknown)", ex.ConstraintName);
            }

            Console.WriteLine($"DATABASE ERROR: {ex}");
            return DomainErrros.Database.Unexpected(ex.Message, ex.SqlState);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"UNEXPECTED ERROR: {ex}");
            return DomainErrros.General.Unexpected($"An unexpected error occurred while updating crew details: {ex.Message}");
        }

    }

    public async Task<ErrorOr<Success>> AddCrewMembersAsync(int crewId, UpdateCrewMembersDto dto)
    {
        var distinctNewIds = dto.UserIds?.Distinct().ToList();
        if (distinctNewIds == null || !distinctNewIds.Any())
        {
            return Result.Success;
        }

        const string addMembersSql = @"
                INSERT INTO user_crew (crew_id, user_id) VALUES (@CrewId, @UserId);";

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            var checksResult = await PerformPreUpdateChecksAsync(crewId, conn, transaction);
            if (checksResult.IsError)
            {
                return checksResult.Errors;
            }

            var newMemberParams = distinctNewIds
                .Select(userId => new { CrewId = crewId, UserId = userId })
                .ToList();

            await conn.ExecuteAsync(addMembersSql, newMemberParams, transaction: transaction);

            await transaction.CommitAsync();
            return Result.Success;
        }
        catch (PostgresException ex)
        {

            await transaction.RollbackAsync();
            int failedUserId = TryParseUserIdFromError(ex.Detail);
            if (ex.SqlState == "23503") { return CrewErrors.InvalidUserId(failedUserId, ex.ConstraintName); }
            if (ex.SqlState == "23505") { return CrewErrors.UserAlreadyAssigned(failedUserId, ex.ConstraintName); }
            Console.WriteLine($"DATABASE ERROR: {ex}");
            return DomainErrros.Database.Unexpected(ex.Message, ex.SqlState);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"UNEXPECTED ERROR: {ex}");
            return DomainErrros.General.Unexpected($"An unexpected error occurred while adding crew members: {ex.Message}");
        }

    }

    private int TryParseUserIdFromError(string? detail)
    {
        try
        {
            if (detail != null && detail.Contains("(user_id)=("))
            {
                var parts = detail.Split(new[] { "(user_id)=(" }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    var idPart = parts[1].Split(')')[0];
                    if (int.TryParse(idPart, out int userId)) return userId;
                }
            }
        }
        catch { }
        return 0;
    }

    public async Task<ErrorOr<Success>> RemoveCrewMembersAsync(int crewId, UpdateCrewMembersDto dto)
    {

        var distinctRemovedIds = dto.UserIds?.Distinct().ToList();
        if (distinctRemovedIds == null || !distinctRemovedIds.Any())
        {
            return Result.Success;
        }

        const string removeMembersSql = @"
                DELETE FROM user_crew WHERE crew_id = @CrewId AND user_id = ANY(@UserIdsToRemove);";

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            var checksResult = await PerformPreUpdateChecksAsync(crewId, conn, transaction);
            if (checksResult.IsError)
            {
                return checksResult.Errors;
            }

            await conn.ExecuteAsync(removeMembersSql,
                new { CrewId = crewId, UserIdsToRemove = distinctRemovedIds },
                transaction: transaction);

            await transaction.CommitAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"UNEXPECTED ERROR: {ex}");
            return DomainErrros.General.Unexpected($"An unexpected error occurred while removing crew members: {ex.Message}");
        }
    }
}