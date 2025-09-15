namespace Dotnet8RestApiJwtTemplate.Api.Configs;

public class AppSettings
{
    public required UrlService MyProperty { get; set; }
    public required string SwaggerUrl { get; set; }
    public bool? IsShowSwagger { get; set; } = true;
}
