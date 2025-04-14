using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly string _connectionString;

    public DatabaseHealthCheck(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync(cancellationToken);

            var query = @"SELECT 1";

            await using var cmd = new NpgsqlCommand(query, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                reader.GetInt32(0);
                return HealthCheckResult.Healthy("Database is healthy.");
            }
            else
            {
                throw new InvalidDataException();
            }

        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Failed to connect to the database.", ex);
        }
    }
}