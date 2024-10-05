using System.Diagnostics.CodeAnalysis;
using Ephemerally.Xunit;

namespace Ephemerally.Azure.Cosmos.Xunit;

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
public abstract class DependentSubjectFixture<TSubject> : ISubjectFixture<TSubject>
{
    private readonly ISubjectFixture<TSubject> _implementation;
    private readonly ISubjectFixture[] _managedFixtures;

    protected DependentSubjectFixture(
        ISubjectFixture<TSubject> implementation,
        params ISubjectFixture[] managedFixtures)
    {
        _implementation = implementation;
        _managedFixtures = managedFixtures;
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
            await Task.WhenAll(_managedFixtures.Select(async x =>
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