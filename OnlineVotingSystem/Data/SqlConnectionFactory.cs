using Microsoft.Data.SqlClient;

namespace OnlineVotingSystem.Data;

public sealed class SqlConnectionFactory
{
    public SqlConnection Create() => new(DatabaseSettings.ConnectionString);
}
