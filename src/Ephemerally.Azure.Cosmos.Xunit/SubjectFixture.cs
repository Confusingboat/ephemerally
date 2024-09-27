using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Ephemerally.Azure.Cosmos.Xunit;

public interface ISubjectFixture;

public interface ISubjectFixture<TSubject> : ISubjectFixture
{
    Task<TSubject> GetOrCreateSubjectAsync();
}

[SuppressMessage("ReSharper", "UseConfigureAwaitFalse")]
public abstract class SubjectFixture<TSubject> :
    ISubjectFixture<TSubject>,
    IAsyncDisposable,
    IAsyncLifetime
{
    private readonly Lazy<Task<TSubject>> _subject;

    // Inheriting class should implement an accessor for the subject e.g.
    //      public TSubject Subject => _subject.Value.Result;

    // ReSharper disable once MemberCanBePrivate.Global
    public Task<TSubject> GetOrCreateSubjectAsync() => _subject.Value;

    protected SubjectFixture() => _subject = new(CreateSubjectAsync);

    protected abstract Task<TSubject> CreateSubjectAsync();

    protected abstract Task DisposeSubjectAsync();

    public Task InitializeAsync() => _subject.Value;

    public virtual async Task DisposeAsync()
    {
        if (!_subject.IsValueCreated) return;

        await DisposeSubjectAsync();
    }

    async ValueTask IAsyncDisposable.DisposeAsync() =>
        await ((IAsyncLifetime)this).DisposeAsync();
}