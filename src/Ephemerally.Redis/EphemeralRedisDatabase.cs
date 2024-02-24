using StackExchange.Redis;

namespace Ephemerally.Redis;

public class EphemeralRedisDatabase(in IEphemeral<IDatabase> databaseEphemeral) :
    RedisDatabaseDecorator(databaseEphemeral.Value),
    IEphemeralRedisDatabase
{
    private readonly IEphemeral<IDatabase> _databaseEphemeral = databaseEphemeral;

    public ValueTask DisposeAsync() => _databaseEphemeral.DisposeAsync();

    public void Dispose() => (_databaseEphemeral as IDisposable)?.Dispose();
}