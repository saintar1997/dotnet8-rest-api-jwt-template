using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Dotnet8RestApiJwtTemplate.Api.Configs;
using Dotnet8RestApiJwtTemplate.Api.Services.AuthService;

namespace SSCDashboard.Test.Services;

public class AuthenServiceTests
{
    private static IOptions<JwtOptions> Opt() => Options.Create(new JwtOptions
    {
        Key = "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF", 
        Issuer = "Dotnet8RestApiJwtTemplate",
        Audience = "Dotnet8RestApiJwtTemplate.Client",
        AccessTokenMinutes = 5
    });

    [Fact]
    public void Generate_Returns_Valid_JWT_With_Claims_Issuer_Audience()
    {
        // arrange
        var svc = new AuthenService(Opt());
        var username = "u1";
        var nickname = "n1";

        // act
        var response = svc.Generate(username, nickname);

        // assert
        response.Should().NotBeNull();
        response.AccessToken.Should().NotBeNullOrWhiteSpace(); // ตรวจสอบว่ามี Token string มาจริง
        response.TokenType.Should().Be("Bearer");

        var handler = new JwtSecurityTokenHandler();
        var p = Opt().Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(p.Key));
        var tvp = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = p.Issuer,
            ValidateAudience = true,
            ValidAudience = p.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        handler.ValidateToken(response.AccessToken, tvp, out var validated);
        var token = (JwtSecurityToken)validated;

        token.Issuer.Should().Be(p.Issuer);
        token.Audiences.Should().Contain(p.Audience);

        var claims = token.Claims.ToDictionary(c => c.Type, c => c.Value);
        claims["username"].Should().Be(username);
        claims["nickname"].Should().Be(nickname);
    }
    [Fact]
    public void Generate_Sets_Expiration_In_Future_And_Response_Is_Correct()
    {
        // arrange
        var svc = new AuthenService(Opt());
        var options = Opt().Value;

        var startTime = DateTime.UtcNow;

        // act
        var response = svc.Generate("s", "u");

        // assert
        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);

        token.ValidTo.Should().BeAfter(startTime); 

        token.ValidTo.Should().BeCloseTo(startTime.AddMinutes(options.AccessTokenMinutes), TimeSpan.FromSeconds(1));

        response.Expires.Offset.Should().Be(TimeSpan.FromHours(7));

        response.Expires.ToUniversalTime().Should().BeCloseTo(token.ValidTo, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Validate_Fails_When_Wrong_Key()
    {
        // arrange
        var svc = new AuthenService(Opt());
        var response = svc.Generate("s", "u");

        var wrongKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(new string('x', 64)));
        var tvp = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = wrongKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false
        };

        // act
        Action act = () => new JwtSecurityTokenHandler().ValidateToken(response.AccessToken, tvp, out _);

        // assert
        act.Should().Throw<SecurityTokenInvalidSignatureException>();
    }
}