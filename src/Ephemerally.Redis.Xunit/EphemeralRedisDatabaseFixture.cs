using System;
using System.Collections.Generic;
using System.Text;

namespace Ephemerally.Redis.Xunit;

internal class EphemeralRedisDatabaseFixture : EphemeralRedisDatabasePoolFixture
{
    private readonly Lazy<Task<IEphemeralRedisDatabase>> _database;

    public IEphemeralRedisDatabase Database => _database.Value.Result;

    public EphemeralRedisDatabaseFixture()
    {
        _database = new(CreateDatabaseAsync);
    }  

    protected virtual async Task<IEphemeralRedisDatabase> CreateDatabaseAsync()
    {
        var multiplexer = await GetMultiplexer();
        return multiplexer.GetDatabase() as IEphemeralRedisDatabase;
    }
}