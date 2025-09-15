using System.Threading;
using Moq;
using Dotnet8RestApiJwtTemplate.Api.Repositories.HealthCheckRepository;
using Dotnet8RestApiJwtTemplate.Api.Services.HealthCheckService;

namespace SSCDashboard.Test.Services;

public class HealthCheckServiceTests
{
    [Fact]
    public async Task HealthCheck_Calls_Repo()
    {
        // arrange
        var repo = new Mock<IHealthCheckRepository>(MockBehavior.Strict);
        repo.Setup(r => r.CanConnectAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var svc = new HealthCheckService(repo.Object);

        // act
        await svc.HealthCheck();

        // assert
        repo.Verify(r => r.CanConnectAsync(It.IsAny<CancellationToken>()), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HealthCheck_Propagates_Exception()
    {
        // arrange
        var repo = new Mock<IHealthCheckRepository>();
        repo.Setup(r => r.CanConnectAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.Exception("db fail"));

        var svc = new HealthCheckService(repo.Object);

        // act & assert
        await Assert.ThrowsAsync<System.Exception>(() => svc.HealthCheck());
    }
}
