namespace Dotnet8RestApiJwtTemplate.Api.Models.AuthenModel;

public sealed record AuthenRequest
{
    public string Username { get; init; } = "";
    public string Nickname { get; init; } = "";
}