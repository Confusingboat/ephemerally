namespace EphemeralDb;

public abstract class Ephemeral<TObject> : IEphemeral<TObject>
    where TObject : class
{
    private readonly Lazy<Task<TObject>> _container;

    protected string FullName { get; }

    private EphemeralOptions Options { get; }

    protected Ephemeral(EphemeralOptions options)
    {
        Options = options;
        FullName = EphemeralMetadata.GetFullName(options.Name, options.ContainerLifetimeSeconds);
        _container = new Lazy<Task<TObject>>(EnsureExistsAsync);
    }

    public async ValueTask<TObject> GetAsync()
    {
        if (Options.CreationCachingBehavior == CreationCachingBehavior.Cache)
        {
            return await _container.Value;
        }

        return await EnsureExistsAsync();
    }

    /// <summary>
    /// In an overridden implementation, this method should return the object if it exists, or create it if it does not.
    /// </summary>
    /// <returns></returns>
    protected abstract Task<TObject> EnsureExistsAsync();

    /// <summary>
    /// In an overridden implementation, this method should delete the object.
    /// </summary>
    /// <returns></returns>
    protected abstract Task CleanupSelfAsync();

    /// <summary>
    /// In an overridden implementation, this method should delete all expired objects.
    /// </summary>
    /// <returns></returns>
    protected abstract Task CleanupAllAsync();

    public async ValueTask DisposeAsync()
    {
        if (Options.CleanupBehavior == CleanupBehavior.NoCleanup) return;

        await CleanupSelfAsync();
        if (Options.CleanupBehavior == CleanupBehavior.SelfOnly) return;

        await CleanupAllAsync();
    }
}
