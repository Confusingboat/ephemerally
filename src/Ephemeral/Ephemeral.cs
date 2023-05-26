namespace Ephemeral;

public abstract class Ephemeral<TObject> : IEphemeral<TObject>
    where TObject : class
{
    private readonly Lazy<Task<TObject>> _object;

    private string FullName => Metadata.FullName;

    public EphemeralMetadata Metadata { get; }

    private EphemeralOptions Options { get; }

    protected Ephemeral(EphemeralOptions options)
    {
        Options = options;
        Metadata = EphemeralMetadata.New(options.Name, options.GetExpiration(DateTimeOffset.UtcNow));
        _object = new Lazy<Task<TObject>>(() => EnsureExistsAsync(FullName));
    }

    public async ValueTask<TObject> GetAsync()
    {
        if (Options.CreationCachingBehavior == CreationCachingBehavior.Cache)
        {
            return await _object.Value;
        }

        return await EnsureExistsAsync(FullName);
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
        if (Options.CleanupBehavior == CleanupBehavior.NoCleanup) return;

        await CleanupSelfAsync(FullName);
        if (Options.CleanupBehavior == CleanupBehavior.SelfOnly) return;

        await CleanupAllAsync();
    }
}
