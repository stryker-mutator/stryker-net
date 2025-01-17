using System.Collections.Generic;

namespace Stryker.Abstractions.Testing;

public interface ITestIdentifiers
{
    IEnumerable<string> GetIdentifiers();
    int Count { get; }
    bool IsEmpty { get; }
    bool IsEveryTest { get; }
    ITestIdentifiers Merge(ITestIdentifiers other);
    bool Contains(string testId);
    bool ContainsAny(ITestIdentifiers other);
    bool IsIncludedIn(ITestIdentifiers other);
    ITestIdentifiers Intersect(ITestIdentifiers other);
    ITestIdentifiers Excluding(ITestIdentifiers testsToSkip);
}
