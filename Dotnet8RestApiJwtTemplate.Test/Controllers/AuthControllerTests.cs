using Dotnet8RestApiJwtTemplate.Api.Controllers;
using Dotnet8RestApiJwtTemplate.Api.Models.AuthenModel;
using Dotnet8RestApiJwtTemplate.Api.Services.AuthService;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace SSCDashboard.Test.Controllers;

public class AuthenControllerTests
{
    [Fact]
    public void Issue_Returns_BadRequest_When_Empty_Input()
    {
        // arrange
        var svc = new Mock<IAuthenService>();
        var logger = new Mock<ILogger<AuthenController>>();
        var controller = new AuthenController(svc.Object, logger.Object);
        var req = new AuthenRequest
        {
            Username = "",
            Nickname = ""
        };

        // act
        var result = controller.Issue(req);

        // assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        bad.Value.Should().Be("username and nickname are required.");

        svc.VerifyNoOtherCalls();
    }

    [Fact]
    public void Issue_Returns_BadRequest_When_Whitespace_Input()
    {
        // arrange
        var svc = new Mock<IAuthenService>();
        var logger = new Mock<ILogger<AuthenController>>();
        var controller = new AuthenController(svc.Object, logger.Object);
        var req = new AuthenRequest
        {
            Username = "   ",
            Nickname = "id-1"
        };

        // act
        var result = controller.Issue(req);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
        svc.VerifyNoOtherCalls();
    }

    [Fact]
    public void Issue_Returns_Ok_With_Token_And_Calls_Service()
    {
        // arrange
        var svc = new Mock<IAuthenService>();
        var logger = new Mock<ILogger<AuthenController>>();
        var controller = new AuthenController(svc.Object, logger.Object);
        var req = new AuthenRequest
        {
            Username = "sec-1",
            Nickname = "id-1"
        };

        var expectedTokenResponse = new AuthenResponse
        {
            AccessToken = "this.is.a.mocked.jwt.token",
            TokenType = "Bearer",
            Expires = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        svc.Setup(s => s.Generate("sec-1", "id-1")).Returns(expectedTokenResponse);

        // act
        var result = controller.Issue(req);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);

        ok.Value.Should().BeEquivalentTo(expectedTokenResponse);

        svc.Verify(s => s.Generate("sec-1", "id-1"), Times.Once);
        svc.VerifyNoOtherCalls();
    }
}