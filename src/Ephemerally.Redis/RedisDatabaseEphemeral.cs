using StackExchange.Redis;

namespace Ephemerally.Redis;

public class RedisDatabaseEphemeral(IDatabase value) : Ephemeral<IDatabase>(value, x => x.Database.ToString(), EphemeralCreationOptions),
    IDisposable
{
    private static readonly EphemeralCreationOptions EphemeralCreationOptions = new()
    {
        CleanupBehavior = CleanupBehavior.SelfOnly
    };

    protected override Task CleanupSelfAsync() =>
        Task.WhenAll(
            Value.Multiplexer.GetEndPoints(true)
                .Select(endpoint => Value.Multiplexer.GetServer(endpoint))
                .Select(server => server.FlushDatabaseAsync(Value.Database)));

    protected override void CleanupSelf()
    {
        var servers = Value.Multiplexer
            .GetEndPoints(true)
            .Select(endpoint => Value.Multiplexer.GetServer(endpoint));

        foreach (var server in servers)
        {
            server.FlushDatabase(Value.Database);
        }
    }

    // TODO: Implement some way to persist that the database hasn't been cleaned up.
    //       In Redis 7+ this could be a well-known lua function that returns the expiry
    protected override Task CleanupAllAsync() => Task.CompletedTask;
}