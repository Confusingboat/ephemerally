namespace Ephemerally.Azure.Cosmos.Xunit;

public abstract class ConsumingSubjectFixture<TSubject> : SubjectFixture<TSubject>
{   
    
}

file record ConsumedSubject<TSubject>(TSubject Subject, bool Dispose);