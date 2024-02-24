using StackExchange.Redis;

namespace Ephemerally.Redis.Xunit;

public class PooledRedisDatabase(in FixedSizeObjectPool<IDatabase> pool, IDatabase database) :
    RedisDatabaseDecorator(database),
    IEphemeralRedisDatabase
{
    private readonly FixedSizeObjectPool<IDatabase> _pool = pool;

    private void Return() => _pool.Return(Database);

    public ValueTask DisposeAsync()
    {
        Return();
        return Database.TryDisposeAsync();
    }

    public void Dispose()
    {
        Return();
        Database.TryDispose();
    }
}