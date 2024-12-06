using System.Diagnostics.CodeAnalysis;

namespace Ephemerally.Xunit;

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
public class DependentSubjectFixture<TSubject> : ISubjectFixture<TSubject>
{
    private readonly ISubjectFixture<TSubject> _implementation;
    private readonly ISubjectFixture[] _dependencies;

    public DependentSubjectFixture(
        ISubjectFixture<TSubject> implementation,
        params ISubjectFixture[] dependencies)
    {
        _implementation = implementation;
        _dependencies = dependencies;
    }

    public Task<TSubject> GetOrCreateSubjectAsync() => _implementation.GetOrCreateSubjectAsync();

    public Task InitializeAsync() => _implementation.InitializeAsync();

    public async Task DisposeAsync()
    {
        try
        {
            await _implementation.DisposeAsync();
        }
        finally
        {
            var exceptions = new List<Exception>();
            await Task.WhenAll(_dependencies.Select(async x =>
                {
                    try
                    {
                        await x.DisposeAsync();
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            ));
            if (exceptions.Count > 0)
            {
#pragma warning disable CA2219
                throw new AggregateException(exceptions);
#pragma warning restore CA2219
            }
        }
    }
}