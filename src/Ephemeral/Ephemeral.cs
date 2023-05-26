namespace Ephemeral;

public abstract class Ephemeral<TObject> : IEphemeral<TObject>
    where TObject : class
{
    private readonly EphemeralOptions _options;
    private readonly EphemeralMetadata _metadata;
    private readonly Lazy<Task<TObject>> _object;

    private string FullName => _metadata.FullName;
    public DateTimeOffset Expiration => _metadata.Expiration!.Value;
    public IEphemeralMetadata Metadata => _metadata;
    public bool IsCreated { get; private set; }

    protected Ephemeral(EphemeralOptions options)
    {
        _options = options;
        _metadata = EphemeralMetadata.New(options.Name, options.GetExpiration(DateTimeOffset.UtcNow));
        _object = new Lazy<Task<TObject>>(EnsureExistsInternal);
    }

    public async ValueTask<TObject> GetAsync()
    {
        if (_options.CreationCachingBehavior == CreationCachingBehavior.Cache)
        {
            return await _object.Value;
        }

        return await EnsureExistsInternal();
    }

    private async Task<TObject> EnsureExistsInternal()
    {
        var result = await EnsureExistsAsync(FullName);
        IsCreated = true;
        return result;
    }

    /// <summary>
    /// In an overridden implementation, this method should return the TObject if it exists, or create it if it does not.
    /// </summary>
    /// <returns></returns>
    protected abstract Task<TObject> EnsureExistsAsync(string fullName);

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

        await CleanupSelfAsync(FullName);
        if (_options.CleanupBehavior == CleanupBehavior.SelfOnly) return;

        await CleanupAllAsync();
    }
}
