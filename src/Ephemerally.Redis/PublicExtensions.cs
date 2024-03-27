using Ephemerally.Redis;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace Ephemerally;

public static class PublicExtensions
{
    public static IEphemeralRedisDatabase AsEphemeral(this IDatabase database) =>
        database is null or IEphemeralRedisDatabase
            ? (IEphemeralRedisDatabase)database
            : database.ToEphemeral();

    public static IEphemeralRedisDatabase ToEphemeral(this IDatabase database) =>
        new EphemeralRedisDatabase(new RedisDatabaseEphemeral(database));

    public static IEphemeralRedisDatabase GetEphemeralDatabase(
        this IConnectionMultiplexer multiplexer,
        int db = -1,
        object asyncState = null) =>
        multiplexer.GetDatabase(db, asyncState).AsEphemeral();

    #region EphemeralConnectionMultiplexer

    public static EphemeralConnectionMultiplexer AsEphemeralMultiplexer(this IConnectionMultiplexer multiplexer) =>
        multiplexer as EphemeralConnectionMultiplexer ?? multiplexer.ToEphemeralMultiplexer();

    public static EphemeralConnectionMultiplexer ToEphemeralMultiplexer(this IConnectionMultiplexer multiplexer) =>
        new(multiplexer);

    #endregion

    #region PooledConnectionMultiplexer

    public static PooledConnectionMultiplexer AsPooledMultiplexer(this IConnectionMultiplexer multiplexer) =>
        multiplexer as PooledConnectionMultiplexer ?? multiplexer.ToPooledMultiplexer();

    public static PooledConnectionMultiplexer ToPooledMultiplexer(this IConnectionMultiplexer multiplexer) =>
        new(multiplexer);
   

    #endregion
}