using Microsoft.Data.SqlClient;
namespace Dotnet8RestApiJwtTemplate.Api.Configs;

public interface ISqlConnectionFactory
{
    ValueTask<SqlConnection> CreateAsync(CancellationToken ct = default);
}
