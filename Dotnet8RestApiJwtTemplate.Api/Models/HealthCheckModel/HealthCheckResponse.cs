namespace Dotnet8RestApiJwtTemplate.Api.Models.HealthCheckModel;

public sealed record HealthCheckResponse
{
    public string? Status { get; set; }

    public DateTime Timestamp { get; set; }

    public string? Error { get; set; }
}
