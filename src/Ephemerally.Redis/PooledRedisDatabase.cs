using StackExchange.Redis;

namespace Ephemerally.Redis;

public class PooledRedisDatabase(in FixedSizeObjectPool<IDatabase> pool, IDatabase database) :
    RedisDatabaseDecorator(database),
    IEphemeralRedisDatabase
{
    private bool _disposed;

    private readonly FixedSizeObjectPool<IDatabase> _pool = pool;

    private void Return() => _pool.Return(RedisDatabase);

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        Return();
        await RedisDatabase.TryDisposeAsync();

        _disposed = true;
    }

    public void Dispose()
    {
        if (_disposed) return;

        Return();
        RedisDatabase.TryDispose();

        _disposed = true;
    }
}