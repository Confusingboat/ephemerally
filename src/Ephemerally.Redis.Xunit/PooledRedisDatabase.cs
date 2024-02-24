using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
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
        return Database is IAsyncDisposable disposable
            ? disposable.DisposeAsync()
            : new();
    }

    public void Dispose()
    {
        Return();
        (Database as IDisposable)?.Dispose();
    }
}