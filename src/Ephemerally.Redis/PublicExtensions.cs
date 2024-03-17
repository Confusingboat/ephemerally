using Ephemerally.Redis;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace Ephemerally;

public static class PublicExtensions
{
    public static EphemeralRedisDatabase AsEphemeral(this IDatabase database) =>
        database is null or EphemeralRedisDatabase
            ? (EphemeralRedisDatabase)database
            : database.ToEphemeral();

    public static EphemeralRedisDatabase ToEphemeral(this IDatabase database) =>
        new(new RedisDatabaseEphemeral(database));

    public static EphemeralRedisDatabase GetEphemeralDatabase(
        this IConnectionMultiplexer multiplexer,
        int db = -1) =>
        multiplexer.GetDatabase(db).AsEphemeral();

    public static EphemeralConnectionMultiplexer AsEphemeralMultiplexer(this IConnectionMultiplexer multiplexer) =>
        multiplexer is EphemeralConnectionMultiplexer connectionMultiplexer
            ? connectionMultiplexer
            : multiplexer.ToEphemeralMultiplexer();

    public static EphemeralConnectionMultiplexer ToEphemeralMultiplexer(this IConnectionMultiplexer multiplexer) =>
        new(multiplexer);

    public static async Task<EphemeralConnectionMultiplexer> AsEphemeralMultiplexer<T>(this Task<T> creatingMultiplexer)
        where T : IConnectionMultiplexer =>
        (await creatingMultiplexer.ConfigureAwait(false)).AsEphemeralMultiplexer();

    public static async Task<EphemeralConnectionMultiplexer> ToEphemeralMultiplexer<T>(this Task<T> creatingMultiplexer)
        where T : IConnectionMultiplexer =>
        (await creatingMultiplexer.ConfigureAwait(false)).ToEphemeralMultiplexer();
}