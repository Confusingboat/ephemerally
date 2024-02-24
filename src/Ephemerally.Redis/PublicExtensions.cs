using Ephemerally.Redis;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace Ephemerally;

public static class PublicExtensions
{
    public static EphemeralRedisDatabase ToEphemeral(this IDatabase database) =>
        new(new RedisDatabaseEphemeral(database));
}