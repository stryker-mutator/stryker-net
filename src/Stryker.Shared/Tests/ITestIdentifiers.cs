namespace Stryker.Shared.Tests;
public interface ITestIdentifiers
{
    IEnumerable<Identifier> GetIdentifiers();
    int Count { get; }
    bool IsEmpty { get; }
    bool IsEveryTest { get; }
    ITestIdentifiers Merge(ITestIdentifiers other);
    bool Contains(Identifier testId);
    bool ContainsAny(ITestIdentifiers other);
    bool IsIncludedIn(ITestIdentifiers other);
    ITestIdentifiers Intersect(ITestIdentifiers other);
}
