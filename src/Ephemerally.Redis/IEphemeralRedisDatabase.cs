using StackExchange.Redis;

namespace Ephemerally.Redis;

public interface IEphemeralRedisDatabase : IDatabase, IDisposable, IAsyncDisposable { }