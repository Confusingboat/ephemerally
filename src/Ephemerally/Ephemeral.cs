namespace Ephemerally;

public sealed class Ephemeral : IEphemeralExtension
{
    public static Ephemeral Create = new();
}

public interface IEphemeralExtension { }

public abstract class Ephemeral<TValue> : IEphemeral<TValue>
    where TValue : class
{
    private bool _disposed;

    private readonly EphemeralOptions _options;
    private readonly IEphemeralMetadata _metadata;
    private readonly TValue _object;

    private string FullName => _metadata.FullName;
    public DateTimeOffset Expiration => _metadata.Expiration!.Value;
    public IEphemeralMetadata Metadata => _metadata;

    protected Ephemeral(TValue value, Func<TValue, string> getFullName, EphemeralOptions options)
        : this(value, EphemeralMetadata.Parse(getFullName(value)), options) { }

    protected Ephemeral(TValue value, IEphemeralMetadata metadata, EphemeralOptions options)
    {
        _object = value;
        _options = options;
        _metadata = metadata;
    }

    public TValue Value => _object ?? throw new InvalidOperationException("The object has not been created yet.");

    /// <summary>
    /// In an overridden implementation, this method should delete the TObject.
    /// </summary>
    /// <returns></returns>
    protected abstract Task CleanupSelfAsync();

    /// <summary>
    /// In an overridden implementation, this method should delete all expired TObject.
    /// </summary>
    /// <returns></returns>
    protected abstract Task CleanupAllAsync();

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_disposed) return;

            if (_options.CleanupBehavior == CleanupBehavior.NoCleanup) return;

            await CleanupSelfAsync().ConfigureAwait(false);
            if (_options.CleanupBehavior == CleanupBehavior.SelfOnly) return;

            await CleanupAllAsync().ConfigureAwait(false);
        }
        finally
        {
            _disposed = true;

            await Value.TryDisposeAsync().ConfigureAwait(false);
        }
    }

    protected virtual void CleanupSelf() { }

    protected virtual void CleanupAll() { }

    public void Dispose()
    {
        try
        {
            if (_disposed) return;

            if (_options.CleanupBehavior == CleanupBehavior.NoCleanup) return;

            CleanupSelf();
            if (_options.CleanupBehavior == CleanupBehavior.SelfOnly) return;

            CleanupAll();
        }
        finally
        {
            _disposed = true;

            Value.TryDispose();
        }
    }
}