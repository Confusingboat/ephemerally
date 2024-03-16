using StackExchange.Redis;

namespace Ephemerally.Redis.Xunit;

public class PooledRedisDatabase(in FixedSizeObjectPool<IDatabase> pool, IDatabase database) :
    RedisDatabaseDecorator(database),
    IEphemeralRedisDatabase
{
    private readonly FixedSizeObjectPool<IDatabase> _pool = pool;

    private void Return() => _pool.Return(RedisDatabase);

    public ValueTask DisposeAsync()
    {
        Return();
        return RedisDatabase.TryDisposeAsync();
    }

    public void Dispose()
    {
        Return();
        RedisDatabase.TryDispose();
    }
}