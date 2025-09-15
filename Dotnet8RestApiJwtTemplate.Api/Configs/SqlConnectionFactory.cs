using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Dotnet8RestApiJwtTemplate.Api.Configs;

public sealed class SqlConnectionFactory(IOptionsMonitor<DatabaseOptions> opt) : ISqlConnectionFactory
{
    public async ValueTask<SqlConnection> CreateAsync(CancellationToken ct = default)
    {
        var cs = opt.CurrentValue.ConnectionString
                 ?? throw new ArgumentNullException("SQL connection string is missing");
        var conn = new SqlConnection(cs);
        await conn.OpenAsync(ct);
        return conn;
    }
}
