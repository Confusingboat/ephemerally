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
}