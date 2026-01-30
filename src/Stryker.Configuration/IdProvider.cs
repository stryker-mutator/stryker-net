using System.Threading;
using Stryker.Abstractions;

namespace Stryker.Configuration;

public class BasicIdProvider : IProvideId
{
    private int _id;

    public int NextId() => Interlocked.Increment(ref _id)-1;
}
