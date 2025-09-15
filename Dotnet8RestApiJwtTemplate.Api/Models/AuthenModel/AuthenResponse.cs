namespace Dotnet8RestApiJwtTemplate.Api.Models.AuthenModel;

public sealed record AuthenResponse
{
    public string AccessToken { get; init; } = "";
    public string TokenType { get; init; } = "";
    public DateTimeOffset Expires { get; init; }
}