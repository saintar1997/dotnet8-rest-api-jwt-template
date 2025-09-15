using Dotnet8RestApiJwtTemplate.Api.Configs;

namespace Dotnet8RestApiJwtTemplate.Api.Repositories.HealthCheckRepository;

public class HealthCheckRepository : IHealthCheckRepository
{
    private readonly ISqlConnectionFactory _factory;
    public HealthCheckRepository(ISqlConnectionFactory factory) => _factory = factory;

    public async Task CanConnectAsync(CancellationToken ct = default)
    {
        await using var conn = await _factory.CreateAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT 1";
        cmd.CommandTimeout = 2; // วินาที
        _ = await cmd.ExecuteScalarAsync(ct); // throw ถ้าเชื่อมต่อไม่ได้
    }
}
