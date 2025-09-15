using Dotnet8RestApiJwtTemplate.Api.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scrutor;
using System.Reflection;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure Services
var appSettings = builder.Configuration.Get<AppSettings>()
    ?? throw new ArgumentException("AppSettings is not found");

ConfigureControllers(builder);
ConfigureServicesAndRepositories(builder);
ConfigureUtilityServices(builder);
ConfigureDatabase(builder, appSettings);
ConfigureLogging(builder, appSettings);
ConfigureSwagger(builder);
ConfigureAuth(builder);

var app = builder.Build();

// Configure Middleware
ConfigureMiddleware(app, appSettings);

app.MapControllers();
app.Run();

void ConfigureControllers(WebApplicationBuilder builder)
{
    builder.Services
        .AddControllers(options =>
        {
            options.ReturnHttpNotAcceptable = true;
            options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
        })
        .AddJsonOptions(options =>
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        );
}

void ConfigureServicesAndRepositories(WebApplicationBuilder builder)
{
    builder.Services.Scan(selector => selector
        .FromAssemblies(Assembly.GetExecutingAssembly())
        .AddClasses(tf => tf.Where(t =>
            t.Name.EndsWith("Service") ||
            t.Name.EndsWith("Repository") ||
            t.Name.EndsWith("Client")))
        .UsingRegistrationStrategy(RegistrationStrategy.Throw)
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    );
}

void ConfigureUtilityServices(WebApplicationBuilder builder)
{

}

void ConfigureDatabase(WebApplicationBuilder builder, AppSettings appSettings)
{
    var cs = builder.Configuration.GetConnectionString("Default");
    if (string.IsNullOrWhiteSpace(cs))
    {
        throw new ArgumentNullException(nameof(cs), "SQL Server Connection String is not found");
    }

    builder.Services.Configure<DatabaseOptions>(o => o.ConnectionString = cs);

    builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
}

void ConfigureLogging(WebApplicationBuilder builder, AppSettings appSettings)
{
}

void ConfigureSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.EnableAnnotations(); // Enables annotations like SwaggerSchema
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "SSC Dotnet8RestApiJwtTemplate API",
            Version = "v1",
            Description = "API for Dotnet8RestApiJwtTemplate"
        });

        // เพิ่ม XML Comments
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

        // JWT support
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "ใส่ Bearer {token}"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                Array.Empty<string>()
            }
        });
    });

    builder.Services.Configure<UrlService>(builder.Configuration.GetSection("UrlService"));

}

void ConfigureAuth(WebApplicationBuilder builder)
{
    // bind options
    builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

    // read options once for validation params
    var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
              ?? throw new ArgumentException("Jwt options missing");

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = true;
            o.SaveToken = false;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            o.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    Console.WriteLine("JWT failed: " + ctx.Exception.Message);
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorization();
}

void ConfigureMiddleware(WebApplication app, AppSettings appSettings)
{
    // CORS
    app.UseCors(options => options
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
    );

    var pathBase = appSettings.SwaggerUrl;
    if (!string.IsNullOrEmpty(pathBase))
    {
        app.UsePathBase(pathBase);
    }

    // Swagger
    if (appSettings.IsShowSwagger ?? false)
    {
        app.UseSwagger(c =>
        {
            c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                swaggerDoc.Servers =
                [
                    new() { Url = $"https://{httpReq.Host.Value}{appSettings.SwaggerUrl}" },
                    new() { Url = $"http://{httpReq.Host.Value}{appSettings.SwaggerUrl}" }
                ];
            });
        });
        app.UseSwaggerUI();
    }

    // Other Middleware
    app.UseHsts();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
}