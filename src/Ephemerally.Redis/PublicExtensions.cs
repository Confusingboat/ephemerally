using Ephemerally.Redis;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace Ephemerally;

public static class PublicExtensions
{
    extension(IDatabase database)
    {
        public IEphemeralRedisDatabase AsEphemeral() =>
            database is null or IEphemeralRedisDatabase
                ? (IEphemeralRedisDatabase)database
                : database.ToEphemeral();

        public IEphemeralRedisDatabase ToEphemeral() =>
            new EphemeralRedisDatabase(new RedisDatabaseEphemeral(database));
    }

    public static IEphemeralRedisDatabase GetEphemeralDatabase(
        this IConnectionMultiplexer multiplexer,
        int db = -1,
        object asyncState = null) =>
        multiplexer.GetDatabase(db, asyncState).AsEphemeral();

    #region EphemeralConnectionMultiplexer

    extension(IConnectionMultiplexer multiplexer)
    {
        public EphemeralConnectionMultiplexer AsEphemeralMultiplexer() =>
            multiplexer as EphemeralConnectionMultiplexer ?? multiplexer.ToEphemeralMultiplexer();

        public EphemeralConnectionMultiplexer ToEphemeralMultiplexer() =>
            new(multiplexer);
    }

    #endregion

    #region PooledConnectionMultiplexer

    extension(IConnectionMultiplexer multiplexer)
    {
        public PooledConnectionMultiplexer AsPooledMultiplexer() =>
            multiplexer as PooledConnectionMultiplexer ?? multiplexer.ToPooledMultiplexer();

        public PooledConnectionMultiplexer ToPooledMultiplexer() =>
            new(multiplexer);
    }

    #endregion
}