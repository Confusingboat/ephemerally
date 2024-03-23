using System;
using System.Collections.Generic;
using System.Text;

namespace Ephemerally.Redis.xUnit;

internal class PooledEphemeralRedisDatabaseFixture : PooledEphemeralRedisMultiplexerFixture
{
    private readonly Lazy<Task<IEphemeralRedisDatabase>> _database;

    public IEphemeralRedisDatabase Database => _database.Value.Result;

    public PooledEphemeralRedisDatabaseFixture()
    {
        _database = new(CreateDatabaseAsync);
    }  

    protected virtual async Task<IEphemeralRedisDatabase> CreateDatabaseAsync()
    {
        var multiplexer = await GetMultiplexer();
        return multiplexer.GetDatabase() as IEphemeralRedisDatabase;
    }
}