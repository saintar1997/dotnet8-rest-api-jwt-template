using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Dotnet8RestApiJwtTemplate.Api.Controllers;
using Dotnet8RestApiJwtTemplate.Api.Services.HealthCheckService;
using Dotnet8RestApiJwtTemplate.Api.Models.HealthCheckModel;

namespace SSCDashboard.Test.Controllers;

public class HealthCheckControllerTests
{
    [Fact]
    public void Alive_Returns_Ok_Healthy()
    {
        // arrange
        var svc = new Mock<IHealthCheckService>();
        var logger = new Mock<ILogger<HealthCheckController>>();
        var controller = new HealthCheckController(svc.Object, logger.Object);

        // act
        var result = controller.Alive();

        // assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<HealthCheckResponse>(ok.Value);
        Assert.Equal("Healthy", body.Status);
    }

    [Fact]
    public async Task Health_Returns_200_When_Service_Ok()
    {
        // arrange
        var svc = new Mock<IHealthCheckService>(MockBehavior.Strict);
        svc.Setup(s => s.HealthCheck()).Returns(Task.CompletedTask);
        var logger = new Mock<ILogger<HealthCheckController>>();
        var controller = new HealthCheckController(svc.Object, logger.Object);

        // act
        var result = await controller.Health();

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<HealthCheckResponse>(ok.Value);
        Assert.Equal("Healthy", body.Status);
        svc.Verify(s => s.HealthCheck(), Times.Once);
        svc.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Health_Returns_500_And_Logs_When_Service_Throws()
    {
        // arrange
        var svc = new Mock<IHealthCheckService>();
        svc.Setup(s => s.HealthCheck()).ThrowsAsync(new InvalidOperationException("db down"));
        var logger = new Mock<ILogger<HealthCheckController>>();
        var controller = new HealthCheckController(svc.Object, logger.Object);

        // act
        var result = await controller.Health();

        // assert
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);

        logger.VerifyLogErrorCalled(Times.AtLeastOnce());
    }
}

/// <summary>
/// helper สำหรับ verify ILogger.LogError แบบไม่ยุ่งยาก
/// </summary>
internal static class LoggerMoqExtensions
{
    public static void VerifyLogErrorCalled<T>(this Mock<ILogger<T>> logger, Times times)
    {
        logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((_, __) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((_, __) => true)),
            times);
    }

    public static void VerifyLog<T>(this Mock<ILogger<T>> logger, LogLevel level, Times times)
    {
        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((_, __) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((_, __) => true)),
            times);
    }
}
