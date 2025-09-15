using Dotnet8RestApiJwtTemplate.Api.Configs;
using Dotnet8RestApiJwtTemplate.Api.Models.AuthenModel;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dotnet8RestApiJwtTemplate.Api.Services.AuthService;

public sealed class AuthenService(IOptions<JwtOptions> opt) : IAuthenService
{
    public AuthenResponse Generate(string username, string nickname)
    {
        var o = opt.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(o.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("username", username),
            new Claim("nickname", nickname),
        };

        var expirationTimeUtc = DateTime.UtcNow.AddMinutes(o.AccessTokenMinutes);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: o.Issuer,
            audience: o.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expirationTimeUtc,
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        var bangkokOffset = TimeSpan.FromHours(7);
        var expirationTimeInBangkok = new DateTimeOffset(expirationTimeUtc).ToOffset(bangkokOffset);

        return new AuthenResponse
        {
            AccessToken = accessToken,
            Expires = expirationTimeInBangkok,
            TokenType = "Bearer"
        };
    }
}
