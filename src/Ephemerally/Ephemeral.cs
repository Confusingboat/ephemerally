namespace Ephemerally;

public abstract class Ephemeral<TValue> : IEphemeral<TValue>
    where TValue : class
{
    private readonly EphemeralOptions _options;
    private readonly EphemeralMetadata _metadata;
    private readonly TValue _object;

    private string FullName => _metadata.FullName;
    public DateTimeOffset Expiration => _metadata.Expiration!.Value;
    public IEphemeralMetadata Metadata => _metadata;

    protected Ephemeral(TValue value, Func<TValue,string> getFullName, EphemeralOptions options)
    {
        _object = value;
        _options = options;
        _metadata = EphemeralMetadata.New(getFullName(value));
    }

    public TValue Value => _object ?? throw new InvalidOperationException("The object has not been created yet.");


    /// <summary>
    /// In an overridden implementation, this method should delete the TObject.
    /// </summary>
    /// <returns></returns>
    protected abstract Task CleanupSelfAsync(string fullName);

    /// <summary>
    /// In an overridden implementation, this method should delete all expired TObject.
    /// </summary>
    /// <returns></returns>
    protected abstract Task CleanupAllAsync();

    public async ValueTask DisposeAsync()
    {
        if (_options.CleanupBehavior == CleanupBehavior.NoCleanup) return;

        await CleanupSelfAsync(FullName).F();
        if (_options.CleanupBehavior == CleanupBehavior.SelfOnly) return;

        await CleanupAllAsync().F();
    }
}
