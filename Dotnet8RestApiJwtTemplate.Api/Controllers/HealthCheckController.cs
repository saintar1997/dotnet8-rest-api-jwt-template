using Dotnet8RestApiJwtTemplate.Api.Models.HealthCheckModel;
using Dotnet8RestApiJwtTemplate.Api.Services.HealthCheckService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet8RestApiJwtTemplate.Api.Controllers;

[Route("v1/[controller]")]
[ApiController]
[Authorize]
public class HealthCheckController(IHealthCheckService healthCheckService, ILogger<HealthCheckController> logger) : ControllerBase
{
    private readonly ILogger _logger = logger;
    private readonly IHealthCheckService _healthCheckService = healthCheckService;

    [HttpGet("alive")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
    public ActionResult<HealthCheckResponse> Alive()
    {
        _logger.LogInformation("GET healthcheck/alive");
        return Ok(new HealthCheckResponse { Status = "Healthy", Timestamp = DateTime.Now });
    }

    [HttpGet("connection")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Health()
    {
        try
        {
            _logger.LogInformation("GET healthcheck/connection");
            await _healthCheckService.HealthCheck();
            return Ok(new HealthCheckResponse { Status = "Healthy", Timestamp = DateTime.Now });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET healthcheck/health, Database health check failed");
            return StatusCode(StatusCodes.Status500InternalServerError, new HealthCheckResponse { Status = "Unhealthy", Error = "An error occurred while checking the database health.", Timestamp = DateTime.Now });
        }
    }
}

