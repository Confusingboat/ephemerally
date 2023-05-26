namespace Ephemeral;

public interface IEphemeral : IAsyncDisposable { }

public interface IEphemeral<TContainer> : IEphemeral
    where TContainer : class
{
    ValueTask<TContainer> GetAsync();
}