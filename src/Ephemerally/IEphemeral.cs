namespace Ephemerally;

public interface IEphemeral : IAsyncDisposable { }

public interface IEphemeral<out TObject> : IEphemeral
    where TObject : class
{
    TObject Value { get; }
}