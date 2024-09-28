using Ephemerally.Xunit;
using Shouldly;

namespace Ephemerally.Tests.Fixtures;

public class SubjectFixtureTests
{
    [Test]
    public void Fixture_should_be_constructed_as_uninitialized()
    {
        var sut = new TestSubjectFixture();
        sut.State.Value.ShouldBe(State.Uninitialized);
    }

    [Test]
    public async Task InitializeAsync_should_initialize_fixture()
    {
        var sut = new TestSubjectFixture();

        await sut.InitializeAsync();

        sut.State.Value.ShouldBe(State.Initialized);
    }

    [Test]
    public async Task DisposeAsync_on_initialized_fixture_should_dispose_fixture()
    {
        var sut = new TestSubjectFixture();
        await sut.InitializeAsync();

        await sut.DisposeAsync();

        sut.State.Value.ShouldBe(State.Disposed);
    }

    [Test]
    public async Task DisposeAsync_on_uninitialized_fixture_should_be_uninitialized()
    {
        var sut = new TestSubjectFixture();

        await sut.DisposeAsync();

        sut.State.Value.ShouldBe(State.Uninitialized);
    }

    [Test]
    public async Task GetOrCreateSubjectAsync_should_always_return_same_instance()
    {
        var sut = new ObjectSubjectFixture();
        await sut.InitializeAsync();

        var gettingRef1 = sut.GetOrCreateSubjectAsync();
        var gettingRef2 = sut.GetOrCreateSubjectAsync();
        var refs = await Task.WhenAll(gettingRef1, gettingRef2);

        refs[1].ShouldBeSameAs(refs[0]);
    }
}

file class ObjectSubjectFixture : SubjectFixture<object>
{
    protected override Task<object> CreateSubjectAsync() => Task.FromResult(new object());
}

file class TestSubjectFixture : SubjectFixture<StateWrapper>
{
    public StateWrapper State { get; } = new();

    protected override Task<StateWrapper> CreateSubjectAsync()
    {
        State.Value = Fixtures.State.Initialized;
        return Task.FromResult(State);
    }

    protected override Task DisposeSubjectAsync()
    {
        State.Value = Fixtures.State.Disposed;
        return Task.CompletedTask;
    }
}

internal class StateWrapper
{
    public State Value { get; set; } = State.Uninitialized;
}

internal enum State
{
    Uninitialized = 0,
    Initialized = 1,
    Disposed = 2
}