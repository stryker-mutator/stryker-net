namespace Stryker.Shared.Tests;

// represents a simple list of test identifier with some properties
public interface ITestGuids
{
    IEnumerable<Guid> GetGuids();
    int Count { get; }
    bool IsEmpty { get; }
    bool IsEveryTest { get; }
    ITestGuids Merge(ITestGuids other);
    bool Contains(Guid testId);
    bool ContainsAny(ITestGuids other);
    bool IsIncludedIn(ITestGuids other);
    ITestGuids Intersect(ITestGuids other);
}
