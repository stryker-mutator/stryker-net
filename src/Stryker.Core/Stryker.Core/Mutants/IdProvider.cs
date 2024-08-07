using System.Threading;

namespace Stryker.Core.Mutants;

public interface IProvideId
{
    int NextId();
}

internal class BasicIdProvider : IProvideId
{
    private int _id;

    public int NextId() => Interlocked.Increment(ref _id)-1;
}
