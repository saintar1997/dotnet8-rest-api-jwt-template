using Dotnet8RestApiJwtTemplate.Api.Repositories.HealthCheckRepository;

namespace Dotnet8RestApiJwtTemplate.Api.Services.HealthCheckService;

public class HealthCheckService(IHealthCheckRepository repo) : IHealthCheckService
{
    public Task HealthCheck() => repo.CanConnectAsync();
}
