using System.Net;
using StackExchange.Redis;
using StackExchange.Redis.Maintenance;
using StackExchange.Redis.Profiling;

namespace Ephemerally.Redis.Xunit;

public class PooledConnectionMultiplexer : IConnectionMultiplexer
{
    private readonly IConnectionMultiplexer _multiplexer;

    private readonly FixedSizeObjectPool<IDatabase> _databases;

    public PooledConnectionMultiplexer(IConnectionMultiplexer multiplexer)
        : this(multiplexer, Enumerable.Range(0, 16).ToArray()) { }

    public PooledConnectionMultiplexer(IConnectionMultiplexer multiplexer, int[] databases)
    {
        _multiplexer = multiplexer;
        _databases = new(
            databases
                .Select(db => (IDatabase)multiplexer.GetDatabase(db).ToEphemeral())
                .ToList());
    }

    public void Dispose()
    {
        foreach (var db in _databases.Objects)
        {
            db.TryDispose();
        }
        _multiplexer.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(
            _databases
                .Objects
                .Select(db => db
                    .TryDisposeAsync()
                    .AsTask()));
        await _multiplexer.DisposeAsync();
    }

    #region Decorated Members

    public IDatabase GetDatabase(int db = -1, object asyncState = null) => new PooledRedisDatabase(_databases, _databases.Get());

    #endregion

    #region Delegated IConnectionMultiplexer Members

    public void RegisterProfiler(Func<ProfilingSession> profilingSessionProvider) => _multiplexer.RegisterProfiler(profilingSessionProvider);

    public ServerCounters GetCounters() => _multiplexer.GetCounters();

    public EndPoint[] GetEndPoints(bool configuredOnly = false) => _multiplexer.GetEndPoints(configuredOnly);

    public void Wait(Task task) => _multiplexer.Wait(task);

    public T Wait<T>(Task<T> task) => _multiplexer.Wait(task);

    public void WaitAll(params Task[] tasks) => _multiplexer.WaitAll(tasks);

    public int HashSlot(RedisKey key) => _multiplexer.HashSlot(key);

    public ISubscriber GetSubscriber(object asyncState = null) => _multiplexer.GetSubscriber(asyncState);

    public IServer GetServer(string host, int port, object asyncState = null) => _multiplexer.GetServer(host, port, asyncState);

    public IServer GetServer(string hostAndPort, object asyncState = null) => _multiplexer.GetServer(hostAndPort, asyncState);

    public IServer GetServer(IPAddress host, int port) => _multiplexer.GetServer(host, port);

    public IServer GetServer(EndPoint endpoint, object asyncState = null) => _multiplexer.GetServer(endpoint, asyncState);

    public IServer[] GetServers() => _multiplexer.GetServers();

    public Task<bool> ConfigureAsync(TextWriter log = null) => _multiplexer.ConfigureAsync(log);

    public bool Configure(TextWriter log = null) => _multiplexer.Configure(log);

    public string GetStatus() => _multiplexer.GetStatus();

    public void GetStatus(TextWriter log) => _multiplexer.GetStatus(log);

    public void Close(bool allowCommandsToComplete = true) => _multiplexer.Close(allowCommandsToComplete);

    public Task CloseAsync(bool allowCommandsToComplete = true) => _multiplexer.CloseAsync(allowCommandsToComplete);

    public string GetStormLog() => _multiplexer.GetStormLog();

    public void ResetStormLog() => _multiplexer.ResetStormLog();

    public long PublishReconfigure(CommandFlags flags = CommandFlags.None) => _multiplexer.PublishReconfigure(flags);

    public Task<long> PublishReconfigureAsync(CommandFlags flags = CommandFlags.None) => _multiplexer.PublishReconfigureAsync(flags);

    public int GetHashSlot(RedisKey key) => _multiplexer.GetHashSlot(key);

    public void ExportConfiguration(Stream destination, ExportOptions options = ExportOptions.All) => _multiplexer.ExportConfiguration(destination, options);

    public string ClientName => _multiplexer.ClientName;

    public string Configuration => _multiplexer.Configuration;

    public int TimeoutMilliseconds => _multiplexer.TimeoutMilliseconds;

    public long OperationCount => _multiplexer.OperationCount;

    public bool PreserveAsyncOrder
    {
        get => _multiplexer.PreserveAsyncOrder;
        set => _multiplexer.PreserveAsyncOrder = value;
    }

    public bool IsConnected => _multiplexer.IsConnected;

    public bool IsConnecting => _multiplexer.IsConnecting;

    public bool IncludeDetailInExceptions
    {
        get => _multiplexer.IncludeDetailInExceptions;
        set => _multiplexer.IncludeDetailInExceptions = value;
    }

    public int StormLogThreshold
    {
        get => _multiplexer.StormLogThreshold;
        set => _multiplexer.StormLogThreshold = value;
    }

    public event EventHandler<RedisErrorEventArgs> ErrorMessage
    {
        add => _multiplexer.ErrorMessage += value;
        remove => _multiplexer.ErrorMessage -= value;
    }

    public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed
    {
        add => _multiplexer.ConnectionFailed += value;
        remove => _multiplexer.ConnectionFailed -= value;
    }

    public event EventHandler<InternalErrorEventArgs> InternalError
    {
        add => _multiplexer.InternalError += value;
        remove => _multiplexer.InternalError -= value;
    }

    public event EventHandler<ConnectionFailedEventArgs> ConnectionRestored
    {
        add => _multiplexer.ConnectionRestored += value;
        remove => _multiplexer.ConnectionRestored -= value;
    }

    public event EventHandler<EndPointEventArgs> ConfigurationChanged
    {
        add => _multiplexer.ConfigurationChanged += value;
        remove => _multiplexer.ConfigurationChanged -= value;
    }

    public event EventHandler<EndPointEventArgs> ConfigurationChangedBroadcast
    {
        add => _multiplexer.ConfigurationChangedBroadcast += value;
        remove => _multiplexer.ConfigurationChangedBroadcast -= value;
    }

    public event EventHandler<ServerMaintenanceEvent> ServerMaintenanceEvent
    {
        add => _multiplexer.ServerMaintenanceEvent += value;
        remove => _multiplexer.ServerMaintenanceEvent -= value;
    }

    public event EventHandler<HashSlotMovedEventArgs> HashSlotMoved
    {
        add => _multiplexer.HashSlotMoved += value;
        remove => _multiplexer.HashSlotMoved -= value;
    }

    #endregion
}