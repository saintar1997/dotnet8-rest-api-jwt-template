using Dotnet8RestApiJwtTemplate.Api.Models.AuthenModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dotnet8RestApiJwtTemplate.Api.Services.AuthService;

namespace Dotnet8RestApiJwtTemplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthenController(IAuthenService tokensService, ILogger<AuthenController> logger) : ControllerBase
{
    private readonly ILogger _logger = logger;
    private readonly IAuthenService _tokensService = tokensService;

    [HttpPost("token")]
    [Produces("application/json")]
    [AllowAnonymous]
    public IActionResult Issue([FromBody] AuthenRequest req)
    {
        _logger.LogInformation("POST authen/token");

        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Nickname))
            return BadRequest("username and nickname are required.");

        var response = _tokensService.Generate(req.Username, req.Nickname);

        return Ok(response);
    }
}
