namespace Dotnet8RestApiJwtTemplate.Api.Repositories.HealthCheckRepository;

public interface IHealthCheckRepository
{
    Task CanConnectAsync(CancellationToken ct = default);
}
