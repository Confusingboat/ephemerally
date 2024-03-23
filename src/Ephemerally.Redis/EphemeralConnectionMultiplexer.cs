using System.Net;
using StackExchange.Redis;
using StackExchange.Redis.Maintenance;
using StackExchange.Redis.Profiling;

namespace Ephemerally.Redis;

public class EphemeralConnectionMultiplexer(IConnectionMultiplexer underlyingMultiplexer)
    : ConnectionMultiplexerDecorator(underlyingMultiplexer)
{
    public override IDatabase GetDatabase(int db = -1, object asyncState = null) => 
        UnderlyingMultiplexer.GetDatabase(db, asyncState).AsEphemeral();
}

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

public abstract class ConnectionMultiplexerDecorator(IConnectionMultiplexer underlyingMultiplexer)
    : IConnectionMultiplexer
{
    private bool _disposed;

    public IConnectionMultiplexer UnderlyingMultiplexer { get; } = underlyingMultiplexer;

    public virtual void Dispose()
    {
        if (_disposed) return;

        _disposed = true;

        UnderlyingMultiplexer.Dispose();
    }

    public virtual ValueTask DisposeAsync()
    {
        if (_disposed) return new();

        _disposed = true;

        return UnderlyingMultiplexer.DisposeAsync();
    }


    #region Decorated Members

    public virtual IDatabase GetDatabase(int db = -1, object asyncState = null) => UnderlyingMultiplexer.GetDatabase(db, asyncState);

    #endregion

    #region Delegated IConnectionMultiplexer Members

    public void RegisterProfiler(Func<ProfilingSession> profilingSessionProvider) => UnderlyingMultiplexer.RegisterProfiler(profilingSessionProvider);

    public ServerCounters GetCounters() => UnderlyingMultiplexer.GetCounters();

    public EndPoint[] GetEndPoints(bool configuredOnly = false) => UnderlyingMultiplexer.GetEndPoints(configuredOnly);

    public void Wait(Task task) => UnderlyingMultiplexer.Wait(task);

    public T Wait<T>(Task<T> task) => UnderlyingMultiplexer.Wait(task);

    public void WaitAll(params Task[] tasks) => UnderlyingMultiplexer.WaitAll(tasks);

    public int HashSlot(RedisKey key) => UnderlyingMultiplexer.HashSlot(key);

    public ISubscriber GetSubscriber(object asyncState = null) => UnderlyingMultiplexer.GetSubscriber(asyncState);

    public IServer GetServer(string host, int port, object asyncState = null) => UnderlyingMultiplexer.GetServer(host, port, asyncState);

    public IServer GetServer(string hostAndPort, object asyncState = null) => UnderlyingMultiplexer.GetServer(hostAndPort, asyncState);

    public IServer GetServer(IPAddress host, int port) => UnderlyingMultiplexer.GetServer(host, port);

    public IServer GetServer(EndPoint endpoint, object asyncState = null) => UnderlyingMultiplexer.GetServer(endpoint, asyncState);

    public IServer[] GetServers() => UnderlyingMultiplexer.GetServers();

    public Task<bool> ConfigureAsync(TextWriter log = null) => UnderlyingMultiplexer.ConfigureAsync(log);

    public bool Configure(TextWriter log = null) => UnderlyingMultiplexer.Configure(log);

    public string GetStatus() => UnderlyingMultiplexer.GetStatus();

    public void GetStatus(TextWriter log) => UnderlyingMultiplexer.GetStatus(log);

    public void Close(bool allowCommandsToComplete = true) => UnderlyingMultiplexer.Close(allowCommandsToComplete);

    public Task CloseAsync(bool allowCommandsToComplete = true) => UnderlyingMultiplexer.CloseAsync(allowCommandsToComplete);

    public string GetStormLog() => UnderlyingMultiplexer.GetStormLog();

    public void ResetStormLog() => UnderlyingMultiplexer.ResetStormLog();

    public long PublishReconfigure(CommandFlags flags = CommandFlags.None) => UnderlyingMultiplexer.PublishReconfigure(flags);

    public Task<long> PublishReconfigureAsync(CommandFlags flags = CommandFlags.None) => UnderlyingMultiplexer.PublishReconfigureAsync(flags);

    public int GetHashSlot(RedisKey key) => UnderlyingMultiplexer.GetHashSlot(key);

    public void ExportConfiguration(Stream destination, ExportOptions options = ExportOptions.All) => UnderlyingMultiplexer.ExportConfiguration(destination, options);

    public string ClientName => UnderlyingMultiplexer.ClientName;

    public string Configuration => UnderlyingMultiplexer.Configuration;

    public int TimeoutMilliseconds => UnderlyingMultiplexer.TimeoutMilliseconds;

    public long OperationCount => UnderlyingMultiplexer.OperationCount;

    public bool PreserveAsyncOrder
    {
        get => UnderlyingMultiplexer.PreserveAsyncOrder;
        set => UnderlyingMultiplexer.PreserveAsyncOrder = value;
    }

    public bool IsConnected => UnderlyingMultiplexer.IsConnected;

    public bool IsConnecting => UnderlyingMultiplexer.IsConnecting;

    public bool IncludeDetailInExceptions
    {
        get => UnderlyingMultiplexer.IncludeDetailInExceptions;
        set => UnderlyingMultiplexer.IncludeDetailInExceptions = value;
    }

    public int StormLogThreshold
    {
        get => UnderlyingMultiplexer.StormLogThreshold;
        set => UnderlyingMultiplexer.StormLogThreshold = value;
    }

    public event EventHandler<RedisErrorEventArgs> ErrorMessage
    {
        add => UnderlyingMultiplexer.ErrorMessage += value;
        remove => UnderlyingMultiplexer.ErrorMessage -= value;
    }

    public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed
    {
        add => UnderlyingMultiplexer.ConnectionFailed += value;
        remove => UnderlyingMultiplexer.ConnectionFailed -= value;
    }

    public event EventHandler<InternalErrorEventArgs> InternalError
    {
        add => UnderlyingMultiplexer.InternalError += value;
        remove => UnderlyingMultiplexer.InternalError -= value;
    }

    public event EventHandler<ConnectionFailedEventArgs> ConnectionRestored
    {
        add => UnderlyingMultiplexer.ConnectionRestored += value;
        remove => UnderlyingMultiplexer.ConnectionRestored -= value;
    }

    public event EventHandler<EndPointEventArgs> ConfigurationChanged
    {
        add => UnderlyingMultiplexer.ConfigurationChanged += value;
        remove => UnderlyingMultiplexer.ConfigurationChanged -= value;
    }

    public event EventHandler<EndPointEventArgs> ConfigurationChangedBroadcast
    {
        add => UnderlyingMultiplexer.ConfigurationChangedBroadcast += value;
        remove => UnderlyingMultiplexer.ConfigurationChangedBroadcast -= value;
    }

    public event EventHandler<ServerMaintenanceEvent> ServerMaintenanceEvent
    {
        add => UnderlyingMultiplexer.ServerMaintenanceEvent += value;
        remove => UnderlyingMultiplexer.ServerMaintenanceEvent -= value;
    }

    public event EventHandler<HashSlotMovedEventArgs> HashSlotMoved
    {
        add => UnderlyingMultiplexer.HashSlotMoved += value;
        remove => UnderlyingMultiplexer.HashSlotMoved -= value;
    }

    #endregion
}