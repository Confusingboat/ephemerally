using StackExchange.Redis;

namespace Ephemerally.Redis;

public class EphemeralConnectionMultiplexer(IConnectionMultiplexer underlyingMultiplexer)
    : ConnectionMultiplexerDecorator(underlyingMultiplexer)
{
    private bool _disposed;
    private readonly HashSet<int> _databases = new();

    public override IDatabase GetDatabase(int db = -1, object asyncState = null)
    {
        var ephemeralDb = UnderlyingMultiplexer.GetDatabase(db, asyncState).AsEphemeral();
        _databases.Add(db);
        return ephemeralDb;
    }

    public override void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try
        {
            foreach (var db in _databases)
            {
                UnderlyingMultiplexer
                    .GetRootMultiplexer()
                    .GetDatabase(db)
                    .AsEphemeral()
                    .TryDispose();
            }
        }
        finally
        {
            base.Dispose();
        }
    }

    public override async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        try
        {
            await Task.WhenAll(
                _databases
                    .Select(db => UnderlyingMultiplexer
                        .GetRootMultiplexer()
                        .GetDatabase(db)
                        .AsEphemeral()
                        .TryDisposeAsync()
                        .AsTask()
                    )
                );
        }
        finally
        {
            await base.DisposeAsync();
        }
    }
}