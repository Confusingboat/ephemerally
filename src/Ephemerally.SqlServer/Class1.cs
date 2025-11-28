using Microsoft.Data.SqlClient;

namespace Ephemerally.SqlServer;

public class SqlDatabaseEphemeral : Ephemeral<SqlDatabase>
{
    public SqlDatabaseEphemeral(
        SqlDatabase value,
        Func<SqlDatabase, string> getFullName,
        EphemeralOptions options)
        : base(value, getFullName, options) { }

    protected override Task CleanupSelfAsync() => throw new NotImplementedException();

    protected override Task CleanupAllAsync() => throw new NotImplementedException();
}

public class SqlDatabase
{
    public string Name { get; init; }
}