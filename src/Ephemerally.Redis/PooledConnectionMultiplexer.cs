using StackExchange.Redis;

namespace Ephemerally.Redis;

public class PooledConnectionMultiplexer : ConnectionMultiplexerDecorator
{
    private bool _disposed;

    private readonly FixedSizeObjectPool<IDatabase> _databases;

    public PooledConnectionMultiplexer(IConnectionMultiplexer underlyingMultiplexer)
        : this(underlyingMultiplexer, Enumerable.Range(0, 16).ToArray()) { }

    public PooledConnectionMultiplexer(
        IConnectionMultiplexer underlyingMultiplexer,
        int[] databases) : base(underlyingMultiplexer)
    {
        _databases = new(
            databases
                .Select(db => underlyingMultiplexer.GetDatabase(db))
                .ToList());
    }

    public override void Dispose()
    {
        try
        {
            if (_disposed) return;

            _disposed = true;

            foreach (var db in _databases.Objects)
            {
                db.TryDispose();
            }
        }
        finally
        {
            base.Dispose();
        }
    }

    public override async ValueTask DisposeAsync()
    {
        try
        {
            if (_disposed) return;

            _disposed = true;

            await Task.WhenAll(
                _databases
                    .Objects
                    .Select(db => db
                        .TryDisposeAsync()
                        .AsTask()));
        }
        finally
        {
            await base.DisposeAsync();
        }
    }

    #region Decorated Members

    public override IDatabase GetDatabase(int db = -1, object asyncState = null) =>
        new PooledRedisDatabase(
            _databases,
            db == -1
                ? _databases.Get()
                : _databases.GetWhere(x => x.Database == db)
        );

    #endregion
}