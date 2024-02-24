﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ephemerally.Redis.Xunit;

public class EphemeralRedisDatabaseFixture : RedisMultiplexerFixture
{
    private readonly Lazy<Task<IEphemeralRedisDatabase>> _database;

    public IEphemeralRedisDatabase Database => _database.Value.Result;

    public EphemeralRedisDatabaseFixture()
    {
        _database = new(CreateDatabaseAsync);
    }

    protected override async Task<IConnectionMultiplexer> CreateMultiplexerAsync()
    {
        var implementation = await base.CreateMultiplexerAsync();
        return new PooledConnectionMultiplexer(implementation);
    }

    protected virtual async Task<IEphemeralRedisDatabase> CreateDatabaseAsync()
    {
        var multiplexer = await GetMultiplexer();
        return multiplexer.GetDatabase() as IEphemeralRedisDatabase;
    }
}